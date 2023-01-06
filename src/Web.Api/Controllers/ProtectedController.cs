using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Web.Api.Controllers;
[Authorize(Policy = "ApiUser")]
[Route("api/[controller]/[action]")]
[ApiController]
public class ProtectedController : ControllerBase
{
    // GET api/protected/home
    [HttpGet("home")]
    public IActionResult Home() => new OkObjectResult(new { result = true });
}