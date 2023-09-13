﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace MacPConversionsMvcClient.Models
{
    public class ConversionTypeInfo
    {
        public byte ConversionType { get; set; }
        public string ConversionName { get; set; }

        public SelectListItem MapTo()
        {
            return new SelectListItem
            {
                Text = ConversionName,
                Value = ConversionType.ToString()
            };
        }
    }
}
