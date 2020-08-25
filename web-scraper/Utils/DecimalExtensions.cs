using System;
using System.Collections.Generic;
using System.Text;

namespace web_scraper.Utils
{
    public static class DecimalExtensions
    {
        public static decimal? ParseNullableInt(this string value)
        {
            if (value == null || value.Trim() == string.Empty)
            {
                return null;
            }
            else
            {
                try
                {
                    return decimal.Parse(value);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
