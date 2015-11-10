namespace Common
{
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
