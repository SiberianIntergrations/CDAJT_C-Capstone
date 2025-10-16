using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Testing
{
    [ApiController]
    [Route("api/testing")]
    public class TestingController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public TestingController(IWebHostEnvironment env) => _env = env;

        [HttpGet("check-folder")]
        public async Task<IActionResult> CheckAndCreateFolder()
        {
            // var cwDir = _env.ContentRootPath; // project root
            // string findFolder = Path.Combine(cwDir, "MenuPhotos");
            // if (!Path.Exists(findFolder))
            // {
            //     Directory.CreateDirectory(findFolder);
            //     return Ok("Folder Should be Created");
            // }

            string backendRoot = _env.ContentRootPath;
            string frontEndURLStorage = Path.GetFullPath(Path.Combine(backendRoot, "..", "front-end/publics/menu-items"));
            if (!Path.Exists(frontEndURLStorage))
            {
                Directory.CreateDirectory(frontEndURLStorage);
            }

            
            return Ok($"Backend: {backendRoot} FrontEnd: {frontEndURLStorage.Replace("\\","/")}");

        }
    }
}