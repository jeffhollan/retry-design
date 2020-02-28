package com.functions;

// This would be part of the functions Java SDK
public class RetryResult {
    public RetryResult(Boolean retryExecution, int retryCount, String delay)
    {
        this.retryExecution = retryExecution;
        this.retryCount = retryCount;
        this.delay = delay;
    }
    public Boolean retryExecution;
    public int retryCount; // -1 for Infinite
    public String delay;   // Timespan 00:00:10 for 10 seconds
}
