This project contains two console apps to query indeed.com in ways not possible through the website.

IndeedKeywordTrends shows the number of jobs for a set of keywords across multiple major cities for different radii. In other words, it can show the concentration of jobs for keywords (e.g. scala, python, angular) by city. It appends all results to the same csv file, so if it is run over time it is possible to see trends for those keywords over time.

IndeedKeywordByLocation shows the concentration of jobs by keyword by city. It queries every single job for the specified keyword and then groups them by city so that it is possible to see which city has the highest number of jobs with that keyword. It is more useful for keywords which do not return very many jobs (e.g. typescript, webgl).