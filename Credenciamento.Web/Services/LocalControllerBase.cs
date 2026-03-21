using Credenciamento.Application.Interfaces.Global;
using Credenciamento.Application.Models;
using Credenciamento.Shared.Helpers;
using Credenciamento.Web.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Credenciamento.Web.Services
{
    public class LocalControllerBase : Controller
    {
        private readonly ICacheService _cache;
        public LocalControllerBase(IServiceProvider services)
        {
            _cache = services.GetRequiredService<ICacheService>();
        }

        protected UserModel GetUserFromToken()
        {
            if (Request.Cookies.TryGetValue("user-token", out string? token))
            {
                var cacheKey = $"user:{StringHelpers.FromBase64(token)}";
                if (_cache.HasKey(cacheKey))
                {
                    return _cache.GetObject<UserModel>(cacheKey);
                }
            }

            return null;
        }

        protected bool Logoff()
        {
            if (Request.Cookies.TryGetValue("user-token", out string? token))
            {
                var cacheKey = $"user:{StringHelpers.FromBase64(token)}";
                if (_cache.HasKey(cacheKey))
                    _cache.RemoveKey(cacheKey);

                return true;
            }

            return false;
        }

        public LocalBaseViewModel GetLocalBaseViewModel()
        {
            return new LocalBaseViewModel
            {
                User = GetUserFromToken()
            };
        }
    }
}
