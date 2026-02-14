namespace RequestService.Policies;

using Polly;
using Polly.Retry;

public class ClientPolicy
{
	public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; set; }
	public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; set; }

	public ClientPolicy()
	{
		ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
				res => !res.IsSuccessStatusCode).RetryAsync(5);

		LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(
			res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(3));
	}
}
