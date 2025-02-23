using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Data;
using VoteSystem.Models;

namespace VoteSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteDbController(AppDbContext context, HttpClient httpClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vote>>> GetVotes()
        {
            return await context.Votes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Vote>> PostVotes(Vote? vote)
        {
            if (vote == null)
            {
                return BadRequest("Vote cannot be null");
            }

            vote.Date = DateTime.Now.ToUniversalTime();

            try
            {
                context.Votes.Add(vote);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving vote to the database");
            }

            return CreatedAtAction(nameof(GetVotes), new { id = vote.Id, time = vote.Date }, vote);
        }

        [HttpGet("get-polls")]
        public async Task<IActionResult> GetPolls()
        {
            var response = await httpClient.GetAsync("http://poll_service/PollsController/");
            var polls = await response.Content.ReadAsStringAsync();
            return Ok(polls);
        }

    }
}