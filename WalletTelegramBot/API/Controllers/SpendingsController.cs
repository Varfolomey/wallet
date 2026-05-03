using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.API;
using WalletTelegramBot.Business.Features;

namespace WalletTelegramBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpendingsController(IMediator mediator, IHubContext<SpendingHub> hub) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SpendingDto>>> GetList(
        [FromQuery] string? userName,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = new GetSpendingsQuery
        {
            UserName = userName,
            FromDate = fromDate,
            ToDate = toDate
        };
        var result = await mediator.Send(query);
        return result.Success ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateSpendingDto dto)
    {
        var command = new CreateSpendingCommand
        {
            Amount = dto.Amount,
            Comment = dto.Comment,
            UserName = dto.UserName
        };
        var result = await mediator.Send(command);
        if (!result.Success)
            return BadRequest(result.Message);

        await hub.Clients.All.SendAsync("spendingChanged");
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var command = new DeleteSpendingCommand(id);
        var result = await mediator.Send(command);
        if (!result.Success)
            return NotFound(result.Message);

        await hub.Clients.All.SendAsync("spendingChanged");
        return Ok(result.Data);
    }
}
