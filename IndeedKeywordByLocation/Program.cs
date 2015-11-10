using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var queryUrl = IndeedQueryUtil.BuildBatchQueryFormatUrl(key, searchTerms);
            List<Job> jobs = IndeedQueryUtil.GetAllJobs(queryUrl, key);

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
    }
}
