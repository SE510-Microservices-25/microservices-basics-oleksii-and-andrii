using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Data;
using VoteSystem.Models;

namespace VoteSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteDbController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vote>>> GetVotes()
        {
            return await context.Votes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Vote>> PostVotes(Vote vote)
        {
            vote.VoteDate = DateTime.Now.ToUniversalTime();
            context.Votes.Add(vote);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVotes), new { id = vote.Id }, vote);
        }
    }
}