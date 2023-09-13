using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacPConversionsMvcClient.Utils
{
    public class MacPDisabledFeaturesHandler : IDisabledFeaturesHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
        {
            context.Result = new ContentResult { 
                ContentType = "text/html",
                Content ="<h1>Feature not yet implemented / currently disabled.</h1><button onClick='history.back()'>Go Back</button>",
                StatusCode = 404
            };
            return Task.CompletedTask;
        }
    }
}
