using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;

namespace DwaProject.BL.Mapping
{
    public static class CountryMapper
    {
        public static IEnumerable<Country> MapToDAL(IEnumerable<BLCountry> countries) =>
            countries.Select(x => MapToDAL(x));

        public static Country MapToDAL(BLCountry country) =>
            new Country
            {
                Id = country.Id ?? 0,
                Name = country.Name,
               Code = country.Code
            };

        public static IEnumerable<BLCountry> MapToBL(IEnumerable<Country> countries) =>
            countries.Select(x => MapToBL(x));

        public static BLCountry MapToBL(Country country) =>
            new BLCountry
            {
                Id = country.Id,
                Name = country.Name,
                Code = country.Code
            };
    }
}

