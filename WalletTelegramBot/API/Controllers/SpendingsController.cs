using Microsoft.AspNetCore.Mvc;
using MediatR;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.Business.Features;

namespace WalletTelegramBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpendingsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить список всех трат
    /// </summary>
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
        return result.IsSuccess ? Ok(result.value) : BadRequest(result.errorMessages);
    }

    /// <summary>
    /// Создать новую трату
    /// </summary>
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
        return result.IsSuccess ? Ok(result.value) : BadRequest(result.errorMessages);
    }

    /// <summary>
    /// Удалить трату по ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var command = new DeleteSpendingCommand(id);
        var result = await mediator.Send(command);
        return result.IsSuccess ? Ok(result.value) : NotFound(result.errorMessages);
    }
}
