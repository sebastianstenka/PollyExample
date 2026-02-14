using Microsoft.AspNetCore.Mvc;

namespace PollyResponseService.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class ResponseController : ControllerBase
{
	[Route("{id:int}")]
	[HttpGet]
	public ActionResult GetAResponse(int id)
	{
		var random = new Random();
		var randomNumber = random.Next(1, 101);

		if (randomNumber >= id)
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		return Ok();
	}
}
