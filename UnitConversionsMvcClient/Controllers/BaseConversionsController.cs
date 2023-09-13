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

namespace MacPConversionsMvcClient.Controllers
{
    [FeatureGate(nameof(FeatureFlags.BaseConversions))]
    public class BaseConversionsController : Controller
    {
        private readonly ILogger<BaseConversionsController> _logger;
        private ErrorViewModel? _errorViewModel;

        public BaseConversionsController(ILogger<BaseConversionsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect("BaseConversions/BaseConversionsPage");
        }

        public async Task<IActionResult> BaseConversionsPage()
        {
            IEnumerable<SelectListItem>? conversionTypeList = new List<SelectListItem>();
            try
            {
                conversionTypeList = await Helpers.GetConversionTypes();
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

        public async Task<JsonResult> ConvertBase(string fromValue, int conversionTypeCode)
        {
            string results = string.Empty;
            try
            {
                BaseConversionData conversionData = new BaseConversionData { ValueFrom = fromValue, ConversionType = (byte)conversionTypeCode };

                using (HttpClient httpClient = new HttpClient())
                {
                    string postContent = JsonConvert.SerializeObject(conversionData);
                    StringContent stringContent = new StringContent(postContent, Encoding.UTF8, "application/json");
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("http://localhost:60937/api/baseconversions/convert", stringContent);
                    results = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                results = Constants.Messages.ERROR_MICROSERVICE_CONNECTION_FAILED;
            }
            catch (Exception ex)
            {
                results = ex.Message;
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
