using Microsoft.AspNetCore.Mvc;

namespace HodoCloudAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase
    {
        [HttpPost, DisableRequestSizeLimit]
        public void PostUserFile()
        {
            var file = Request.Form.Files[0];
        }
    }
}
