using System.Text.RegularExpressions;
using PhoneNumbers;

namespace MNR.SDK.Commons.Extensions;

public static class PhoneExtensions
{
    private static readonly Regex PhoneRegex = new("[^a-zA-Z0-9]+", RegexOptions.Compiled);

    public static string NormalizePhone(this string phone)
    {
        return new string((phone ?? throw new ArgumentNullException(nameof(phone)))
            .Where(char.IsDigit).ToArray());
    }

    public static string FormatPhone(this string phone)
        => PhoneRegex.Replace(phone, string.Empty);

    public static bool IsPhoneValid(this string phone, string countryId)
    {
        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(countryId))
            return false;

        var phoneNumber = new PhoneNumber();
        var validator = PhoneNumberUtil.GetInstance();
        var code = validator.GetCountryCodeForRegion(countryId);
        var number = phone.NormalizePhone().Substring(code.ToString().Length);

        if (string.IsNullOrWhiteSpace(number))
            return false;

        return validator
            .IsValidNumber(phoneNumber
                .ToBuilder()
                .SetNationalNumber(ulong.Parse(number))
                .SetCountryCode(code)
                .Build());
    }
}