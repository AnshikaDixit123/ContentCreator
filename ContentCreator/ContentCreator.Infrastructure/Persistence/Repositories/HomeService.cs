using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class HomeService : IHomeService
    {
        private readonly IContentCreatorDBContext _context;
        public HomeService(IContentCreatorDBContext context)
        {
            _context = context;
        }
        public async Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();



            return response;
        }
    }
}
