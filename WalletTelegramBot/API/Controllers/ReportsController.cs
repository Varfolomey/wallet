using Microsoft.AspNetCore.Mvc;
using MediatR;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.Business.Features;

namespace WalletTelegramBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить отчёт за день
    /// </summary>
    [HttpGet("day/{date:datetime}")]
    public async Task<ActionResult<DayReportDto>> GetDayReport(DateTime date)
    {
        var query = new GetDayReportQuery(date);
        var result = await mediator.Send(query);
        return result.Success ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>
    /// Получить отчёт за месяц
    /// </summary>
    [HttpGet("month/{year:int}/{month:int}")]
    public async Task<ActionResult<List<MonthReportDto>>> GetMonthReport(int year, int month)
    {
        var query = new GetMonthReportQuery(year, month);
        var result = await mediator.Send(query);
        return result.Success ? Ok(result.Data) : BadRequest(result.Message);
    }
}
