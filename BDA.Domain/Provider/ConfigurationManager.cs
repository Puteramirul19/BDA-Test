using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using BDA.Interface;

namespace BDA.Provider
{
    public class ConfigurationManager : IConfigurationManager
    {
        public string ApplicationConnection()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        }
    }
}
