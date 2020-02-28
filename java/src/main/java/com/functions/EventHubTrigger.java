package com.functions;

import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;
import java.util.*;

/**
 * Azure Functions with Event Hub trigger.
 */
public class EventHubTrigger {
    /**
     * This function will be invoked when an event is received from Event Hub.
     */
    @FunctionName("EventHubTrigger")
    // Set the return type as RetryResult
    public RetryResult run(
        @EventHubTrigger(name = "message", eventHubName = "myeventhub", connection = "EventHubConnectionString", consumerGroup = "$Default", cardinality = Cardinality.MANY) List<String> message,
        final ExecutionContext context
    ) 
    {
        context.getLogger().info("Java Event Hub trigger function executed.");
        Boolean someValue;
        try 
        {
            // try to run some code.
            someValue = true;
        }
        catch(Exception ex)
        {
            // Throws an exception which would then check the retryPolicy in host.json to know how to proceed.
            // Run would result as failed.  Batch would be retried if host.json had a retryPolicy
            throw ex;
        }
        
        if(someValue == false)
        // would retry and run would result in successful.  Batch would be retried.
            return new RetryResult(true, 30, "00:00:30");
        else
        // would not retry run. Would checkpoint and continue. Run would result in successful.
            return null;
    }
}
