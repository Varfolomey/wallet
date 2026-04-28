using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WalletApp;

public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly JsonSerializerOptions _jsonOptions;

	public ApiService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_jsonOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			WriteIndented = false
		};
	}

	// Spendings
	public async Task<List<SpendingDto>> GetSpendingsAsync(string? userName = null, DateTime? fromDate = null, DateTime? toDate = null)
	{
		var queryBuilder = new StringBuilder("?");
		if (!string.IsNullOrEmpty(userName))
			queryBuilder.Append($"userName={Uri.EscapeDataString(userName)}&");
		if (fromDate.HasValue)
			queryBuilder.Append($"fromDate={fromDate.Value:yyyy-MM-dd}&");
		if (toDate.HasValue)
			queryBuilder.Append($"toDate={toDate.Value:yyyy-MM-dd}&");

		var query = queryBuilder.ToString().TrimEnd('&');
		var response = await _httpClient.GetAsync($"api/spendings{query}");
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<List<SpendingDto>>(json, _jsonOptions) ?? new List<SpendingDto>();
	}

	public async Task<SpendingDto> CreateSpendingAsync(SpendingCreateDto spending)
	{
		var json = JsonSerializer.Serialize(spending, _jsonOptions);
		var content = new StringContent(json, Encoding.UTF8, "application/json");
		var response = await _httpClient.PostAsync("api/spendings", content);
		response.EnsureSuccessStatusCode();
		var responseJson = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<SpendingDto>(responseJson, _jsonOptions)!;
	}

	public async Task DeleteSpendingAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"api/spendings/{id}");
		response.EnsureSuccessStatusCode();
	}

	// Incomes
	public async Task<List<IncomeDto>> GetIncomesAsync(DateTime? fromDate = null, DateTime? toDate = null)
	{
		var queryBuilder = new StringBuilder("?");
		if (fromDate.HasValue)
			queryBuilder.Append($"fromDate={fromDate.Value:yyyy-MM-dd}&");
		if (toDate.HasValue)
			queryBuilder.Append($"toDate={toDate.Value:yyyy-MM-dd}&");

		var query = queryBuilder.ToString().TrimEnd('&');
		var response = await _httpClient.GetAsync($"api/incomes{query}");
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<List<IncomeDto>>(json, _jsonOptions) ?? new List<IncomeDto>();
	}

	public async Task<IncomeDto> CreateIncomeAsync(IncomeCreateDto income)
	{
		var json = JsonSerializer.Serialize(income, _jsonOptions);
		var content = new StringContent(json, Encoding.UTF8, "application/json");
		var response = await _httpClient.PostAsync("api/incomes", content);
		response.EnsureSuccessStatusCode();
		var responseJson = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<IncomeDto>(responseJson, _jsonOptions)!;
	}

	public async Task DeleteIncomeAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"api/incomes/{id}");
		response.EnsureSuccessStatusCode();
	}

	// Reports
	public async Task<DayReportDto> GetDayReportAsync(DateTime date)
	{
		var response = await _httpClient.GetAsync($"api/reports/day/{date:yyyy-MM-dd}");
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<DayReportDto>(json, _jsonOptions)!;
	}

	public async Task<MonthReportDto> GetMonthReportAsync(int year, int month)
	{
		var response = await _httpClient.GetAsync($"api/reports/month/{year}/{month}");
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<MonthReportDto>(json, _jsonOptions)!;
	}
}

// DTO модели
public class SpendingDto
{
	public int Id { get; set; }
	public decimal Amount { get; set; }
	public string Comment { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public DateTime DateTime { get; set; }
}

public class SpendingCreateDto
{
	public decimal Amount { get; set; }
	public string Comment { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
}

public class IncomeDto
{
	public int Id { get; set; }
	public decimal Amount { get; set; }
	public string Comment { get; set; } = string.Empty;
	public DateTime Date { get; set; }
}

public class IncomeCreateDto
{
	public decimal Amount { get; set; }
	public string Comment { get; set; } = string.Empty;
}

public class DayReportDto
{
	public DateTime Date { get; set; }
	public decimal TotalSpendings { get; set; }
	public decimal TotalIncomes { get; set; }
	public decimal Balance { get; set; }
}

public class MonthReportDto
{
	public int Year { get; set; }
	public int Month { get; set; }
	public List<DayReportDto> Days { get; set; } = new();
	public decimal TotalBalance { get; set; }
}
