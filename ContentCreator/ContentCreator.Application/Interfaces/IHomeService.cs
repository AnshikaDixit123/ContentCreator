using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IHomeService
    {        
        Task<ResponseData<UserResponseModel>> GetMyProfileAsync(Guid UserId, CancellationToken cancellation);
        Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> SaveChangesAsync(SaveChangesRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> CreateRolesAsync(CreateRolesRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> AddCountryAsync(AddCountryRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> AddStateAsync(AddStateRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> AddCityAsync(AddCityRequest request, CancellationToken cancellation);
        Task<ResponseData<List<CountryResponseModel>>> GetCountryAsync(CancellationToken cancellation);
        Task<ResponseData<List<StateResponseModel>>> GetStateAsync(Guid CountryId, CancellationToken cancellation);
        Task<ResponseData<List<CityResponseModel>>> GetCityAsync(Guid CountryId, Guid StateId, CancellationToken cancellation);
        Task<ResponseData<List<CountryStateCityNestedResponseModel>>> CountryStateCityNestedAsync(CancellationToken cancellation);
    }
}
