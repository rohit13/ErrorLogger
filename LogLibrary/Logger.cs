using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Threading;

namespace LogLibrary
{
    public class PostError
    {
        public PostError()
        { }
        //public string ErrorLogID { get; set; }
        //public string UserId { get; set; }
        public string ApplicationID { get; set; }
        public string ErrorCategory { get; set; }
        public string ErrorMessage { get; set; }
        //public string TimeStamp { get; set; }
    }
    public class Logger
    {
        private static int SERVICE_PORT = 17662;
        private static string SERVICE_URL = "http://localhost:{0}/";
        private static string POST_ACTION = "api/RestService";
        private static int execute = 1;
        private static int queueCount = 1;
        private static SWTools.BlockingQueue<PostError> queue;
        public Logger()
        {
        }
        private void startQueue()
        {
            if (queueCount == 1)
            {
                queueCount++;
                queue = new SWTools.BlockingQueue<PostError>();
            }
        }

        private void startThread()
        {
            if (execute == 1)
            {
                execute++;
                Thread newThread = new Thread(x =>
                {
                    while (true)
                    {
                        PostError newLog = queue.deQ();
                        HitOnRest(newLog);
                        Thread.Sleep(10);
                        Console.WriteLine("Sent to rest : " + newLog.ErrorMessage);
                    }
                });

                newThread.Start();
            }
        }

        public void HitOnRest(PostError post)
        {
            string output = string.Empty;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format(SERVICE_URL, SERVICE_PORT));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsync(String.Format(POST_ACTION), new StringContent(new JavaScriptSerializer().Serialize(post), Encoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                Task<string> task = response.Content.ReadAsStringAsync();  // returns immediately
                string temp = task.Result;  // blocks until task completes
                output = "And the result is: " + temp.ToString();
            }
            else
            {
                output = string.Format("ERROR: {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            Console.WriteLine(output);
        }
        public void Log(/*string errorLogId,*/ string applicationId/*, string userId,*/, string errorMessage, string errorCategory, Exception ex = null)
        {
            Console.WriteLine(errorMessage);
            startQueue();
            startThread();
            PostError post = new PostError();
            //post.ErrorLogID = errorLogId;
            post.ApplicationID = applicationId;
            //post.UserId = userId;
            post.ErrorMessage = errorMessage;
            post.ErrorCategory = errorCategory;
            //post.TimeStamp = Convert.ToString(DateTime.Now);
            queue.enQ(post);
        }
    }
}
