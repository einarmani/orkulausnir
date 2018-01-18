using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orkulausnir.DataAccess;

namespace Orkulausnir.Controllers
{
    public class UploadController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Vinsamlegast veldu skjal til að hlaða upp";
            }
            else
            {
                var isSuccessful = await AzureDataAccess.UploadFile(file);
                if (isSuccessful)
                {
                    ViewBag.Message = "Skjali hlaðið upp";
                }
                else
                {
                    ViewBag.Message = "Skjali EKKI hlaðið upp þar sem til er skjal með sama nafni";
                }
            }

            return View("Index");
        }
    }
}