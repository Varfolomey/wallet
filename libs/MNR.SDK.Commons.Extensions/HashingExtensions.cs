using System.Security.Cryptography;
using System.Text;

namespace MNR.SDK.Commons.Extensions;

public static class HashingExtensions
{
    public static string Sha512(this string input)
    {
        using var hashManager = SHA512.Create();
        var hash = hashManager.ComputeHash(Encoding.UTF8.GetBytes(input));
        return GetStringFromHash(hash);
    }

    public static string Sha1(this string input)
    {
        using var hashManager = SHA1.Create();
        var hash = hashManager.ComputeHash(Encoding.UTF8.GetBytes(input));
        return GetStringFromHash(hash);
    }

    public static string Md5(this string input)
    {
        using var hashManager = MD5.Create();
        var hash = hashManager.ComputeHash(Encoding.UTF8.GetBytes(input));
        return GetStringFromHash(hash);
    }

    public static string HmacSha256(this string value, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var byteArray = Encoding.UTF8.GetBytes(value);

        using var hmac = new HMACSHA256(keyBytes);
        using var stream = new MemoryStream(byteArray);
        return GetStringFromHash(hmac.ComputeHash(stream));
    }

    public static string HmacMd5(this string value, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var byteArray = Encoding.UTF8.GetBytes(value);

        using var hmac = new HMACMD5(keyBytes);
        using var stream = new MemoryStream(byteArray);
        return GetStringFromHash(hmac.ComputeHash(stream));
    }

    public static string JsonBase64(this object value)
    {
        return value.ToJson().Base64();
    }

    public static T FromBase64Json<T>(this string data)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(data)).FromJson<T>();
    }

    public static string Base64(this string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    private static string GetStringFromHash(IEnumerable<byte> hash)
    {
        var result = new StringBuilder();
        foreach (var t in hash)
            result.Append(t.ToString("X2"));
        return result.ToString();
    }
}