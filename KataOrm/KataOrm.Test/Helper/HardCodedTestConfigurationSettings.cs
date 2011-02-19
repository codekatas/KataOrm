using System;
using KataOrm.Configuration;

namespace KataOrm.Test.Helper
{
    public class HardCodedTestConfigurationSettings : IConfigurationSettings
    {
        public string ConnectionSettings
        {
            get { return "Data Source=WIN7-64-V8; Trusted_Connection=true; Initial Catalog=KataOrm"; }
        }
    }
}