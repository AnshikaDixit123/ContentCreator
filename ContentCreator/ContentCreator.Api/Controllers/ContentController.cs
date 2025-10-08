using ContentCreator.Api.FileModels;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


namespace ContentCreator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly IWebHostEnvironment _env;
        public ContentController(IContentService contentService, IWebHostEnvironment env)
        {
            _contentService = contentService;
            _env = env;
        }
        [HttpPost("UploadAPost")]
        public async Task<IActionResult> UploadAPost([FromForm] UploadAPostFileRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "ContentCreator", request.UserId.ToString());
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream, cancellation);
                }
                var relativePath = Path.Combine("ContentCreator", request.UserId.ToString(), fileName)
                       .Replace("\\", "/");
                var serviceRequest = new UploadAPostRequest
                {
                    UserId = request.UserId,
                    PostDescription = request.PostDescription,
                    FileName = fileName,
                    FilePath = relativePath,
                };
                response = await _contentService.UploadAPostAsync(serviceRequest, cancellation);
            }
            catch (Exception ex) 
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
