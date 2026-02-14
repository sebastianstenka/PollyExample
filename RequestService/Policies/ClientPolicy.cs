namespace RequestService.Policies;

using System.Net;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

public class ClientPolicy
{
    // === RETRY ===
    public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialWithJitterRetry { get; }

    // Retry for transient HTTP
    public AsyncRetryPolicy<HttpResponseMessage> TransientHttpRetry { get; }

    // === CIRCUIT BREAKER ===
    public AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreaker { get; }

    // === TIMEOUT ===
    public AsyncTimeoutPolicy<HttpResponseMessage> TimeoutPolicy { get; }

    // === BULKHEAD ===
    public AsyncBulkheadPolicy<HttpResponseMessage> BulkheadPolicy { get; }

    // === COMBINED POLICIES ===
    public AsyncPolicyWrap<HttpResponseMessage> RetryAndBreaker { get; }
    public AsyncPolicyWrap<HttpResponseMessage> FullResiliencePipeline { get; }

    public ClientPolicy()
    {
        // ===== COMMON HANDLER =====
        var transientHandler = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(res =>
                (int)res.StatusCode >= 500 ||
                res.StatusCode == HttpStatusCode.RequestTimeout ||
                res.StatusCode == HttpStatusCode.TooManyRequests);

        // ===== BASIC RETRY =====
        ImmediateHttpRetry = transientHandler
            .RetryAsync(3);

        LinearHttpRetry = transientHandler
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));

        ExponentialHttpRetry = transientHandler
            .WaitAndRetryAsync(5,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        // ===== EXPONENTIAL + JITTER =====
        ExponentialWithJitterRetry = transientHandler
            .WaitAndRetryAsync(5, attempt =>
            {
                var jitter = Random.Shared.NextDouble();
                return TimeSpan.FromSeconds(Math.Pow(2, attempt) + jitter);
            });

        // ===== TRANSIENT HTTP ONLY =====
        TransientHttpRetry = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(res =>
                (int)res.StatusCode >= 500)
            .RetryAsync(3);

        // ===== CIRCUIT BREAKER =====
        CircuitBreaker = transientHandler
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (_, _) => Console.WriteLine("Circuit OPEN"),
                onReset: () => Console.WriteLine("Circuit CLOSED"));

        // ===== TIMEOUT =====
        TimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(5);

        // ===== BULKHEAD =====
        BulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization: 5,
            maxQueuingActions: 10);

        // ===== COMBINED =====

        RetryAndBreaker = Policy.WrapAsync(
            ExponentialWithJitterRetry,
            CircuitBreaker);

        FullResiliencePipeline = Policy.WrapAsync(
            BulkheadPolicy,
            TimeoutPolicy,
            ExponentialWithJitterRetry,
            CircuitBreaker);
    }
}
