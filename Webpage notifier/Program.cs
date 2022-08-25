using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;


namespace Webpage_notifier
{
    class Program
    {
        static void Main(string[] args)
        {


            var webpageSearchJob1 = new WebpageSearchJob
            {
                keywords = new List<string> { "keyword1-job1", "keyword2-job1" },
                urls = new List<string> { "url1-job1", "url2-job1" }
            };
            var webpageSearchJob2 = new WebpageSearchJob
            {
                keywords = new List<string> { "keyword1-job2", "keyword2-job2" },
                urls = new List<string> { "url1-job2", "url2-job2" }
            };

            var webpageSearch1 = new WebpageSearch
            {
                WebpageSearchJobs = new List<WebpageSearchJob> { webpageSearchJob1, webpageSearchJob2 }
            };

            string jsonString = JsonSerializer.Serialize(webpageSearch1);




            // get contents of specified web pages
            //string pageHtml = GetHtml("www.google.com");
            string pageHtml = GetHtml("https://www.epsdistribucija.rs/Nis_Dan_3_Iskljucenja.htm");
            if(pageHtml == null)
            {
                // continue;
            }

            // search for specified keywords
            // notify if found

            

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
                ShowMessage("Getting HTML for url '" + url + "' failed with error: '" + ex.InnerException + "'.", "Error");

                return null;
            }
        }

        /// <summary>
        /// Display a message in a messagebox.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">Message type. Options: "Info", "Error".</param>
        private static void ShowMessage(string message, string type = "Info")
        {
            switch (type.ToLower())
            {
                case "info":
                    MessageBox.Show(message, "Webpage notifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "error":
                    MessageBox.Show(message, "Webpage notifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                default:
                    MessageBox.Show(message, "Webpage notifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
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
