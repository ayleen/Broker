namespace Ayleen.Broker.Api.Services;

public static class Helpers
{
    public static string GetMd5(string input)
    {
        var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        var hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}