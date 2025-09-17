using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IHomeService
    {        
        Task<ResponseData<bool>> GetMyProfileAsync(GetMyProfileRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> CreateRolesAsync(CreateRolesRequest request, CancellationToken cancellation);
        Task<ResponseData<List<CountryResponseModel>>> GetCountryAsync(CancellationToken cancellation);
        Task<ResponseData<List<StateResponseModel>>> GetStateAsync(CancellationToken cancellation);
        Task<ResponseData<List<CityResponseModel>>> GetCityAsync(CancellationToken cancellation);
        Task<ResponseData<List<CountryStateCityNestedResponseModel>>> CountryStateCityNestedAsync(CancellationToken cancellation);
    }
}
