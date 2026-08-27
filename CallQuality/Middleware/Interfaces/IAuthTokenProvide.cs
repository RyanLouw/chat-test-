namespace CallQuality.Middleware.Interfaces;

public interface IAuthTokenProvide
{
    Task<string> GetTokenAsync();
}
