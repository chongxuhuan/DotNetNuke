using System;
using System.Threading;

using DotNetNuke.Instrumentation;

namespace DotNetNuke.Common.Utilities.Internal
{
    /// <summary>
    /// Allows an action to be run and retried after a delay when an exception is thrown.
    /// <remarks>If the action never succeeds the final exception will be re-thrown for the caller to catch.</remarks>
    /// </summary>
    public class RetryableAction
    {
        /// <summary>
        /// The Action to execute
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// A message describing the action to be used in the log messages
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// The maximum number of retries to attempt
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// The number of milliseconds to wait between retries.
        /// <remarks>The delay period is approximate and will be affected by the demands of other threads on the system.</remarks>
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// A factor by which the delay is adjusted after each retry.  Default is 1.
        /// <remarks>To double the delay with every retry use a factor of 2, retrys will be 1s, 2s, 4s, 8s...</remarks>
        /// <remarks>To quarter the delay with every retry use a factor of 0.25, retrys will be 1s, 0.25, 0.0625, 0.015625s...</remarks>
        /// </summary>
        public float DelayMultiplier { get; set; }

        public RetryableAction(Action action, string logMessage, int maxRetries = 30, int delay = 1000, float delayMultiplier = 1)
        {
            Action = action;
            LogMessage = logMessage;
            MaxRetries = maxRetries;
            Delay = delay;
            DelayMultiplier = delayMultiplier;
        }

        public void TryIt()
        {
            var currentDelay = Delay;
            int retrysRemaining = MaxRetries;

            do
            {
                try
                {
                    Action();
                    return;
                }
                catch(Exception)
                {
                    if (retrysRemaining <= 0)
                    {
                        DnnLog.Warn("All retries failed - " + LogMessage);
                        throw;
                    }

                    DnnLog.Info(string.Format("Retrying operation {0} - {1}", retrysRemaining, LogMessage));
                    Thread.Sleep(currentDelay);

                    const double epsilon = 0.0001;
                    if(Math.Abs(DelayMultiplier - 1) > epsilon)
                    {
                        currentDelay = (int)(currentDelay * DelayMultiplier);
                    }
                }
                retrysRemaining--;
            } while (true);
        }
    }
}