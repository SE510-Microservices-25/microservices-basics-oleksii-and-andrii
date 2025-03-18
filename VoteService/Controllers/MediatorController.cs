using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoteSystem.Command;
using VoteSystem.Models;
using VoteSystem.Query;

namespace VoteSystem.Controllers;

[ApiController]
[Route("api/votes")]
public class VotesCqrs : ControllerBase
{
    private readonly IMediator _mediator;

    public VotesCqrs(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Vote>> CreateProduct([FromBody] CreateVoteCommand command)
    {
        var vote = await _mediator.Send(command);
        return Ok(vote);
    }

    [HttpGet]
    public async Task<ActionResult<List<Vote>>> GetProducts()
    {
        var products = await _mediator.Send(new GetVotesQuery());
        return Ok(products);
    }
}