using Credenciamento.Application.Interfaces.Global;

namespace Credenciamento.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cache;
    public CacheController(ICacheService cache)
    {
        _cache = cache;
    }

    [HttpGet]
    public IActionResult List()
    {
        var result = _cache.GetCacheSnapshot();
        return Ok(result);
    }
}
