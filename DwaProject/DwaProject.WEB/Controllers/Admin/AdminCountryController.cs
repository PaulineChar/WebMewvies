using AutoMapper;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.WEB.Controllers.Admin
{
    public class AdminCountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private static VMFilter _filter = new VMFilter() { Page = 1, TotalPages = 1};
        public AdminCountryController(ICountryRepository countryRepo, IMapper mapper) 
        {
            _countryRepository = countryRepo;
            _mapper = mapper;
        }



        public ActionResult Index()
        {
            try
            {
                (var blCountries, _filter.TotalPages) = _countryRepository.GetAndPage(_filter.Page);


                var vmCountries = _mapper.Map<List<VMCountry>>(blCountries.ToList());

                ViewData["Filter"] = _filter;

                return View(vmCountries);
            }
            catch
            {
                return View();
            }

            
        }

        public ActionResult CountryTableBodyPartial(int page)
        {
            _filter.Page = page;
            (var blCountries, _filter.TotalPages) = _countryRepository.GetAndPage(_filter.Page);
            var vmCountries = _mapper.Map<List<VMCountry>>(blCountries.ToList());

            ViewData["Filter"] = _filter;

            return PartialView("_CountryTableBodyPartial", vmCountries);
        }

        
    }
}
