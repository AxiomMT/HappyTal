using HappyTal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HappyTal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static Dictionary<string, CakeFactory> factoryDictionary = new Dictionary<string, CakeFactory>();
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("sessionId", HttpContext.Session.Id);     // Just a trick to easily make the session work
            return View();
        }

        public IActionResult CakeFactory()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Starts the Factory
        /// </summary>
        /// <returns></returns>
        public IActionResult StartFactory()
        {
            string sessionId = HttpContext.Session.Id;
            if (!factoryDictionary.ContainsKey(sessionId))
            {
                CakeFactory factory = new CakeFactory();
                factoryDictionary.Add(sessionId, factory);
            }

            factoryDictionary[sessionId].Run();
            return new JsonResult(null);
        }

        /// <summary>
        /// Stops the Factory
        /// </summary>
        /// <returns></returns>
        public IActionResult StopFactory()
        {
            string sessionId = HttpContext.Session.Id;
            factoryDictionary[sessionId].Stop();
            return new JsonResult(null);
        }

        /// <summary>
        /// Gets each Factory creation stage data
        /// </summary>
        /// <returns>A JSON serialized object containing the expected data</returns>
        public IActionResult GetFactoryDataMS()
        {
            string sessionId = HttpContext.Session.Id;
            CakeFactory factory = factoryDictionary[sessionId];           
            return Json(factory);
        }
    }
}
