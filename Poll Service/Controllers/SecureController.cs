using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PollSystem.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class SecureController
{
	[HttpGet]
	public IActionResult Get()
	{
		return new OkResult();
	}
}
