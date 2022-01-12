using System.Numerics;
using System.Security.Cryptography;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI;
using Nethereum.Signer;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;

namespace NethereumSample
{
    public static class GlobalVar
    {
        public const string JS_FILE_LOCATION = "./node_signatures/dist/main.js";
    }

    class BigIntegerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BigInteger);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var bigInt = (BigInteger)value;
            var stringBigInt = value.ToString();
            writer.WriteValue(stringBigInt);
        }
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

        public long apiExpiry;

        public readonly BigInteger expiry = BigInteger.Parse("2209006800");
        public string marketHash;
        public string baseToken;

        [JsonConverter(typeof(BigIntegerConverter))]
        public BigInteger totalBetSize;

        [JsonConverter(typeof(BigIntegerConverter))]
        public BigInteger percentageOdds;
        public string maker;
        public string executor;
        public bool isMakerBettingOutcomeOne;

        [JsonConverter(typeof(BigIntegerConverter))]
        public BigInteger salt;

        public string GetSignature(string privateKey)
        {
            var signer = new EthereumMessageSigner();
            var hash = this.GetHash();
            return signer.Sign(hash, privateKey);
        }

        public SignedOrder GetSignedOrder(string privateKey)
        {
            var signature = this.GetSignature(privateKey);
            return new SignedOrder
            {
                marketHash = this.marketHash,
                baseToken = this.baseToken,
                totalBetSize = this.totalBetSize,
                percentageOdds = this.percentageOdds,
                maker = this.maker,
                executor = this.executor,
                isMakerBettingOutcomeOne = this.isMakerBettingOutcomeOne,
                apiExpiry = this.apiExpiry,
                salt = this.salt,
                signature = signature
            };
        }

        public static async Task<string?> GetCancelSignature(
            string[] orderHashes,
            string privateKey,
            int chainId
        )
        {
            // Invoke javascript
            string? result = await StaticNodeJSService.InvokeFromFileAsync<string>(
                GlobalVar.JS_FILE_LOCATION,
                args: new object[] { orderHashes, privateKey, 80001 }
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

    class SignedOrder : Order
    {
        public string signature;
    }
}
