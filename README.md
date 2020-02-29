# Retry design proposal (C#)

The host.json is used to set the general retry policy:

```
{
  "version": "2.0",
  // ...
  // This host.json would only be valid for checkpoint based triggers (CosmosDB, Event Hubs, Kafka)
  "checkpointRetryPolicy":
  {
    "retryCount": 10, // -1 for infinite
    "sleepDuration": "00:00:05"
  }
}
```

In an execution, the return value can also be used to set the result of an execution.  This would be honored regardless if a retry policy is defined in `host.json`.  If one is defined, the return value would override for that retry.

# Examples

Scenario 1: The execution occurs, the messages are processed. No exceptions are thrown by the function, and no `RetryResult` is returned.  The stream continues to progress to the next batch.

Scenario 2: The execution occurs, an exception is thrown, but no retry policy is defined in host.json.  The execution will be marked as failed. The stream continues to progress to the next batch.

Scenario 3: The execution occurs, an exception is thrown, there is a retry policy defined in host.json.  The execution will be marked as failed. The stream WILL NOT continue on with the checkpoint.  The retry policy will be honored.  After the final retry has happened (if an upper limit was given) the stream WILL continue processing.

Scenario 4: The execution occurs, *no* exception is thrown, but a `RetryResult` object is returned.  The execution will be marked as succeeded.  The stream WILL NOT continue on with the checkpoint. The retry policy defined in the retryResult will attempt to be honored (potentially overwriting one defined in host.json).  If all retries have been exhausted as defined in the `RetryResult`, the stream WILL continue processing.

# How it works and considerations

- If your trigger passes in a single message (e.g. `EventData`) and not the entire batch (e.g. `EventData[]`), if *any* messages in the current batch result in a retry attempt the ENTIRE BATCH will be retried.

- All retry state is kept IN MEMORY of the current host.  That means it's possible you define a retry count of 10.  After 4 attempts, the host loses the lease, another host will pick it up.  It's retry count would be at 0.  It's possible it tries another 10 times.  Meaning that message may have attempted to process 14 times even though the retry count was 10.

