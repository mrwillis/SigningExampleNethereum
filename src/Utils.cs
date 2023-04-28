using System.Numerics;
using System.Security.Cryptography;

namespace SigningExampleNethereum {

    public static class Utilities {
public static BigInteger Random32Bytes()
        {
            byte[] number = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(number);
            number[^1] &= (byte)0x7F; //force sign bit to positive
            return new BigInteger(number);
        }
    }       
}