using System.Collections.Generic;

namespace Common
{
    public class IndeedQueryResult
    {
        public string Query { get; set; }
        public string Location { get; set; }
        public int Radius { get; set; }
        public int TotalResults { get; set; }
        public List<Job> Results { get; set; }

        public int Start { get; set; }
        public int End { get; set; }
        public int PageNumber { get; set; }
    }
}
