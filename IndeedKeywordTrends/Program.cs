using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Common;

namespace IndeedKeywordTrends
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = Keys.GetIndeedKey();

            var cities = File.ReadAllLines("cities.txt");
            var keywords = File.ReadAllLines("keywords.txt");
            var radii = new[] { 25, 50 };

            string indeedUrlFormat = "http://api.indeed.com/ads/apisearch?publisher=" + key + "&" +
                                     "limit=0&" +
                                     "v=2&" +
                                     "format=json&" +
                                     "radius={0}&" +  // radius
                                     "q={1}&" +       // query
                                     "l={2}";         // location

            string outFile = "results.csv";
            if (args.Length >= 1)
            {
                outFile = args[0];
            }
            if (!File.Exists(outFile))
            {
                File.WriteAllLines(outFile, new[] { "Date,Radius,Location,Query,TotalResults" });
            }

            var outLines = new List<string>();

            using (var wc = new WebClient())
            {
                foreach (var city in cities)
                {
                    foreach (var keyword in keywords)
                    {
                        foreach (var radius in radii)
                        {
                            var query = string.Format(indeedUrlFormat, radius, Uri.EscapeDataString(keyword), Uri.EscapeDataString(city));
                            var json = wc.DownloadString(query);
                            var result = JsonConvert.DeserializeObject<IndeedQueryResult>(json);

                            var outLine = string.Format("{0},{1},\"{2}\",{3},{4}", DateTime.Now.ToShortDateString(),
                                                        result.Radius, result.Location, result.Query, result.TotalResults);
                            outLines.Add(outLine);
                            Console.WriteLine("did {0}", outLine);
                            Thread.Sleep(250);
                        }
                    }
                }
            }

            File.AppendAllLines(outFile, outLines);
        }
    }

    public class IndeedQueryResult
    {
        public string Query { get; set; }
        public string Location { get; set; }
        public string Radius { get; set; }
        public string TotalResults { get; set; }
    }
}
