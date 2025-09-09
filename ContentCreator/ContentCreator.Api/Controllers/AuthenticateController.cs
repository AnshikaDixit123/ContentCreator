using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AuthenticateController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("AuthenticateAdmin")]
        public async Task<IActionResult> AuthenticateAdmin([FromForm] SigningRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData();
            try
            {
                response = await _accountService.AuthenticateAdminAsync(request, cancellation);
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
