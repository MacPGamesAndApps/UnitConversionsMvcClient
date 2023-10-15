using MacPConversionsMvcClient.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace MacPConversionsMvcClient.Utils
{
    public static class Helpers
    {
        public static async Task<IEnumerable<SelectListItem>?> GetConversionTypes(FeatureFlags feature)
        {
            IEnumerable<SelectListItem>? conversionTypeList = new List<SelectListItem>();

            string conversionTypeListJason = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(GetEndpointRootUrl(feature) + "gettypes");
                conversionTypeListJason = await httpResponseMessage.Content.ReadAsStringAsync();
            }

            conversionTypeList = JsonConvert.DeserializeObject<List<ConversionTypeInfo>>(conversionTypeListJason).Select(ctl => ctl.MapTo());

            return conversionTypeList;
        }

        public static string GetEndpointRootUrl(FeatureFlags feature)
        {
            string endpointRootUrl = string.Empty;
            switch (feature)
            {
                case FeatureFlags.UnitConversions:
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
                    {
                        endpointRootUrl = Constants.ConversionServiceEndPoints.PROD_UNIT_CONVERSION_ENDPOINT;
                    }
                    else
                    {
                        endpointRootUrl = Constants.ConversionServiceEndPoints.DEV_UNIT_CONVERSION_ENDPOINT;
                    }
                    break;
                case FeatureFlags.BaseConversions:
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
                    {
                        endpointRootUrl = Constants.ConversionServiceEndPoints.PROD_BASE_CONVERSION_ENDPOINT;
                    }
                    else
                    {
                        endpointRootUrl = Constants.ConversionServiceEndPoints.DEV_BASE_CONVERSION_ENDPOINT;
                    }
                    break;
                default:
                    break;
            }
            return endpointRootUrl;
        }
    }
}
