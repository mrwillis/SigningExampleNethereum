using System.Numerics;
using System.Security.Cryptography;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI;
using Nethereum.Signer;
using Jering.Javascript.NodeJS;
using System.Diagnostics;

namespace NethereumSample
{
    public static class GlobalVar
    {
        public const string JS_FILE_LOCATION = "./node_signatures/dist/main.js";
    }

    class Order
    {
        public static BigInteger Random32Bytes()
        {
            byte[] number = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(number);
            number[^1] &= (byte)0x7F; //force sign bit to positive
            return new BigInteger(number);
        }

        public readonly BigInteger expiry = BigInteger.Parse("2209006800");
        public string marketHash;
        public string baseToken;
        public BigInteger totalBetSize;
        public BigInteger percentageOdds;
        public string maker;
        public string executor;
        public bool isMakerBettingOutcomeOne;
        public BigInteger salt;

        public Order(
            string marketHash,
            string baseToken,
            BigInteger totalBetSize,
            BigInteger percentageOdds,
            string maker,
            string executor,
            bool isMakerBettingOutcomeOne
        )
        {
            this.marketHash = marketHash;
            this.baseToken = baseToken;
            this.totalBetSize = totalBetSize;
            this.percentageOdds = percentageOdds;
            this.maker = maker;
            this.executor = executor;
            this.isMakerBettingOutcomeOne = isMakerBettingOutcomeOne;
            this.salt = Random32Bytes();
        }
        public string GetSignature(string privateKey)
        {
            var signer = new EthereumMessageSigner();
            var hash = this.GetHash();
            return signer.Sign(hash, privateKey);
        }

        public static async Task<string?> GetCancelSignature(
            string[] orderHashes,
            string privateKey
        )
        {
            // Invoke javascript
            string? result = await StaticNodeJSService.InvokeFromFileAsync<string>(
                GlobalVar.JS_FILE_LOCATION,
                args: new object[] { orderHashes, privateKey }
            );
            return result;
        }

        public byte[] GetHash()
        {
            var abiEncode = new ABIEncode();
            var orderHash = abiEncode.GetSha3ABIEncodedPacked(
                new ABIValue("bytes32", this.marketHash.HexToByteArray()),
                new ABIValue("address", this.baseToken),
                new ABIValue("uint256", this.totalBetSize),
                new ABIValue("uint256", this.percentageOdds),
                new ABIValue("uint256", this.expiry),
                new ABIValue("uint256", this.salt),
                new ABIValue("address", this.maker),
                new ABIValue("address", this.executor),
                new ABIValue("bool", this.isMakerBettingOutcomeOne)
            );
            return orderHash;
        }
    }
}
