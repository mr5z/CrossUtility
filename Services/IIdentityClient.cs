using CrossUtility.Services.Identity;
using CrossUtility.Services.Identity.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace CrossUtility.Services
{
    public interface IIdentityClient
    {
        Task<AuthorizationResponse> Authorize(AuthorizationRequest request);
        Task<TokenResponse> RequestToken(TokenRequest request, CancellationToken cancellationToken = default);
    }
}
