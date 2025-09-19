using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromForm] CreateNewUserRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
                response = await _homeService.CreateUserAsync(request, cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("CreateRoles")]
        public async Task<IActionResult> CreateRoles([FromForm] CreateRolesRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
              
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile(Guid UserId, CancellationToken cancellation)
        {
            var response = new ResponseData<UserResponseModel>();
            try
            {
                response = await _homeService.GetMyProfileAsync(UserId, cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetCountry")]
        public async Task<IActionResult> GetCountry(CancellationToken cancellation)
        {
            var response = new ResponseData<List<CountryResponseModel>>();
            try
            {
                response = await _homeService.GetCountryAsync(cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetState")]
        public async Task<IActionResult> GetState(Guid CountryId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<StateResponseModel>>();
            try
            {
                response = await _homeService.GetStateAsync(CountryId, cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetCity")]
        public async Task<IActionResult> GetCity(Guid StateId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<CityResponseModel>>();
            try
            {
                response = await _homeService.GetCityAsync(StateId, cancellation);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("CountryStateCityNested")]
        public async Task<IActionResult> CountryStateCityNested(CancellationToken cancellation)
        {
            var response = new ResponseData<List<CountryStateCityNestedResponseModel>>();
            try
            {
                response = await _homeService.CountryStateCityNestedAsync(cancellation);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("SaveChanges")]
        public async Task<IActionResult> SaveChanges([FromForm] SaveChangesRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
                response = await _homeService.SaveChangesAsync(request, cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
