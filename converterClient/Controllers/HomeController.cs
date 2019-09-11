using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using converterClient.Models;

namespace converterClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Converter()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetConvertedData(string userInput)
        {
            string result = null;

            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(userInput))
                {
                    var response = client.PostAsJsonAsync(new Uri("https://localhost:54610/convert/start"), userInput)
                        .Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;

                        result = responseString;
                    }
                }
            }

            return new OkObjectResult(result);
    }



    //public IActionResult Index()
    //{
    //    return View();
    //}

    //public IActionResult About()
    //{
    //    ViewData["Message"] = "Your application description page.";

    //    return View();
    //}

    //public IActionResult Contact()
    //{
    //    ViewData["Message"] = "Your contact page.";

    //    return View();
    //}

    //public IActionResult Privacy()
    //{
    //    return View();
    //}

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}
}
}
