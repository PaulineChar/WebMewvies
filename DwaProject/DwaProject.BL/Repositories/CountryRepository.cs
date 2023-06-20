using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Repositories
{
    public interface ICountryRepository
    {
        (IEnumerable<BLCountry>, int) GetAndPage(int page);

        IEnumerable<BLCountry> Search(string namePart);

        IEnumerable<BLCountry> GetAll();

        BLCountry Get(int id);
    }
    public class CountryRepository : ICountryRepository
    {
        private readonly RwaMoviesContext _dbContext;

        public CountryRepository(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BLCountry Get(int id)
        {
            Country dbCountry = _dbContext.Countries.FirstOrDefault(country => country.Id == id)!;
            BLCountry blCountry = CountryMapper.MapToBL(dbCountry);
            return blCountry;
        }

        public IEnumerable<BLCountry> GetAll()
        {
            var dbCountries = _dbContext.Countries;
            var blCountries = CountryMapper.MapToBL(dbCountries);

            return blCountries;
        }

        public (IEnumerable<BLCountry>, int) GetAndPage(int page)
        {
            var size = 10;

            var dbCountries = _dbContext.Countries;
            var countries = CountryMapper.MapToBL(dbCountries);

            var totalPages = countries.Count()/size + (countries.Count()%size == 0 ? 0 : 1);
            countries = countries.Skip((page - 1) * size).Take(size);

            return (countries, totalPages);
        }

        public IEnumerable<BLCountry> Search(string namePart)
        {
            //Ignore case
            var countries = _dbContext.Countries.Where(c => c.Name.ToLower().Contains(namePart.ToLower()));
            var blCountries = CountryMapper.MapToBL(countries);

            return blCountries;
        }
    }
    
}
