namespace RequestService.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[Controller]")]
[ApiController]
public class RequestController(IHttpClientFactory clientFactory) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult> MakeRequest()
	{
		using var response = await clientFactory.CreateClient("ClientWithImmediateHttpRequest").GetAsync("http://localhost:5020/api/response/25");

		if (response.IsSuccessStatusCode)
		{
			Console.WriteLine("Request successful");
			return Ok();
		}

		Console.WriteLine("Request failed");
		return StatusCode(StatusCodes.Status500InternalServerError);
	}
}
