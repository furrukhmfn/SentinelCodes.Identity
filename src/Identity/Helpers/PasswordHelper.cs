using System.Security.Cryptography;
using System.Text;

namespace Identity.Helpers;

public class PasswordHelper
{
    public readonly int _keySize;
    public readonly int _iterations;
    public readonly HashAlgorithmName _hashAlgorithmName;

    public PasswordHelper(int keySize = 64,
                          int iterations = 35000)
    {
        this._keySize = keySize;
        this._iterations = iterations;
        this._hashAlgorithmName = HashAlgorithmName.SHA512;
    }

    public (string salt, string hash) CreatePasswordSaltAndHash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_keySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            _iterations,
            _hashAlgorithmName,
            _keySize);

        return (salt: Convert.ToHexString(salt), hash: Convert.ToHexString(hash));
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromHexString(salt),
            _iterations,
            _hashAlgorithmName,
            _keySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
