using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NSoup;
using System.Threading;
using MoreLinq;

namespace IndeedSameJobDifferentPost
{
    class Program
    {
        public static char[] splitChars = new[] { ' ' };
        public static HashSet<char> AcceptableCharacters = "abcdefghijklmnopqrstuvwxyz1234567890".ToHashSet();

        static void Main(string[] args)
        {
            var jobs = GetJobs();
            BuildNGrams(jobs);

            for (int i = 0; i < jobs.Count; i++)
            {
                for (int j = i + 1; j < jobs.Count; j++)
                {
                    var first = jobs[i];
                    var second = jobs[j];

                    var firstGrams = first.FullTextNGrams[9];
                    var secondGrams = second.FullTextNGrams[9];

                    var same = firstGrams.Intersect(secondGrams).ToList();

                    if (same.Count > 15)
                    {
                        Console.WriteLine("Possible match:");
                        Console.WriteLine(first.JobQueryResult.Url);
                        Console.WriteLine(second.JobQueryResult.Url);
                    }
                }
            }
        }

        public static List<FullJob> GetJobs()
        {
            string key = Keys.GetIndeedKey();

            string searchTerms = "scala";
            string location = "Baltimore, MD";
            int radius = 25;

            List<FullJob> jobs;
            var cachedResults = $"{searchTerms}_{location}_{radius}.json";

            if (File.Exists(cachedResults))
            {
                jobs = JsonConvert.DeserializeObject<List<FullJob>>(File.ReadAllText(cachedResults));
            }
            else
            {
                var queryUrl = IndeedQueryUtil.BuildBatchQueryFormatUrl(key, searchTerms, location, radius);
                var jobQueryResults = IndeedQueryUtil.GetAllJobs(queryUrl, key);

                jobs = new List<FullJob>();

                foreach (var job in jobQueryResults)
                {
                    Thread.Sleep(1000);
                    var document = NSoupClient.Connect(job.Url).Timeout(5000).Get();
                    var summaryNodes = document.Select("#job_summary");
                    var fullText = summaryNodes.Text;

                    jobs.Add(new FullJob
                    {
                        JobQueryResult = job,
                        FullText = fullText
                    });
                }

                string json = JsonConvert.SerializeObject(jobs, Formatting.Indented);
                File.WriteAllText(cachedResults, json);
            }

            return jobs;
        }

        public static void BuildNGrams(List<FullJob> fullJobs)
        {
            foreach (var job in fullJobs)
            {
                job.FullTextNGrams = new Dictionary<int, List<string>>();

                var fullTextWords = job.FullText.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                var cleanedWords = StandardizeWords(fullTextWords);

                for (int i = 5; i < 10; i++)
                {
                    var nGramList = new List<string>();
                    var windowsOfNWords = cleanedWords.Window(i);

                    foreach (var window in windowsOfNWords)
                    {
                        var ngram = string.Join(" ", window);
                        nGramList.Add(ngram);
                    }

                    job.FullTextNGrams[i] = nGramList;
                }
            }
        }

        public static List<string> StandardizeWords(IEnumerable<string> words)
        {
            var cleanedWords = new List<string>();
            foreach (var word in words)
            {
                var lowercaseWord = word.ToLower();
                var acceptableChars = lowercaseWord.Where(x => AcceptableCharacters.Contains(x)).ToArray();
                cleanedWords.Add(new string(acceptableChars));
            }
            return cleanedWords;
        }
    }

    class FullJob
    {
        public Job JobQueryResult { get; set; }
        public string FullText { get; set; }
        public Dictionary<int, List<string>> FullTextNGrams { get; set; }
    }
}
