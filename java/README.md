# Retry design proposal (Java)

The host.json is used to set the general retry policy:

```json
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