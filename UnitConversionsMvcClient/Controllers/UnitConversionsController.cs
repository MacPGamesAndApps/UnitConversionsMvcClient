using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MacPConversionsMvcClient.Models;
using MacPConversionsMvcClient.Utils;
using Microsoft.FeatureManagement.Mvc;

namespace UnitConversionsMvcClient.Controllers
{
    [FeatureGate(nameof(FeatureFlags.UnitConversions))]
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
                conversionTypeList = await Helpers.GetConversionTypes(FeatureFlags.UnitConversions);
            }
            catch (HttpRequestException ex)
            {
                _errorViewModel = new ErrorViewModel { ErrorMessage = Constants.Messages.ERROR_MICROSERVICE_CONNECTION_FAILED };
                return Redirect("Error");
            }
            catch (Exception ex)
            {
                _errorViewModel = new ErrorViewModel { ErrorMessage = ex.Message };
                return Redirect("Error");
            }
            return View(conversionTypeList);
        }

        public async Task<JsonResult> ConvertUnit(double fromValue, int conversionTypeCode)
        {
            string? results = string.Empty;
            try
            {
                double? convertedValue = null;
                UnitConversionData conversionData = new UnitConversionData { ValueFrom = fromValue, ConversionType = (byte)conversionTypeCode };

                using (HttpClient httpClient = new HttpClient())
                {
                    string postContent = JsonConvert.SerializeObject(conversionData);
                    StringContent stringContent = new StringContent(postContent, Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(Helpers.GetEndpointRootUrl(FeatureFlags.UnitConversions) + "convert", stringContent);
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
                results = Constants.Messages.ERROR_MICROSERVICE_CONNECTION_FAILED;
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
