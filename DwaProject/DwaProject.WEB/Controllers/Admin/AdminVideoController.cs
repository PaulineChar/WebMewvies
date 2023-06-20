using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using DwaProject.WEB.Mapping;
using DwaProject.WEB.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.WEB.Controllers.Admin
{
    public class AdminVideoController : Controller
	{
		private readonly IVideoRepository _videoRepository;
		private readonly IMapper _mapper;
		private readonly IImageRepository _imageRepository;
		private readonly IGenreRepository _genreRepository;
		private readonly ITagRepository _tagRepository;

		private VMFilter _filter = new VMFilter() { Page = 1, FilterName = "", TotalPages = 1, FilterType = "video"};

		public AdminVideoController(IVideoRepository videoRepository, IMapper mapper, ITagRepository tagRepository, IImageRepository imageRepository, IGenreRepository genreRepository)
		{
			_videoRepository = videoRepository;
			_mapper = mapper;
			_imageRepository = imageRepository;
			_genreRepository = genreRepository;
			_tagRepository = tagRepository;
		}



		// GET: VideoController
		public ActionResult Index()
		{
			//create the cookies
			CookieFetch();
			
            IEnumerable<BLVideo> blVideos;

			if(_filter.FilterType == "video")
				(blVideos, _filter.TotalPages) = _videoRepository.Search(_filter.FilterName, "name", "asc", _filter.Page, 5);
			else
			{
				(blVideos, _filter.TotalPages) = _videoRepository.SearchByGenreName(_filter.FilterName, _filter.Page, 5);
			}

			ViewData["Filter"] = _filter;

			var vmVideos = VMVideoMapper.MapToVM(blVideos).ToList();
			var blList = blVideos.ToList();
			

			//get genre
			for (int i = 0; i < vmVideos.Count; i++)
			{
				vmVideos[i].GenreName = _genreRepository.Get(blList[i].GenreId).Name;
			}

			return View(vmVideos);
		}

        private void CookieFetch()
        {
            var filterNameCookie = Request.Cookies["filterName"];
            var filterTypeCookie = Request.Cookies["filterType"];

            if (filterNameCookie != null && string.IsNullOrEmpty(_filter.FilterName))
                _filter.FilterName = filterNameCookie;

            if (filterTypeCookie != null && _filter.FilterType == "video")
                _filter.FilterType = filterTypeCookie;
        }

        public ActionResult VideoTableBodyPartial(int page)
        {
            _filter.Page = page;
			CookieFetch();

            IEnumerable<BLVideo> blVideos;

            if (_filter.FilterType == "video")
                (blVideos, _filter.TotalPages) = _videoRepository.Search(_filter.FilterName, "name", "asc", _filter.Page, 5);
            else
            {
                (blVideos, _filter.TotalPages) = _videoRepository.SearchByGenreName(_filter.FilterName, _filter.Page, 5);
            }

            ViewData["Filter"] = _filter;

            var vmVideos = VMVideoMapper.MapToVM(blVideos).ToList();
            var blList = blVideos.ToList();


            //get genre
            for (int i = 0; i < vmVideos.Count; i++)
            {
                vmVideos[i].GenreName = _genreRepository.Get(blList[i].GenreId).Name;
            }

            return PartialView("_VideoTableBodyPartial", vmVideos);
        }

		//Post: VideoController/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(VMFilter filter)
		{
            filter.FilterName ??= "";

            //update the cookies
            Response.Cookies.Append("filterName", filter.FilterName);
			Response.Cookies.Append("filterType", filter.FilterType);
			
			if (_filter.FilterType != filter.FilterType || _filter.FilterName != filter.FilterName)
				_filter.Page = 1;
			else
				_filter.Page = filter.Page;

            _filter.FilterName = filter.FilterName;
            _filter.FilterType = filter.FilterType;

            return RedirectToAction("Index");
		}


		// GET: VideoController/Details/5
		public ActionResult Details(int id)
		{
            var blVideo = _videoRepository.Get(id);
            var video = VMVideoMapper.MapToVM(blVideo);
            video.GenreName = _genreRepository.Get(blVideo.GenreId).Name;
			
			BLImage blImage = _imageRepository.Get((int)blVideo.ImageId);
			VMImage vmImage = _mapper.Map<VMImage>(blImage);
			ViewData["Image"] = vmImage;

            return View(video);
		}

		// GET: VideoController/Create
		public ActionResult Create()
		{
            ViewData["Genres"] = _genreRepository.GetAll().ToList();
            ViewData["Images"] = _mapper.Map<List<VMImage>>(_imageRepository.GetAll().ToList());
			ViewData["Tags"] = _tagRepository.GetAll().ToList();

            return View();
		}

		// POST: VideoController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(VMVideo video)
		{
			try
			{
                //handle image
				if(video.ImageFile != null)
				{
                    var imageArray = GetFileByteAray(video.ImageFile);
                    if (imageArray != null)
                    {
						// To simplify things - we always add image
						VMImage vmImage = new VMImage()
						{
							Content = Convert.ToBase64String(imageArray)
						};
                        video.ImageId = _imageRepository.Post(_mapper.Map<BLImage>(vmImage));
                    }
                }
                else
					video.ImageId = (int)_imageRepository.GetAll().First(i => i.Content == video.ImageContent).Id!;
				
				var blVideo = VMVideoMapper.MapToBL(video);

                _videoRepository.Post(blVideo);

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: VideoController/Edit/5
		public ActionResult Edit(int id)
		{
			var blVideo = _videoRepository.Get(id);
            VMVideo videoToEdit = VMVideoMapper.MapToVM(blVideo);
            videoToEdit.GenreName = _genreRepository.Get(blVideo.GenreId).Name;

            ViewData["Genres"] = _genreRepository.GetAll().ToList();
			ViewData["Images"] = _mapper.Map<List<VMImage>>(_imageRepository.GetAll().ToList());
            ViewData["Tags"] = _tagRepository.GetAll().ToList();

            return View(videoToEdit);
		}

		// POST: VideoController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, VMVideo video)
		{
			try
			{
                //handle image
                if (video.ImageFile != null)
                {
                    var imageArray = GetFileByteAray(video.ImageFile);
                    if (imageArray != null)
                    {
                        // To simplify things - we always add image
                        VMImage vmImage = new VMImage()
                        {
                            Content = Convert.ToBase64String(imageArray)
                        };
                        video.ImageId = _imageRepository.Post(_mapper.Map<BLImage>(vmImage));
                    }
                }
                else
                    video.ImageId = (int)_imageRepository.GetAll().First(i => i.Content == video.ImageContent).Id!;

                var blVideo = VMVideoMapper.MapToBL(video);

				_videoRepository.Post(id, blVideo);

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View(video);
			}
		}

		// GET: VideoController/Delete/5
		public ActionResult Delete(int id)
		{
			var blVideo = _videoRepository.Get(id);
			var video = VMVideoMapper.MapToVM(blVideo);
            video.GenreName = _genreRepository.Get(blVideo.GenreId).Name;

            return View(video);
		}

		// POST: VideoController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, VMVideo video)
		{
			try
			{
				_videoRepository.Delete(id);

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View(video);
			}
		}

        private static byte[] GetFileByteAray(IFormFile formFile)
        {
            if (formFile != null)
            {
                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        formFile.CopyTo(memoryStream);

                        if (memoryStream.Length < 50 * 1024 * 1024)
                        {
                            return memoryStream.ToArray();
                        }
                    }

                }
            }

            return null;
        }
    }
}
