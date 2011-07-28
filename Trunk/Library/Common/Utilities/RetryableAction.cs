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
        /// A message describing the action.  The primary purpose is to make the action identifiable in the log output.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The maximum number of retries to attempt
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// The number of milliseconds to wait between retries.
        /// <remarks>The delay period is approximate and will be affected by the demands of other threads on the system.</remarks>
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// A factor by which the delay is adjusted after each retry.  Default is 1.
        /// <remarks>To double the delay with every retry use a factor of 2, retrys will be 1s, 2s, 4s, 8s...</remarks>
        /// <remarks>To quarter the delay with every retry use a factor of 0.25, retrys will be 1s, 0.25, 0.0625, 0.015625s...</remarks>
        /// </summary>
        public float DelayMultiplier { get; set; }

        public static void RetryEverySecondFor30Seconds(Action action, string description)
        {
            new RetryableAction(action, description, 30, TimeSpan.FromSeconds(1)).TryIt();
        }

        public RetryableAction(Action action, string description, int maxRetries, TimeSpan delay) : this(action, description, maxRetries, delay, 1) {}

        public RetryableAction(Action action, string description, int maxRetries, TimeSpan delay, float delayMultiplier)
        {
            if(delay.TotalMilliseconds > int.MaxValue)
            {
                throw new ArgumentException(string.Format("delay must be less than {0} milliseconds", int.MaxValue));
            }
            
            Action = action;
            Description = description;
            MaxRetries = maxRetries;
            Delay = delay;
            DelayMultiplier = delayMultiplier;
        }

        public void TryIt()
        {
            var currentDelay = (int) Delay.TotalMilliseconds;
            int retrysRemaining = MaxRetries;

            do
            {
                try
                {
                    Action();
                    DnnLog.Trace("Action succeeded - {0}", Description);
                    return;
                }
                catch(Exception)
                {
                    if (retrysRemaining <= 0)
                    {
                        DnnLog.Warn("All retries of action failed - {0}", Description);
                        throw;
                    }

                    DnnLog.Trace("Retrying action {0} - {1}", retrysRemaining, Description);
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