using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("")]
[Route("Home")]
public class HomeController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IGrantService grantService, ILogger<HomeController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        // Public-facing homepage - no authentication or data required
        return View();
    }

    [Route("Error")]
    public IActionResult Error()
    {
        return View();
    }
}
