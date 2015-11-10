using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Common
{
    public static class IndeedQueryUtil
    {
        public static string BuildBatchQueryFormatUrl(string apiKey, string searchTerms, string location = null, int? radius = null)
        {
            var encodedSearchTerms = Uri.EscapeDataString(searchTerms);
            
            string indeedUrlFormat = "http://api.indeed.com/ads/apisearch?publisher=" + apiKey + "&" +
                                     "start={0}&" +
                                     "limit={1}&" +
                                     "v=2&" +
                                     "format=json&" +
                                     $"q={encodedSearchTerms}&";

            if (!string.IsNullOrEmpty(location))
            {
                var encodedLocation = Uri.EscapeDataString(location);

                indeedUrlFormat += $"l={encodedLocation}&";
            }

            if (radius != null)
            {
                indeedUrlFormat += $"radius={radius}&";
            }

            return indeedUrlFormat;
        }

        public static List<Job> GetAllJobs(string queryUrl, string indeedApiKey)
        {
            const int pageSize = 25;

            var jobs = new List<Job>();

            using (var wc = new WebClient())
            {
                for (int page = 0; ; page++)
                {
                    int pageStart = page * pageSize;
                    string apiQuery = string.Format(queryUrl, pageStart, pageSize);
                    var json = wc.DownloadString(apiQuery);
                    Console.WriteLine(json);

                    var queryResult = JsonConvert.DeserializeObject<IndeedQueryResult>(json);

                    jobs.AddRange(queryResult.Results);

                    if (jobs.Count >= queryResult.TotalResults)
                    {
                        break;
                    }

                    Thread.Sleep(250);
                }
            }

            return jobs;
        }
    }
}
