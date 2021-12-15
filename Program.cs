using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI;
using System;
using Nethereum.Signer;
using System.Numerics;
using System.Security.Cryptography;


namespace NethereumSample
{
    class Program
    {
        public static BigInteger Random32Bytes()
        {
            byte[] number = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(number);
            number[number.Length - 1] &= (byte)0x7F; //force sign bit to positive
            return new BigInteger(number);
        }

        static void Main(string[] args)
        {
            var privateKey = "YOUR_PRIVATE_KEY_HERE";
            var signer = new EthereumMessageSigner();
            var abiEncode = new ABIEncode();
            var orderHash = abiEncode.GetSha3ABIEncodedPacked(
                new ABIValue("bytes32", "0x2f89ac57b87363c7e0e88e67aff7424bf977605de87285485181da598fd1f5c7".HexToByteArray()), // marketHash
                new ABIValue("address", "0x6A383cf1F8897585718DCA629a8f1471339abFe4"), // baseToken
                new ABIValue("uint256", BigInteger.Parse("1000000000000000000000")), // totalBetSize
                new ABIValue("uint256", BigInteger.Parse("51302654005356720000")), // percentageOdds
                new ABIValue("uint256", BigInteger.Parse("2209006800")), // expiry (FIXED!), use apiExpiry for the expiry of the orders on sportx.bet
                new ABIValue("uint256", BigInteger.Parse("11088371513674221491423763797610298526004404436355568229714515396917752811892")), // salt, you can use Random.bytes32
                new ABIValue("address", "0x9883D5e7dC023A441A01Ef95aF406C69926a0AB6"), // maker
                new ABIValue("address", "0x3259E7Ccc0993368fCB82689F5C534669A0C06ca"), // executor
                new ABIValue("bool", true) // isMakerBettingOutcomeOne
            );
            var signature = signer.Sign(orderHash, privateKey);
            Console.WriteLine("Order Hash: " + orderHash.ToHex());
            Console.WriteLine("Signature: " + signature.ToString());
            Console.WriteLine("Random bytes: " + Random32Bytes());
        }
    }
}