using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
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
    }
}
