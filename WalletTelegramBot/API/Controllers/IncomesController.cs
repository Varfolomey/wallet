using Microsoft.AspNetCore.Mvc;
using MediatR;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.Business.Features;

namespace WalletTelegramBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить список всех доходов
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<IncomeDto>>> GetList(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = new GetIncomesQuery 
        { 
            FromDate = fromDate,
            ToDate = toDate
        };
        var result = await mediator.Send(query);
        return result.IsSuccess ? Ok(result.value) : BadRequest(result.errorMessages);
    }

    /// <summary>
    /// Создать новый доход
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateIncomeDto dto)
    {
        var command = new CreateIncomeCommand
        {
            Amount = dto.Amount,
            Comment = dto.Comment
        };
        var result = await mediator.Send(command);
        return result.IsSuccess ? Ok(result.value) : BadRequest(result.errorMessages);
    }

    /// <summary>
    /// Удалить доход по ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var command = new DeleteIncomeCommand(id);
        var result = await mediator.Send(command);
        return result.IsSuccess ? Ok(result.value) : NotFound(result.errorMessages);
    }
}
