using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.WEB.Viewmodels;

namespace DwaProject.WEB.Mapping
{
	public class AutomapperProfile : Profile
	{
		public AutomapperProfile()
		{
			CreateMap<BLCountry, VMCountry>();
			CreateMap<BLTag, VMTag>().ReverseMap();
			CreateMap<BLImage, VMImage>().ReverseMap();
			CreateMap<BLUser, VMUser>().ReverseMap();
			CreateMap<BLGenre, VMGenre>().ReverseMap();
			CreateMap<VMValidateEmail, ValidateEmailRequest>();
			CreateMap<VMChangePassword, BLChangePassword>();
		}
	}
}
