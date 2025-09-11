using Azure.Core;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IGeneralService _generalService;

        public GeneralController(IGeneralService generalService)
        {
            _generalService = generalService;
        }

        [HttpGet("GetUserList")]
        public async Task<IActionResult> GetUserList(bool? IncludeSuperAdmin, CancellationToken cancellation)
        {
            var response = new ResponseData<List<UserDetailsResponse>>();
            try
            {
                response = await _generalService.GetUserListAsync(cancellation);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetRoleList")]
        public async Task<IActionResult> GetRoleList(bool? IncludeSuperAdmin, CancellationToken cancellation)
        {
            var response = new ResponseData<List<RolesResponseModel>>();
            try
            {
                response = await _generalService.GetRoleListAsync(IncludeSuperAdmin, cancellation);
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
