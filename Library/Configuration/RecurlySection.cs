using System;
using System.Configuration;

namespace Recurly.Configuration
{
    /// <summary>
    /// Defines username, password, and subdomain properties for web.config/app.config files.
    /// </summary>
    public class RecurlySection : ConfigurationSection
    {
        public static RecurlySection Current
        {
            get { return (RecurlySection)ConfigurationManager.GetSection("recurly"); }
        }

        #region Properties
        
        /// <summary>
        /// API Key
        /// </summary>
        [ConfigurationProperty("apiKey", IsRequired = true)]
        //[StringValidator(MinLength=32, MaxLength=32)]
        public string ApiKey
        {
            get { return (string)base["apiKey"]; }
            set { base["apiKey"] = value; }
        }

        /// <summary>
        /// API Private Key for Recurly.js and Transparent Post API
        /// </summary>
        [ConfigurationProperty("privateKey", IsRequired=false)]
        //[StringValidator(MinLength=32, MaxLength=32)]
        public string PrivateKey
        {
            get { return (string)base["privateKey"]; }
            set { base["privateKey"] = value; }
        }

        /// <summary>
        /// Recurly Subdomain
        /// </summary>
        [ConfigurationProperty("subdomain", IsRequired=true)]
        public string Subdomain
        {
            get { return (string)base["subdomain"]; }
            set { base["subdomain"] = value; }
        }

        [ConfigurationProperty("currency", DefaultValue = "USD", IsRequired = false)]
        public string Currency
        {
            get { return (string) base["currency"]; }
            set { base["currency"] = value; }
        }

        #endregion
    }
}
