namespace ContentCreator.Application.Interfaces
{
    public interface ITokenRevocationConfig
    {
        Task<bool> RevokeToken(string token);
        Task<bool> IsTokenRevoked(string token);
    }
}
