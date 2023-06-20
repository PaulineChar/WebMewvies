using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.WEB.Controllers.Admin
{
    public class AdminTagController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepository;
        private static VMFilter _filter = new VMFilter() { Page = 1, TotalPages = 1 };
        public AdminTagController(IMapper mapper, ITagRepository tagRepository) 
        {
            _mapper = mapper;
            _tagRepository = tagRepository;
        }

        // GET: AdminTagController
        public ActionResult Index()
        {
            (var blTag, _filter.TotalPages) = _tagRepository.GetAndPage(_filter.Page);
            var vmTag = _mapper.Map<List<VMTag>>(blTag.ToList());

            ViewData["Filter"] = _filter;

            return View(vmTag);
        }

        public ActionResult TagTableBodyPartial(int page)
        {
            _filter.Page = page;
            (var blTags, _filter.TotalPages) = _tagRepository.GetAndPage(_filter.Page);
            var vmTags = _mapper.Map<List<VMTag>>(blTags.ToList());

            ViewData["Filter"] = _filter;

            return PartialView("_TagTableBodyPartial", vmTags);
        }

        // GET: AdminTagController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminTagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VMTag tag)
        {
            try
            {
                var blTag = _mapper.Map<BLTag>(tag);
                _tagRepository.Post(blTag);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminTagController/Edit/5
        public ActionResult Edit(int id)
        {
            var blTag = _tagRepository.Get(id);
            var vmTag = _mapper.Map<VMTag>(blTag);

            return View(vmTag);
        }

        // POST: AdminTagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VMTag tag)
        {
            try
            {
                var blTag = _mapper.Map<BLTag>(tag);
                _tagRepository.Post(id, blTag);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(tag);
            }
        }

        // GET: AdminTagController/Delete/5
        public ActionResult Delete(int id)
        {
            var blTag = _tagRepository.Get(id);
            var vmTag = _mapper.Map<VMTag>(blTag);

            return View(vmTag);
        }

        // POST: AdminTagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMTag tag)
        {
            try
            {
                _tagRepository.Delete(id);
                return RedirectToAction(nameof(Index));
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
