namespace Ayleen.Broker.FileBroker.Services;

public static class DataCalculator
{
    public static string Calculate(string key)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        var inputBytes = System.Text.Encoding.ASCII.GetBytes(key);

        var hashBytes = aes.EncryptCbc(inputBytes, new byte[16]);
        return Convert.ToHexString(hashBytes);
    }
}