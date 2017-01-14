This project contains two console apps that query indeed.com in ways 
not possible through the website.

**IndeedKeywordTrends** shows the number of jobs for a set of keywords 
across multiple major cities for different radii. In other words, 
it can show the concentration of jobs for keywords (e.g. scala, 
python, angular) by city. It appends all results to the same csv 
file, so if it is run over time it is possible to see trends for 
those keywords over time.

I wrote a 
**[blog post on using this project to examine trends for
specific tech keywords between 7/2015 and 7/2016
](http://blog.briandrupieski.com/indeed-tech-job-trends)**.

**IndeedKeywordByLocation** shows the concentration of job 
postings by keyword by city. It queries every single job for the 
specified keyword and then groups them by city so that it is 
possible to see which city has the highest number of jobs with 
that keyword. It is more useful for keywords which do not return 
very many jobs (e.g. TypeScript, WebGL).

All projects retrieve the Indeed API key from a git-ignored 
"keys.json" file that is copied from the solution directory to the 
target directory by a post-build action. It looks like the 
following snippet:

```json
{
  "IndeedApiKey": 1234567890123456
}
```