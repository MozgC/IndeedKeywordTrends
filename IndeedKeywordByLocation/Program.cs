using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Common;

namespace IndeedKeywordByLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = Keys.GetIndeedKey();

            var outLines = new List<string>();

            const string searchTerms = "webgl";
            string outFile = string.Format("'{0}' by location.csv", searchTerms);
            File.WriteAllLines(outFile, new[] { "Query,Date,City,State,Source,Sponsored,Expired,FormattedRelativeTime" });

            List<Job> jobs = GetAllJobs(searchTerms, key);

            outLines.AddRange(jobs.Select(job => string.Format("{0},\"{1}\",{2},{3},\"{4}\",{5},{6},{7},\"{8}\"",
                                                               searchTerms, job.Date, job.City, job.State, job.Source,
                                                               job.Sponsored, job.Expired, job.FormattedRelativeTime,
                                                               job.Snippet)));
            File.AppendAllLines(outFile, outLines);

            var cityGroups = jobs.GroupBy(x => x.City).OrderByDescending(x => x.Count());
            var groupOutLines =
                cityGroups.Select(cityGroup => string.Format("{0} jobs in {1}", cityGroup.Count(), cityGroup.Key))
                          .ToList();
            File.AppendAllLines(outFile, groupOutLines);
        }

        public static List<Job> GetAllJobs(string searchTerms, string indeedApiKey)
        {
            const int pageSize = 25;

            string indeedUrlFormat = "http://api.indeed.com/ads/apisearch?publisher=" + indeedApiKey + "&" +
                                     "start={0}&" +
                                     "limit={1}&" +
                                     "v=2&" +
                                     "format=json&" +
                                     "q={2}&";

            var jobs = new List<Job>();

            using (var wc = new WebClient())
            {
                for (int page = 0;; page++)
                {
                    int pageStart = page * pageSize;
                    string apiQuery = string.Format(indeedUrlFormat, pageStart, pageSize, Uri.EscapeDataString(searchTerms));
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

    public class IndeedQueryResult
    {
        public string Query { get; set; }
        public string Location { get; set; }
        public string Radius { get; set; }
        public int TotalResults { get; set; }
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

        public override string ToString()
        {
            return string.Join(" - ", new[] { JobTitle, Company });
        }
    }
}
