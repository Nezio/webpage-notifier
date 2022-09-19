using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;
using System.IO;

namespace Webpage_notifier
{
    class Program
    {
        static string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string localWebpageNotifierPath = localPath + "\\Webpage notifier";
        static string settingsJsonPath = localWebpageNotifierPath + "\\Settings.json";
        static string exampleSettingsJsonPath = localWebpageNotifierPath + "\\Settings_Example.json";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(localWebpageNotifierPath);

            WebpageSearch webpageSearch;

            if (File.Exists(settingsJsonPath))
            {
                string jsonSettingsText = File.ReadAllText(settingsJsonPath);

                try
                {
                    webpageSearch = JsonSerializer.Deserialize<WebpageSearch>(jsonSettingsText);
                }
                catch (Exception ex)
                {
                    ShowMessage("Reading JSON settings failed with error: '" + ex.InnerException + "'. Please check if the settings file is formatted correctly.", MessageBoxIcon.Error);

                    return;
                }

            }
            else
            {
                ShowMessage("Settings Json file not found: '" + settingsJsonPath + "'. Please, configure it and try again. See the example settings file in: '" + exampleSettingsJsonPath + "'.", MessageBoxIcon.Warning);

                if (!File.Exists(exampleSettingsJsonPath))
                {
                    GenerateExampleJson();
                }

                return;
            }

            try
            {
                ProcessWebpageSearch(webpageSearch);
            }
            catch (Exception ex)
            {
                ShowMessage("Processing webpage search failed with error: '" + ex.InnerException + "'.", MessageBoxIcon.Error);

                return;
            }           

        }

        public static void ProcessWebpageSearch(WebpageSearch webpageSearch)
        {
            foreach (WebpageSearchJob webpageSearchJob in webpageSearch.WebpageSearchJobs)
            {
                foreach (string url in webpageSearchJob.urls)
                {

                    // get contents for the web page
                    string pageHtml = GetHtml(url);
                    if (pageHtml == null)
                    {
                        ShowMessage("Couldn't get page body from url: " + url);

                        continue;
                    }

                    // search for specified keywords
                    foreach(string keyword in webpageSearchJob.keywords)
                    {
                        if (pageHtml.Contains(keyword))
                        {
                            // notify if found
                            ShowMessage("Keyword: '" + keyword + "' found in url: '" + url + "'.");
                        }
                    }

                    
                }
            }

        }

        private static string GetHtml(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            using (HttpContent content = response.Content)
                            {
                                string result = content.ReadAsStringAsync().Result;

                                return result;
                            }
                        }
                        else
                        {
                            throw new Exception("Webpage '" + url + "' returned a non-OK status code.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Getting HTML for url '" + url + "' failed with error: '" + ex.InnerException + "'.", MessageBoxIcon.Error);

                return null;
            }
        }

        /// <summary>
        /// Display a message in a messagebox.
        /// </summary>
        /// <param name="message">The message text.</param>
        private static void ShowMessage(string message, MessageBoxIcon messageType = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, "Webpage notifier", MessageBoxButtons.OK, messageType);
        }

        private static void GenerateExampleJson()
        {

            var webpageSearchJobExample1 = new WebpageSearchJob
            {
                keywords = new List<string> { "keyword1-job1", "keyword2-job1" },
                urls = new List<string> { "url1-job1", "url2-job1" }
            };
            var webpageSearchJobExample2 = new WebpageSearchJob
            {
                keywords = new List<string> { "keyword1-job2", "keyword2-job2" },
                urls = new List<string> { "url1-job2", "url2-job2" }
            };

            var webpageSearchExample = new WebpageSearch
            {
                WebpageSearchJobs = new List<WebpageSearchJob> { webpageSearchJobExample1, webpageSearchJobExample2 }
            };

            string exampleJsonString = JsonSerializer.Serialize(webpageSearchExample);

            File.WriteAllText(exampleSettingsJsonPath, exampleJsonString);
        }

        public class WebpageSearch
        {
            public List<WebpageSearchJob> WebpageSearchJobs { get; set; }

        }
        public class WebpageSearchJob
        {
            public List<string> keywords { get; set; }
            public List<string> urls { get; set; }
        }

    }
}
