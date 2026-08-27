using CallQuality.Middleware.Interfaces;
using System.Net.Http.Headers;

namespace CallQuality.Middleware;

public sealed class CallQualityBearerTokenHandler : DelegatingHandler
{
    private readonly IAuthTokenProvide _tokenService;

    public CallQualityBearerTokenHandler(IAuthTokenProvide tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await _tokenService.GetTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}