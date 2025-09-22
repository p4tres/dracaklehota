using Microsoft.AspNetCore.Mvc;

namespace DrDWebAPP.Areas.Mirror.Controllers
{
    [Area("Mirror")]
    public class MirrorController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Popup() => View();
    }
}
