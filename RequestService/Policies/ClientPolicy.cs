namespace RequestService.Policies;

using Polly;
using Polly.Retry;

public class ClientPolicy
{
	public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; set; }

	public ClientPolicy()
	{
		ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
				res => !res.IsSuccessStatusCode).RetryAsync(5);
	}
}
