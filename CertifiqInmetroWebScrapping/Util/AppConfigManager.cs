using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace CertifiqInmetroWebScrapping.Util
{
    public class AppConfigManager
    {
        public Configuration config;

        public AppConfigManager(string exepath)
        {
            config = ConfigurationManager
                .OpenExeConfiguration(exepath);
        }


    }
}
