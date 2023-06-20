using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.WEB.Controllers.Admin
{
    public class AdminGenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        private static VMFilter _filter = new VMFilter() { Page = 1, TotalPages = 1 , FilterName=""};
        
        public AdminGenreController(IGenreRepository genreRepository, IMapper mapper) 
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        // GET: AdminGenreController
        public ActionResult Index(bool isAjax)
        {
            (var blGenres, _filter.TotalPages) = _genreRepository.SearchAndPage(_filter.FilterName, _filter.Page);


            var vmGenres = _mapper.Map<List<VMGenre>>(blGenres.ToList());

            ViewData["Filter"] = _filter;

            if(isAjax)
                return PartialView("IndexPartial", vmGenres);

            return View(vmGenres);
        }


        public ActionResult GenreTableBodyPartial(VMFilter filter)
        {
            _filter.Page = filter.Page;
            (var blGenres, _filter.TotalPages) = _genreRepository.SearchAndPage(_filter.FilterName, _filter.Page);


            var vmGenres = _mapper.Map<List<VMGenre>>(blGenres.ToList());

            ViewData["Filter"] = _filter;

            return PartialView("_GenreTableBodyPartial", vmGenres);
        }

        
        // GET: AdminGenreController/CreatePartial
        public ActionResult CreatePartial()
        {
            return PartialView("CreatePartial");
        }

        // POST: AdminGenreController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartial(VMGenre vmGenre)
        {
            try
            {
                var blGenre = _mapper.Map<BLGenre>(vmGenre);
                _genreRepository.Post(blGenre);

                (var blGenres, _filter.TotalPages) = _genreRepository.SearchAndPage(_filter.FilterName, _filter.Page);
                var vmGenres = _mapper.Map<List<VMGenre>>(blGenres.ToList());

                ViewData["Filter"] = _filter;

                return PartialView("IndexPartial", vmGenres);
            }
            catch
            {
                return PartialView();
            }
        }

        // GET: AdminGenreController/Edit/5
        public ActionResult EditPartial(int id)
        {
            var blGenre = _genreRepository.Get(id);
            var vmGenre = _mapper.Map<VMGenre>(blGenre);

            return PartialView("EditPartial", vmGenre);
        }

        // POST: AdminGenreController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPartial(int id, VMGenre vmGenre)
        {
            try
            {
                var blGenre = _mapper.Map<BLGenre>(vmGenre);
                _genreRepository.Post(id, blGenre);

                (var blGenres, _filter.TotalPages) = _genreRepository.SearchAndPage(_filter.FilterName, _filter.Page);
                var vmGenres = _mapper.Map<List<VMGenre>>(blGenres.ToList());

                ViewData["Filter"] = _filter;

                return PartialView("IndexPartial", vmGenres);

                //return RedirectToAction(nameof(Index), true);
            }
            catch
            {
                return PartialView(vmGenre);
            }
        }

        // GET: AdminGenreController/Delete/5
        public ActionResult DeletePartial(int id)
        {
            var blGenre = _genreRepository.Get(id);
            var vmGenre = _mapper.Map<VMGenre>(blGenre);

            return PartialView("DeletePartial", vmGenre);
        }

        // POST: AdminGenreController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePartial(int id, VMGenre vmGenre)
        {
            try
            {
                _genreRepository.Delete(id);

                (var blGenres, _filter.TotalPages) = _genreRepository.SearchAndPage(_filter.FilterName, _filter.Page);
                var vmGenres = _mapper.Map<List<VMGenre>>(blGenres.ToList());

                ViewData["Filter"] = _filter;

                return PartialView("IndexPartial", vmGenres);
                //return RedirectToAction(nameof(Index), true);
            }
            catch
            {
                return RedirectToAction(nameof(DeleteError));
            }
        }

        public ActionResult DeleteError()
        { 
            return View();
        }
    }
}
