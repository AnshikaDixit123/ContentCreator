using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IContentCreatorDBContext _context;
        public AccountService(IContentCreatorDBContext context)
        {
            _context = context;
        }
        public async Task<ResponseData> AuthenticateAdminAsync(SigningRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData();

            return response;
        }
    }
}
