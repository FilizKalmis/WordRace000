using Microsoft.AspNetCore.Mvc;
using WordRace000.Models;

namespace WordRace000.Controllers
{
    public class WordChainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Play()
        {
            return View();
        }
    }
} 