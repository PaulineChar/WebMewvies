using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Web;

namespace DwaProject.WEB.Controllers.Admin
{
    public class AdminUserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;
        private readonly INotificationRepository _notificationRepository;
        private VMFilter _filter = new VMFilter() { FilterName = "", FilterType = "firstName", Page = 1, TotalPages = 1 };
        

        public AdminUserController(IUserRepository userRepository, IMapper mapper, ICountryRepository countryRepository, INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _countryRepository = countryRepository;
            _notificationRepository = notificationRepository;
        }



        // GET: AdminUserController
        public ActionResult Index()
        {
            ViewData["Filter"] = _filter;

            //create the cookies
            CookieFetch();

            (var blUsers, _filter.TotalPages) = _userRepository.Search(_filter.FilterName, _filter.FilterType, _filter.Page);
            var vmUsers = _mapper.Map<List<VMUser>>(blUsers.ToList());

            return View(vmUsers);
        }

        public ActionResult UserTableBodyPartial(int page)
        {
            _filter.Page = page;
            CookieFetch();

            (var blUsers, _filter.TotalPages) = _userRepository.Search(_filter.FilterName, _filter.FilterType, _filter.Page);
            var vmUsers = _mapper.Map<List<VMUser>>(blUsers.ToList());

            ViewData["Filter"] = _filter;

            return PartialView("_UserTableBodyPartial", vmUsers);
        }

        private void CookieFetch()
        {
            var filterNameCookie = Request.Cookies["filterNameUser"];
            var filterTypeCookie = Request.Cookies["filterTypeUser"];
            if (filterNameCookie != null && string.IsNullOrEmpty(_filter.FilterName))
                _filter.FilterName = filterNameCookie;

            if (filterTypeCookie != null && _filter.FilterType == "firstName")
                _filter.FilterType = filterTypeCookie;
        }

        //POST: AdminuserController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(VMFilter filter)
        {
            filter.FilterName ??= "";

            //update the cookies
            Response.Cookies.Append("filterNameUser", filter.FilterName);
            Response.Cookies.Append("filterTypeUser", filter.FilterType);

            //if the filter changes, goes back to the first page
            if (_filter.FilterType != filter.FilterType || _filter.FilterName != filter.FilterName)
                _filter.Page = 1;
            else
                _filter.Page = filter.Page;

            _filter.FilterName = filter.FilterName;
            _filter.FilterType = filter.FilterType;

            return RedirectToAction("Index");
        }

        // GET: AdminUserController/Details/5
        public ActionResult Details(int id)
        {
            var blUser = _userRepository.Get(id);
            var vmUser = _mapper.Map<VMUser>(blUser);

            ViewData["country"] = _mapper.Map<VMCountry>(_countryRepository.Get(vmUser.CountryOfResidenceId));

            return View(vmUser);
        }

        // GET: AdminUserController/Create
        public ActionResult Create()
        {
            var blCountries = _countryRepository.GetAll().ToList();
            var vmCountries = _mapper.Map<List<VMCountry>>(blCountries);
            ViewData["Country"] = vmCountries;

            return View();
        }

        // POST: AdminUserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserRegisterRequest request)
        {
            try
            {
                var newUser = _userRepository.Register(request);
                var validateRequest = new ValidateEmailRequest()
                {
                    Username = newUser.Username,
                    B64SecToken = newUser.SecurityToken
                };
                _userRepository.ValidateEmail(validateRequest);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                var blCountries = _countryRepository.GetAll().ToList();
                var vmCountries = _mapper.Map<List<VMCountry>>(blCountries);
                ViewData["Country"] = vmCountries;
                ModelState.AddModelError("Username", ex.Message);
                return View(request);
            }
        }

        // GET: AdminUserController/Edit/5
        public ActionResult Edit(int id)
        {
            var blCountries = _countryRepository.GetAll().ToList();
            var vmCountries = _mapper.Map<List<VMCountry>>(blCountries);
            ViewData["Country"] = vmCountries;

            var blUser = _userRepository.Get(id);
            var request = new UserRegisterRequest()
            {
                Username = blUser.Username,
                Email = blUser.Email,
                FirstName = blUser.FirstName,
                LastName = blUser.LastName,
                Phone = blUser.Phone,
                //no password written
                CountryOfResidenceId = blUser.CountryOfResidenceId
            };
            return View(request);
        }

        // POST: AdminUserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserRegisterRequest request)
        {
            try
            {
                var user = _userRepository.Edit(id, request);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                var blCountries = _countryRepository.GetAll().ToList();
                var vmCountries = _mapper.Map<List<VMCountry>>(blCountries);
                ViewData["Country"] = vmCountries;
                ModelState.AddModelError("Username", ex.Message);
                return View(request);
            }
        }

        // GET: AdminUserController/Delete/5
        public ActionResult Delete(int id)
        {
            var blUser = _userRepository.Get(id);
            var vmUser = _mapper.Map<VMUser>(blUser);

            return View(vmUser);
        }

        // POST: AdminUserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMUser vmUser)
        {
            try
            {
                _userRepository.SoftDelete(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(vmUser);
            }
        }
    }
}
