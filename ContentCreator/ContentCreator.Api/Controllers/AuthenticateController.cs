using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Enums;
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
            var response = new ResponseData<LoginResponseModel>();
            try
            {
                response = await _accountService.AuthenticateLoginAsync(request, RoleType.Admin.ToString(), cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("AuthenticateContentCreator")]
        public async Task<IActionResult> AuthenticateContentCreator([FromForm] SigningRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<LoginResponseModel>();
            try
            {
                response = await _accountService.AuthenticateLoginAsync(request, RoleType.ContentCreator.ToString(), cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("AuthenticateEndUser")]
        public async Task<IActionResult> AuthenticateEndUser([FromForm] SigningRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<LoginResponseModel>();
            try
            {
                response = await _accountService.AuthenticateLoginAsync(request, RoleType.EndUser.ToString(), cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromForm] SignUpRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
                response = await _accountService.SignUpAsync(request, cancellation);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Something went wrong";
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
