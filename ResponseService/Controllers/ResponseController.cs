namespace ResponseService.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[Controller]")]
[ApiController]
public class ResponseController : ControllerBase
{
	[Route("{id:int}")]
	[HttpGet]
	public ActionResult GetResponse(int id)
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
