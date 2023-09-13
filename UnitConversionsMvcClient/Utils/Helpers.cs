﻿using MacPConversionsMvcClient.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MacPConversionsMvcClient.Utils
{
    public static class Helpers
    {
        public static async Task<IEnumerable<SelectListItem>?> GetConversionTypes()
        {
            IEnumerable<SelectListItem>? conversionTypeList = new List<SelectListItem>();

            string conversionTypeListJason = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("http://localhost:60937/api/unitconversions/gettypes");
                conversionTypeListJason = await httpResponseMessage.Content.ReadAsStringAsync();
            }

            conversionTypeList = JsonConvert.DeserializeObject<List<ConversionTypeInfo>>(conversionTypeListJason).Select(ctl => ctl.MapTo());

            return conversionTypeList;
        }
    }
}