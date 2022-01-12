using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;
using Nethereum.Signer;
using Newtonsoft.Json;

namespace NethereumSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
            if (privateKey == null)
            {
                throw new Exception("PRIVATE_KEY is not defined");
            }

            var order = new Order
            {
                marketHash = "0x78d2b90f2b585ead37b34005997f551e3f7de26b194b09322f0df6e5246b6f86",
                baseToken = "0x6A383cf1F8897585718DCA629a8f1471339abFe4",
                totalBetSize = BigInteger.Parse("1000000000000000000000"),
                percentageOdds = BigInteger.Parse("51302654005356720000"),
                maker = new EthECKey(privateKey).GetPublicAddress(),
                executor = "0x3259E7Ccc0993368fCB82689F5C534669A0C06ca",
                isMakerBettingOutcomeOne = true,
                apiExpiry = DateTimeOffset.Now.AddSeconds(1000).ToUnixTimeSeconds(),
                salt = Order.Random32Bytes()
            };

            Console.WriteLine("Order hash: " + order.GetHash().ToHex(true));
            Console.WriteLine("Order signature: " + order.GetSignature(privateKey));

            var api = new SportXAPI { url = "https://mumbai.api.sportx.bet" };
            var newOrderResult = await api.PostNewOrder(order.GetSignedOrder(privateKey));
            Console.WriteLine("New order result: " + newOrderResult);

            var deserialized = JsonConvert.DeserializeAnonymousType(
                newOrderResult,
                new { status = default(string), data = new { orders = default(string[]) } }
            );
            Console.WriteLine("Deserialized new order result: " + deserialized);
            var cancelSignature = await Order.GetCancelSignature(
                deserialized.data.orders,
                privateKey,
                80001 // use 137 if mainnet. 80001 = mumbai testnet, 137 = polygon mainnet
            );
            Console.WriteLine("Cancel signature: " + cancelSignature);

            var cancelOrderResult = await api.CancelOrder(
                deserialized.data.orders,
                "Are you sure you want to cancel these orders",
                cancelSignature
            );
            Console.WriteLine("Cancel API call result: " + cancelOrderResult);
        }
    }
}
