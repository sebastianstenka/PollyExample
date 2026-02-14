namespace RequestService.Controllers;

using Microsoft.AspNetCore.Mvc;

using Policies;

[Route("api/[Controller]")]
[ApiController]
public class RequestController(ClientPolicy clientPolicy) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult> MakeRequest()
	{
		var client = new HttpClient();

		var response = await client.GetAsync("http://localhost:5020/api/response/25");

		if (response.IsSuccessStatusCode)
		{
			return Ok();
		}

		return StatusCode(StatusCodes.Status500InternalServerError);
	}
}
