using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DwaProject.BL.Repositories
{

	public interface IUserRepository
	{
		BLUser Register(UserRegisterRequest request);
		void ValidateEmail(ValidateEmailRequest request);
		Tokens JwtTokens(JwtTokensRequest request);
        BLUser Edit(int id, UserRegisterRequest request);
        BLUser SoftDelete(int id);
		IEnumerable<BLUser> GetAll();
		BLUser Get(int id);
		BLUser? Get(string username);

        (IEnumerable<BLUser>, int) Search(string namePart, string type, int page);
        BLUser GetConfirmedUser(string username, string password);

		bool ChangePassword(BLChangePassword request);
    }


	public class UserRepository : IUserRepository
	{
		private readonly RwaMoviesContext _dbContext;
		private readonly IConfiguration _configuration;
		private readonly ICountryRepository _countryRepository;
		public UserRepository(RwaMoviesContext dbContext, IConfiguration configuration, ICountryRepository countryRepository)
		{
			_dbContext = dbContext;
			_configuration = configuration;
			_countryRepository = countryRepository;
		}
		public BLUser Register(UserRegisterRequest request)
		{
			//get users from database
			var dbUsers = _dbContext.Users;
	/*		var users = UserMapper.MapToBL(dbUsers);*/

			// Username: Normalize and check if username exists
			var normalizedUsername = request.Username.ToLower().Trim();
			var normalizedEmail = request.Email.ToLower().Trim();
			if (dbUsers.Any(x => x.Username.Equals(normalizedUsername)))
				throw new InvalidOperationException("Username already exists");
			if(dbUsers.Any(x => x.Email.Equals(normalizedEmail)))
                throw new InvalidOperationException("Email already exists");


            // Password: Salt and hash password
            (byte[] salt, string b64Salt) = GenerateSalt();

			string b64Hash = GenerateHash(request.Password, salt);

			// SecurityToken: Random security token
			byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
			string b64SecToken = Convert.ToBase64String(securityToken);

			// New user
			var newUser = new User
			{
				FirstName = request.FirstName,
				LastName = request.LastName,
				Username = request.Username,
				Email = request.Email,
				Phone = request.Phone,
				CountryOfResidenceId = request.CountryOfResidenceId,
				IsConfirmed = false,
				SecurityToken = b64SecToken,
				PwdSalt = b64Salt,
				PwdHash = b64Hash,
				CreatedAt = DateTime.UtcNow
			};
			_dbContext.Users.Add(newUser);
			_dbContext.SaveChanges();

			return UserMapper.MapToBL(newUser);
		}


		public void ValidateEmail(ValidateEmailRequest request)
		{

			var target = _dbContext.Users.FirstOrDefault(x =>
				x.Username == request.Username && x.SecurityToken == request.B64SecToken);

			if (target == null)
				throw new InvalidOperationException("Authentication failed");

			target.IsConfirmed = true;
			_dbContext.SaveChanges();
		}

		private bool Authenticate(string username, string password)
		{
			var target = _dbContext.Users.Single(x => x.Username == username);

			if (!target.IsConfirmed)
				return false;

			// Get stored salt and hash
			byte[] salt = Convert.FromBase64String(target.PwdSalt);
			byte[] hash = Convert.FromBase64String(target.PwdHash);

			byte[] calcHash =
				KeyDerivation.Pbkdf2(
					password: password,
					salt: salt,
					prf: KeyDerivationPrf.HMACSHA256,
					iterationCount: 100000,
					numBytesRequested: 256 / 8);

			return hash.SequenceEqual(calcHash);
		}

		public Tokens JwtTokens(JwtTokensRequest request)
		{
			var isAuthenticated = Authenticate(request.Username, request.Password);

			if (!isAuthenticated)
				throw new InvalidOperationException("Authentication failed");

			// Get secret key bytes
			var jwtKey = _configuration["JWT:Key"];
			var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

			// Create a token descriptor (represents a token, kind of a "template" for token)
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
				{
					new System.Security.Claims.Claim(ClaimTypes.Name, request.Username),
					new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    //new System.Security.Claims.Claim(ClaimTypes.Role, "User")
                }),
				Issuer = _configuration["JWT:Issuer"],
				Audience = _configuration["JWT:Audience"],
				Expires = DateTime.UtcNow.AddMinutes(10),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(jwtKeyBytes),
					SecurityAlgorithms.HmacSha256Signature)
			};

			// Create token using that descriptor, serialize it and return it
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var serializedToken = tokenHandler.WriteToken(token);

			return new Tokens
			{
				Token = serializedToken
			};
		}

        public BLUser Edit(int id, UserRegisterRequest request)
        {
            
            var dbUsers = _dbContext.Users;
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            var normalizedUsername = user.Username.ToLower().Trim();
            var normalizedEmail = user.Email.ToLower().Trim();

            user.CountryOfResidenceId = request.CountryOfResidenceId;
			user.FirstName = request.FirstName;
			user.LastName = request.LastName;
			if(user.Email != request.Email)
			{
                if (dbUsers.Any(x => x.Email.Equals(normalizedEmail)))
                    throw new InvalidOperationException("Email already exists");
            }
			user.Email = request.Email;
			user.Phone = request.Phone;
			if(user.Username != request.Username)
			{
                if (dbUsers.Any(x => x.Username.Equals(normalizedUsername)))
                    throw new InvalidOperationException("Username already exists");
            }
            user.Username = request.Username;

			if(request.Password != "********")
			{
				// Password: Salt and hash password
				(byte[] salt, string b64Salt) = GenerateSalt();

                string b64Hash = GenerateHash(request.Password, salt);

                user.PwdHash = b64Hash;
				user.PwdSalt = b64Salt;
            }

            _dbContext.SaveChanges();

            var blUser = UserMapper.MapToBL(user);

            return blUser;
        }

        public BLUser SoftDelete(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
			if(user != null)
			{
				user.DeletedAt = DateTime.UtcNow;
				_dbContext.SaveChanges();
			}	

			var blUser = UserMapper.MapToBL(user);
			return blUser;
        }

		//All the users that are not soft-deleted
		public IEnumerable<BLUser> GetAll()
		{
			var users = _dbContext.Users.Where(u => !u.DeletedAt.HasValue);
			var blUsers = UserMapper.MapToBL(users);

			return blUsers;
		}

		public BLUser Get(int id)
		{
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            var blUser = UserMapper.MapToBL(user);

            return blUser;
        }

        public BLUser? Get(string username)
		{
			var user = _dbContext.Users.FirstOrDefault(user => user.Username == username);
			if (user == null)
				return null;

			var blUser = UserMapper.MapToBL(user);
			return blUser;

		}

        public (IEnumerable<BLUser>, int) Search(string namePart, string type, int page)
		{
			namePart = namePart.ToLower();
			IEnumerable<User> users;

			if(type == "firstName")
			{
				users = _dbContext.Users.Where(u => u.FirstName.ToLower().Contains(namePart) && !u.DeletedAt.HasValue);
			}
			else if(type == "lastName")
			{
                users = _dbContext.Users.Where(u => u.LastName.ToLower().Contains(namePart) && !u.DeletedAt.HasValue);
            }
			else
			{
				var countries = _countryRepository.Search(namePart);
				List<int> countriesID = countries.Select(c => (int)c.Id!).ToList();

                users = _dbContext.Users.Where(u => countriesID.Contains(u.CountryOfResidenceId) && !u.DeletedAt.HasValue);
            }

			var blUsers = UserMapper.MapToBL(users);

			int size = 10;
            int count = blUsers.Count();

            int totalPages = count / size + (count % size == 0 ? 0 : 1);

            //Paging
            //First page is 1
            blUsers =
                blUsers.Skip((page - 1) * size)
                .Take(size);

            return (blUsers, totalPages);
        }

        public BLUser GetConfirmedUser(string username, string password)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.IsConfirmed == true && !x.DeletedAt.HasValue);

			if (dbUser == null)
				return null;
			
            var salt = Convert.FromBase64String(dbUser.PwdSalt);
            var b64Hash = GenerateHash(password, salt);

			if (dbUser.PwdHash != b64Hash)
				return null;

			var blUser = UserMapper.MapToBL(dbUser);

            return blUser;
        }

		private static string GenerateHash(string password, byte[] salt)
		{
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

			return b64Hash;
        }

        private static (byte[], string) GenerateSalt()
        {
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var b64Salt = Convert.ToBase64String(salt);

            return (salt, b64Salt);
        }


        public bool ChangePassword(BLChangePassword request)
        {
            var userToChangePassword = _dbContext.Users.FirstOrDefault(x =>
                x.Username == request.Username &&
                !x.DeletedAt.HasValue);

			if (userToChangePassword == null)
				return false;

            var salt = Convert.FromBase64String(userToChangePassword.PwdSalt);
            var b64Hash = GenerateHash(request.Password, salt);

            if (userToChangePassword.PwdHash != b64Hash)
                return false;

            (salt, var b64Salt) = GenerateSalt();

            b64Hash = GenerateHash(request.NewPassword, salt);

            userToChangePassword.PwdHash = b64Hash;
            userToChangePassword.PwdSalt = b64Salt;

            _dbContext.SaveChanges();
			return true;
        }
    }
}
