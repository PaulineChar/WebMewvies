using DwaProject.BL.BLModels;
using DwaProject.WEB.Viewmodels;

namespace DwaProject.WEB.Mapping
{
    public static class VMRegisterMapper
    {
       public static UserRegisterRequest MapToBL(VMSelfRegister request) =>
            new UserRegisterRequest
            {
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CountryOfResidenceId = request.CountryOfResidenceId
            };
    }
}
