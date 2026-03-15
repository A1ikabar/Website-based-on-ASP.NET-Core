using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryWebApp.Models;

namespace LibraryWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}