using System.Security.Cryptography;

namespace Links.Api;

public static class ShortCode
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string New() => RandomNumberGenerator.GetString(Alphabet, 7);
}