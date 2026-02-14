namespace RequestService.Controllers;

using Microsoft.AspNetCore.Mvc;

using Policies;

[Route("api/[Controller]")]
[ApiController]
public class RequestController(ClientPolicy clientPolicy, IHttpClientFactory clientFactory) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult> MakeRequest()
	{
		var client = clientFactory.CreateClient();

		var response = await clientPolicy.ExponentialHttpRetry.ExecuteAsync(() => client.GetAsync("http://localhost:5020/api/response/25"));

		if (response.IsSuccessStatusCode)
		{
			Console.WriteLine("Request successful");
			return Ok();
		}

		Console.WriteLine("Request failed");
		return StatusCode(StatusCodes.Status500InternalServerError);
	}
}
