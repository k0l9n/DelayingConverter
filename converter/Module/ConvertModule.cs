using System;
using converter.Converter;
using Nancy;
using Nancy.Extensions;

namespace converterApi.Module
{

    public class ConvertModule : NancyModule
    {
        private static readonly DelayingConverter DelayingConverter = new DelayingConverter();

        public ConvertModule()
        {
            Post("/start", async (parameters, _) =>
            {
                
                string userData = Request.Body.AsString();
                string result = String.Empty;

                if (userData != null)
                {
                    result = DelayingConverter.Convert(userData);
                }

                return result;
            });
        }
    }
}
