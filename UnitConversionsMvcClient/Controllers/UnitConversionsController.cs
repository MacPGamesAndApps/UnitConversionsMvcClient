using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnitConversionsMvcClient.Models;

namespace UnitConversionsMvcClient.Controllers
{
    public class UnitConversionsController : Controller
    {
        private readonly ILogger<UnitConversionsController> _logger;
        private ErrorViewModel? _errorViewModel;

        public UnitConversionsController(ILogger<UnitConversionsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect("UnitConversions/UnitConversionsPage");
        }

        public async Task<IActionResult> UnitConversionsPage()
        {
            IEnumerable<SelectListItem>? conversionTypeList = new List<SelectListItem>();
            try
            {
                string conversionTypeListJason = string.Empty;

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("http://localhost:60937/api/unitconversions/gettypes");
                    conversionTypeListJason = await httpResponseMessage.Content.ReadAsStringAsync();
                }

                conversionTypeList = JsonConvert.DeserializeObject<List<ConversionTypeInfo>>(conversionTypeListJason).Select(ctl => ctl.MapTo());
            }
            catch (HttpRequestException ex)
            {
                _errorViewModel = new ErrorViewModel { ErrorMessage = "Failed to connect to Microservice. The Microservice is not responding. Please, make sure it is running and working properly." };
                return Redirect("Error");
            }
            catch (Exception ex)
            {
                _errorViewModel = new ErrorViewModel { ErrorMessage = ex.Message };
                return Redirect("Error");
            }
            return View(conversionTypeList);
        }

        //to-do: capture form values and replace hardcoded test data 
        public async Task<JsonResult> Convert(double fromValue, int conversionTypeCode)
        {
            string results = string.Empty;
            try
            {
                double? convertedValue = null;
                ConversionData conversionData = new ConversionData { ValueFrom = fromValue, ConversionType = (byte)conversionTypeCode };

                using (HttpClient httpClient = new HttpClient())
                {
                    string postContent = JsonConvert.SerializeObject(conversionData);
                    StringContent stringContent = new StringContent(postContent, Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("http://localhost:60937/api/unitconversions/convert", stringContent);
                    double responseValue = 0;
                    if (Double.TryParse(await httpResponseMessage.Content.ReadAsStringAsync(), out responseValue))
                    {
                        convertedValue = responseValue;
                    }
                }
                results = convertedValue.ToString();
            }
            catch (HttpRequestException ex)
            {
                results = "Failed to connect to Microservice. The Microservice is not responding. Please, make sure it is running and working properly.";
            }
            catch (Exception ex)
            {
                results =  ex.Message;
            }
            return Json(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            if(_errorViewModel == null )
            {
                _errorViewModel= new ErrorViewModel { ErrorMessage = "Undetermined Error." };
            }
            return View(_errorViewModel);
        }
    }
}
