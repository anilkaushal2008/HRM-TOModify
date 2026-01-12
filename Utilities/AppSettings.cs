using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HRM.Utilities
{
    public static class AppSettings
    {
        public static string SMSApiUrl => ConfigurationManager.AppSettings["SMSApiUrl"];
        public static string SMSApiKey => ConfigurationManager.AppSettings["SMSApiKey"];
        public static string SMSSenderName => ConfigurationManager.AppSettings["SMSSenderName"];
    }
}