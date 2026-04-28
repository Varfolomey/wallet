using System.Globalization;
using MediatR;
using MNR.SDK.Commons.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using WalletTelegramBot.Business.Features;
using IResult = MNR.SDK.Commons.Models.IResult;

namespace WalletTelegramBot.Services.Internals;

public class UpdateHandler(ITelegramBotClient bot,
    IMediator mediator, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private static readonly InputPollOption[] PollOptions = ["Hello", "World!"];

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), token);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnMessageEdited(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            { Poll: { } poll } => OnPoll(poll),
            { PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
        
    }

    private async Task OnMessageEdited(Message msg)
    {
        logger.LogInformation("Receive message update: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        IResult result = null;
        
        var command = messageText.Split(' ')[0];

        try
        {
            if (decimal.TryParse(command.TrimStart('+'), out var amount))
            {
                if (!command.StartsWith('+'))
                    amount *= -1;
                
                result = await UpdateSpending(amount, msg);
            }

            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        if (result?.Success ?? false)
        {
            await bot.SetMessageReaction(msg.Chat.Id, msg.MessageId,
                [new ReactionTypeEmoji { Emoji = "💊" }]);
            return;
        }
    }

    private async Task<IResult> UpdateSpending(decimal amount, Message message)
    {
        var result = await mediator.Send(new UpdateChatMessageCommand
        {
            Amount = amount,
            MessageText = string.Join(' ', message.Text.Split(' ')[1..]),
            TelegramMessageId = message.Id,
            DataAreaId = message.Chat.Id.ToString(),
        });

        return result;
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        IResult result = null;
        
        var command = messageText.Split(' ')[0];
    
        try
        {
            if (decimal.TryParse(command.TrimStart('+'), out var amount))
            {
                if (!command.StartsWith('+'))
                    amount *= -1;

                result = await AddSpending(amount, msg);
            }
            else if (command.StartsWith("."))
            {
                var c = command[1..];

                result = c switch
                {
                    "i" => await AddIncome(msg),
                    "cats" => await GetCats(msg),
                    "agg" => await GetAggregated(msg),
                    "day" => await GetDay(msg),
                    "add_cat" => await AddCat(msg),
                    "rem_cat" => await RemCat(msg),
                    "months" => await GetMonths(msg),
                    _ => await Usage(msg)
                };
            }
            else if (command is "/cats@artur_sem_FMC_bot" or "/cats")
                result = await GetCats(msg);
            else if (command is "/day@artur_sem_FMC_bot" or "/day")
                result = await GetDay(msg);
            else if (command is "/agg@artur_sem_FMC_bot" or "/agg")
                result = await GetAggregated(msg);
            
            if (result?.Success ?? false)
            {
                await bot.SetMessageReaction(msg.Chat.Id, msg.MessageId,
                    [new ReactionTypeEmoji { Emoji = "👀" }]);
                return;
            }

            await Usage(msg);
        }
        catch (Exception) { await Usage(msg); }

    }

    
    private async Task<IResult> AddCat(Message msg)
    {
        var splitted = msg.Text.Split(' ');
        var result = await mediator.Send(new AddCategoryCommand { Name = splitted[1] });

        return result;
    }
    
    private async Task<IResult> RemCat(Message msg)
    {
        var splitted = msg.Text.Split(' ');
        var result = await mediator.Send(new RemoveCategoryCommand { Name = splitted[1] });

        return result;
    }
    
    private async Task<IResult> GetMonths(Message msg)
    {
        var result = await mediator.Send(new GetMonthsQuery());
        await bot.SendMessage(msg.Chat, result.Data, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return result;
    }
    
    private async Task<IResult> GetCats(Message msg)
    {
        var result = await mediator.Send(new GetCatsQuery());
        await bot.SendMessage(msg.Chat, result.Data, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return result;
    }

    private async Task<IResult> GetAggregated(Message msg)
    {
        var result = await mediator.Send(new GetCatsAggregatedQuery());
        await bot.SendMessage(msg.Chat, result.Data, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return result;
    }

    private async Task<IResult> GetDay(Message msg)
    {
        var command = msg.Text?.Split(' ') ?? [];
        var date = command.Length > 1 
            ? DateTime.ParseExact(command[1], "dd.MM.yyyy", CultureInfo.InvariantCulture) 
            : DateTime.Today;
        
        var result = await mediator.Send(new GetDayQuery(date));
        await bot.SendMessage(msg.Chat, result.Data, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return result;
    }

    private async Task<Result> AddIncome(Message msg)
    {
        var splitted = msg.Text.Split(' ');

        var result = await mediator.Send(new AddIncomeCommand
        {
            Amount = decimal.Parse(splitted[1]),
            Comment = splitted[2],
            TelegramMessageId = msg.Id,
            DataAreaId = msg.Chat.Id.ToString(),
        });

        return result;
    }

    private async Task<Result> AddSpending(decimal amount, Message message)
    {
        var result = await mediator.Send(new AddSpendingCommand
        {
            Amount = amount,
            MessageText = string.Join(' ', message.Text.Split(' ')[1..]),
            Author = message.From?.FirstName,
            TelegramMessageId = message.Id,
            DataAreaId = message.Chat.Id.ToString(),
        });

        return result;
    }

    async Task<Result> Usage(Message msg)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                .i - приход, <i>например: .i 100 зарплата</i>
                .cats - отчёт, <i>например: .cats</i>
                .day [дата] - отчёт, <i>например: .day или .day 21.12.2023</i>
            """;
        await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return Result.Ok();
    }

    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }

    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    async Task<Message> SendInlineKeyboard(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow().AddButton("2.1").AddButton("2.2");
        return await bot.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await bot.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> RequestContactAndLocation(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddButton(KeyboardButton.WithRequestLocation("Location"))
            .AddButton(KeyboardButton.WithRequestContact("Contact"));
        return await bot.SendMessage(msg.Chat, "Who or Where are you?", replyMarkup: replyMarkup);
    }

    async Task<Message> StartInlineQuery(Message msg)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await bot.SendMessage(msg.Chat, "Press the button to start Inline Query\n\n" +
                                               "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
    }

    async Task<Message> SendPoll(Message msg)
    {
        return await bot.SendPoll(msg.Chat, "Question", PollOptions, isAnonymous: false);
    }

    async Task<Message> SendAnonymousPoll(Message msg)
    {
        return await bot.SendPoll(chatId: msg.Chat, "Question", PollOptions);
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await bot.AnswerCallbackQuery(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendMessage(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = [ // displayed result
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQuery(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendMessage(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task OnPoll(Poll poll)
    {
        logger.LogInformation("Received Poll info: {Question}", poll.Question);
        return Task.CompletedTask;
    }

    private async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        var answer = pollAnswer.OptionIds.FirstOrDefault();
        var selectedOption = PollOptions[answer];
        if (pollAnswer.User != null)
            await bot.SendMessage(pollAnswer.User.Id, $"You've chosen: {selectedOption.Text} in poll");
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}