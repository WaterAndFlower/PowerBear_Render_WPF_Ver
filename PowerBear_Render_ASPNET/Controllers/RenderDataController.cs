using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PowerBear_Render_ASPNET.DAO;

namespace PowerBear_Render_ASPNET.Controllers {
    [ApiController]
    [Route("/[action]")]
    public class RenderDataController : ControllerBase {
        [HttpPost]
        public ActionResult SendRenderData(NetworkJsonData jsonData) {
            GobalManager.networkJsonData = jsonData;
            return Ok();
        }
        [HttpPost]
        [HttpGet]
        public NetworkJsonData GetRenderData() {
            return GobalManager.networkJsonData;
        }
    }
}
