using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class EventHubTrigger
    {
        [FunctionName("EventHubTrigger")]
        public static async Task<RetryResult> Run(
            [EventHubTrigger("myeventhub", Connection = "EventHubConnectionString")] EventData[] events, 
            ExecutionContext context,
            ILogger log)
        {
            var exceptions = new List<Exception>();
            bool someValue;
            log.LogInformation($"Batch attempt {context.currentRetryCount}");

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    // Replace these two lines with your processing logic.
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }


            if (exceptions.Count > 1)
            // Throws an exception which would then check the retryPolicy in host.json to know how to proceed.
            // Run would result as failed.  Batch would be retried if host.json had a retryPolicy
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
            // Throws an exception which would then check the retryPolicy in host.json to know how to proceed.
            // Run would result as failed.  Batch would be retried if host.json had a retryPolicy
                throw exceptions.Single();

            if(!someValue) 
            {
                // would retry and run would result in successful.  Batch would be retried.
                return new RetryResult()
                {
                    retryExecution = true,
                    retryCount = 20, // -1 for Infinite
                    delay = TimeSpan.FromSeconds(20)
                };
            }
            else
            {
                // would not retry run. Would checkpoint and continue. Run would result in successful.
                return null;
            }
        }
    }
}
