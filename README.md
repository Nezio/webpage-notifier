# webpage-notifier
Notifies if any of the specified keywords is found on any of the specified urls
The app expects settings.json file in "%localappdata%/Webpage notifier".
Upon running the app for the first time settings_example.json file will be generated in the same folder.

You can add multiple webpage search jobs.
Each job contains a list of URLs and keywords.
Each keyword will be searched for in each of the URLs within a job. In other words: the app will notify you if any keyword is found in any of the URLs defined for the job.
After this, the next job will be processed.