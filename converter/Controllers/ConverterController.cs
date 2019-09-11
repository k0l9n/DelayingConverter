using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using converter.Converter;
using Microsoft.AspNetCore.Mvc;

namespace converter.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static readonly DelayingConverter DelayingConverter = new DelayingConverter();

        public ValuesController()
        {
        }

        // GET api/values/"data"
        [HttpGet("{data}")]
        public string Get(string userData)
        {
            string result = String.Empty;

            if (userData != null)
            {
                result = DelayingConverter.Convert(userData);
            }

            return result;
        }

    }
}
