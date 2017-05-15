using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLibrary;

namespace LoggerTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    class Program
    {
        #region Customization

        /// <summary>
        /// How many messages should each thread send before shutting down?
        /// </summary>
        private static int NUMBER_OF_MESSAGES_PER_THREAD = 3;

        /// <summary>
        /// How many threads should be sending messages concurrently?
        /// </summary>
        private static int NUMBER_OF_THREADS_SENDING_MESSAGES = 2;

        /// <summary>
        /// Minimum log level supported by your logger
        /// </summary>
        private static int MIN_LOG_LEVEL = 1;

        /// <summary>
        /// Maximum log level supported by your logger
        /// </summary>
        private static int MAX_LOG_LEVEL = 4;

        /// <summary>
        /// How often should an exception be sent into the logger - every X number of messages
        /// </summary>
        //private static int HOW_OFTEN_AN_EXCEPTION = 3;

        #endregion

        static void Main(string[] args)
        {
            LogLibrary.Logger logger = new LogLibrary.Logger();

            List<Thread> threads = new List<Thread>();

            // Setup threads
            for (int i = 1; i <= NUMBER_OF_THREADS_SENDING_MESSAGES; i++)
            {
                int threadNumber = i;

                Thread newThread = new Thread(x =>
                {
                    Random rand = new Random();

                    string errorMessage = "Error Message, Thread number: {0}, Message Number: {1}, Log Level: {2}";
                    //string logId = "{0}{1}{2}";
                    Exception exception = new Exception();
                    int logLevel;

                    for (int messageNumber = 1; messageNumber <= NUMBER_OF_MESSAGES_PER_THREAD; messageNumber++)
                    {
                        logLevel = rand.Next(MIN_LOG_LEVEL, MAX_LOG_LEVEL);
                        logger.Log(/*string.Format(logId, threadNumber, messageNumber, logLevel),*/ "13",/* "b0f42f58-e2b9-4488-b5aa-f99ec2898af3",*/ string.Format(errorMessage, threadNumber, messageNumber, logLevel), "Error");
                    }
                });

                threads.Add(newThread);
            }

            // Start threads
            foreach (Thread aThread in threads)
            {
                aThread.Start();
            }

            // Join threads
            foreach (Thread aThread in threads)
            {
                aThread.Join();
            }
            Console.WriteLine("All Threads Finished");
            Console.ReadLine();
        }
    }

}
