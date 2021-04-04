using Microsoft.AspNetCore.Mvc;

namespace oLeiteService.Controllers
{
    [Route("signin-google")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // GET: signin-google
        [HttpGet]
        public ActionResult Get()
        {
            return Redirect("/api/animais");
        }
    }
}
