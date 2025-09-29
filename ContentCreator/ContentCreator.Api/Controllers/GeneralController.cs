using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Api.Controllers
{
    //[Authorize]
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
        [HttpGet("GetFileTypeList")]
        public async Task<IActionResult> GetFileTypeList(CancellationToken cancellation)
        {
            var response = new ResponseData<FileTypeComboResponseModel>();
            try
            {
                response = await _generalService.GetFileTypeListAsync(cancellation);
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("AddExtension")]
        public async Task<IActionResult> AddExtension([FromForm] AddExtensionsRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
                response = await _generalService.AddExtensionAsync(request, cancellation);
            }
            catch(Exception ex)
            { 
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
