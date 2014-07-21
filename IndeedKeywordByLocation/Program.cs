using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace IndeedKeywordByLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            var outLines = new List<string>();

            const string searchTerms = "webgl";
            string outFile = string.Format("'{0}' by location.csv", searchTerms);
            File.WriteAllLines(outFile, new[] { "Query,Date,City,State,Source,Sponsored,Expired,FormattedRelativeTime" });

            var allResults = GetAllPagedResults(searchTerms);

            outLines.AddRange(
                allResults.Results.Select(job => string.Format("{0},\"{1}\",{2},{3},\"{4}\",{5},{6},{7},\"{8}\"",
                                                               searchTerms, job.Date, job.City, job.State, job.Source,
                                                               job.Sponsored, job.Expired, job.FormattedRelativeTime,
                                                               job.Snippet)));
            File.AppendAllLines(outFile, outLines);

            var cityGroups = allResults.Results.GroupBy(x => x.City).OrderByDescending(x => x.Count());
            var groupOutLines =
                cityGroups.Select(cityGroup => string.Format("{0} jobs in {1}", cityGroup.Count(), cityGroup.Key))
                          .ToList();
            File.AppendAllLines(outFile, groupOutLines);
        }

        public static IndeedQueryResult GetAllPagedResults(string searchTerms)
        {
            int start = 0;
            const int pageSize = 25;
            int page = 0;

            const string indeedUrlFormat = "http://api.indeed.com/ads/apisearch?publisher=your_api_key_here&" +
                                           "start={0}&" +
                                           "limit={1}&" +
                                           "v=2&" +
                                           "format=json&" +
                                           "q={2}&";

            IndeedQueryResult totalResult = null;

            int lastNumOfRetrievedResults = 0;
            using (var wc = new WebClient())
            {
                while (totalResult == null || lastNumOfRetrievedResults == pageSize)
                {
                    string apiQuery = string.Format(indeedUrlFormat, start, pageSize, Uri.EscapeDataString(searchTerms));
                    var json = wc.DownloadString(apiQuery);
                    Console.WriteLine(json);

                    var queryResult = JsonConvert.DeserializeObject<IndeedQueryResult>(json);

                    if (totalResult == null)
                    {
                        totalResult = queryResult;
                    }
                    else
                    {
                        totalResult.Results.AddRange(queryResult.Results);
                    }

                    page++;
                    start = page * pageSize;
                    lastNumOfRetrievedResults = queryResult.Results.Count;
                    Thread.Sleep(250);
                }
            }

            return totalResult;
        }
    }

    public class IndeedQueryResult
    {
        public string Query { get; set; }
        public string Location { get; set; }
        public string Radius { get; set; }
        public string TotalResults { get; set; }
        public List<Job> Results { get; set; }

        public int Start { get; set; }
        public int End { get; set; }
        public int PageNumber { get; set; }
    }

    public class Job
    {
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FormattedLocation { get; set; }
        public string Source { get; set; }
        public string Date { get; set; }
        public string Snippet { get; set; }
        public string Url { get; set; }
        public string JobKey { get; set; }
        public bool Sponsored { get; set; }
        public bool Expired { get; set; }
        public string FormattedLocationFull { get; set; }
        public string FormattedRelativeTime { get; set; }
    }
}
