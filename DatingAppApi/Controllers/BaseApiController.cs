using DatingAppApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
