using AutoMapper;
using Azure.Core;
using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Mapping;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace DwaProject.WEB.Controllers.Public
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenreRepository _genreRepository;
        private readonly INotificationRepository _notificationRepository;
        private const string SUBJECT = "Validate your email";
        private const string LINK = "https://localhost:7291/api/users/ValidateEmail/";
        private const string BODY = "Validate your email by pressing this link: ";
        private VMFilter _filter = new VMFilter() { FilterName = "", Page = 1, TotalPages = 1 };

        public UserController(IMapper mapper, IUserRepository userRepository, IVideoRepository videoRepository,
            IImageRepository imageRepository, ICountryRepository countryRepository, IHttpContextAccessor httpContextAccessor, 
            IGenreRepository genreRepository, INotificationRepository notificationRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _videoRepository = videoRepository;
            _imageRepository = imageRepository;
            _countryRepository = countryRepository;
            _httpContextAccessor = httpContextAccessor;
            _genreRepository = genreRepository;
            _notificationRepository = notificationRepository;
        }

        //GET
        [Authorize]
        public ActionResult Index(bool posted)
        {
            //create the cookies
            var filterNameCookie = Request.Cookies["filterNameVideo"];

            if(!posted)
            {
                if (filterNameCookie != null && string.IsNullOrEmpty(_filter.FilterName))
                    _filter.FilterName = filterNameCookie;
            }

            (var blVideos, _filter.TotalPages) = _videoRepository.Search(_filter.FilterName, "name", "asc", _filter.Page, 5);
            var vmVideos = VMVideoMapper.MapToVM(blVideos);

            ViewData["Filter"] = _filter;
            ViewData["Images"] = _mapper.Map<List<VMImage>>(_imageRepository.GetAll().ToList());


            return View(vmVideos.ToList());
        }

        public ActionResult VideoTableBodyPartial(int page)
        {
            _filter.Page = page;
            
            (var blVideos, _filter.TotalPages) = _videoRepository.Search(_filter.FilterName, "name", "asc", _filter.Page, 5);
            var vmVideos = VMVideoMapper.MapToVM(blVideos);

            ViewData["Filter"] = _filter;
            ViewData["Images"] = _mapper.Map<List<VMImage>>(_imageRepository.GetAll().ToList());


            return PartialView("_PublicVideoTableBodyPartial", vmVideos.ToList());
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(VMFilter filter)
        {
            filter.FilterName ??= "";

            //update the cookies
            Response.Cookies.Append("filterNameVideo", filter.FilterName);

            //if the filter changes, goes back to the first page
            if (_filter.FilterName != filter.FilterName)
                _filter.Page = 1;
            else
                _filter.Page = filter.Page;

            _filter.FilterName = filter.FilterName;

            return RedirectToAction(nameof(Index), true);
        }

        //GET
        [Authorize]
        public ActionResult Details(int id)
        {
            var blVideo = _videoRepository.Get(id);
            var vmVideo = VMVideoMapper.MapToVM(blVideo);

            vmVideo.GenreName = _genreRepository.Get(vmVideo.GenreId)!.Name;
            ViewData["Image"] = _mapper.Map<VMImage>(_imageRepository.Get(vmVideo.ImageId));

            return View(vmVideo);
        }


        //GET
        public ActionResult SelfRegister()
        {
            ViewData["Countries"] = _mapper.Map<List<VMCountry>>(_countryRepository.GetAll().ToList());
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelfRegister(VMSelfRegister register)
        {
            if (!ModelState.IsValid)
                return View(register);

            try
            {
                var userRequest = VMRegisterMapper.MapToBL(register);
                var newUser = _userRepository.Register(userRequest);

                BLNotification notification = new BLNotification
                {
                    CreatedAt = DateTime.UtcNow,
                    ReceiverEmail = newUser.Email,
                    Subject = SUBJECT,
                    Body = BODY + LINK + HttpUtility.UrlEncode(newUser.Username) + "/" + HttpUtility.UrlEncode(newUser.SecurityToken)
                };


                notification = _notificationRepository.Post(notification);
                _notificationRepository.Send(notification);

                return RedirectToAction(nameof(Login));
            }
            catch(Exception ex)
            {
                ViewData["Countries"] = _mapper.Map<List<VMCountry>>(_countryRepository.GetAll().ToList());
                ModelState.AddModelError("Username", ex.Message);
                return View(register);
            }
            
        }

        //GET
        [HttpGet("api/users/ValidateEmail/{username}/{b64SecToken}")]
        public ActionResult ValidateEmail(VMValidateEmail validateEmail)
        {
            if (!ModelState.IsValid)
                return View(validateEmail);

            var request = _mapper.Map<ValidateEmailRequest>(validateEmail);

            _userRepository.ValidateEmail(request);

            return RedirectToAction(nameof(Login));
        }


        //GET
        public ActionResult Login()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(VMLogin login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = _userRepository.GetConfirmedUser(
                login.Username,
                login.Password);

            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password");
                return View(login);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties()).Wait();

            return RedirectToAction(nameof(Index));
        }


        //POST
        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction(nameof(Login));
        }


        //GET
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(VMChangePassword changePassword)
        {
            var blChangePassword = _mapper.Map<BLChangePassword>(changePassword);

            if(!_userRepository.ChangePassword(blChangePassword))
            {
                ModelState.AddModelError("Username", "Invalid username or password");
                return View("ChangePassword");
            }
            

            return RedirectToAction(nameof(UserProfile));
        }

        //GET
        [Authorize]
        public ActionResult UserProfile()
        {
            var blUser = _userRepository.Get(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value!);

            //it means no one is connected
            if (blUser == null)
                return RedirectToAction(nameof(Logout));

            var blCountries = _countryRepository.GetAll();
            var vmCountries = _mapper.Map<List<VMCountry>>(blCountries.ToList());
            ViewData["Countries"] = vmCountries;

            var vmUser = _mapper.Map<VMUser>(blUser);

            return View(vmUser);
        }
    }
}
