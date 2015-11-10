using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Common
{
    public class Keys
    {
        public static string GetIndeedKey()
        {
            var jObject = JObject.Parse(File.ReadAllText("keys.json"));
            var jToken = jObject.GetValue("IndeedApiKey");
            string key = (string)jToken.ToObject(typeof(string));

            return key;
        }
    }
}
