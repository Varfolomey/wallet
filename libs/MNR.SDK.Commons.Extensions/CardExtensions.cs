using System.Text.RegularExpressions;

namespace MNR.SDK.Commons.Extensions;

public static class CardExtensions
{
    private const string UnknownBrand = "Unknown";
    private const string VisaBrand = "VISA";
    private const string MasterCardBrand = "MasterCard";
    private const string InvalidCardLabel = "Invalid card";

    private static readonly Regex CardRegex = new(@"^\d{12,20}$", RegexOptions.Compiled);
    private static readonly Regex Visa = new(@"^4[0-9]{12}(?:[0-9]{3})?$", RegexOptions.Compiled);
    private static readonly Regex MasterCard =
        new(@"^(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}$",
            RegexOptions.Compiled);

    public static string GetCardType(this string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return UnknownBrand;

        return cardNumber switch
        {
            { } card when Visa.IsMatch(card) => VisaBrand,
            { } card when MasterCard.IsMatch(card) => MasterCardBrand,
            _ => UnknownBrand
        };
    }
    
    public static string NormalizeCard(this string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return null;
        var result = new string(cardNumber.Where(char.IsDigit).ToArray());
        return CardRegex.IsMatch(result) ? result : InvalidCardLabel;
    }

    public static string MaskCardNumber(this string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentNullException(nameof(cardNumber), "Card number cannot be null");

        var trimmedCard = cardNumber.Replace(" ", string.Empty).Trim();
        if (!CardRegex.IsMatch(trimmedCard))
            return InvalidCardLabel;

        var card =
            $"{trimmedCard[..6]}{string.Join("", Enumerable.Repeat('*', trimmedCard.Length - 10))}{trimmedCard[^4..]}";
        var result = string.Empty;
        for (var i = 1; i <= card.Length; i++)
            result += i % 4 == 0 ? $"{card[i - 1]} " : card[i - 1];

        return result.Trim();
    }
}