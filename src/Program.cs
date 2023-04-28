using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;
using Nethereum.Signer;
using Newtonsoft.Json;

namespace SigningExampleNethereum
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello");
            var privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
            if (privateKey == null)
            {
                throw new Exception("PRIVATE_KEY is not defined");
            }
            var chainIdRaw = Environment.GetEnvironmentVariable("CHAIN_ID");
            if (chainIdRaw == null) {
                throw new Exception("CHAIN_ID is not defined");
            }
            var chainId = Int32.Parse(chainIdRaw);

            EthECKey wallet = new EthECKey(privateKey);
            Console.WriteLine("Wallet address: " + wallet.GetPublicAddress());


            var order = new Order
            {
                marketHash = "0x48ee699e68f564a1bfc2ebff1096a6c966033d197a4ea0f09e847efb6d0df97b",
                baseToken = "0x5147891461a7C81075950f8eE6384e019e39ab98",
                totalBetSize = BigInteger.Parse("100000000"),
                percentageOdds = BigInteger.Parse("50000000000000000000"),
                maker = wallet.GetPublicAddress(),
                executor = "0x3259E7Ccc0993368fCB82689F5C534669A0C06ca",
                isMakerBettingOutcomeOne = true,
                apiExpiry = DateTimeOffset.Now.AddSeconds(1000).ToUnixTimeSeconds(),
                salt = Order.Random32Bytes()
            };

            Console.WriteLine("Order hash: " + order.GetHash().ToHex(true));
            Console.WriteLine("Order signature: " + order.GetSignature(privateKey));

            var api = new SXBetApi { url = "https://api.toronto.sx.bet" };
            var newOrderResult = await api.PostNewOrder(order.GetSignedOrder(privateKey));
            Console.WriteLine("New order result: " + newOrderResult);

            var deserialized = JsonConvert.DeserializeAnonymousType(
                newOrderResult,
                new { status = default(string), data = new { orders = default(string[]) } }
            );
            Console.WriteLine("Deserialized new order result: " + deserialized);

            var ordersList = new List<string>(deserialized.data.orders);

            ordersList.ForEach(Console.WriteLine);

            var cancelSignature = CancelOrdersV1.GetCancelOrdersEIP712Payload(ordersList, chainId, wallet);

            // var cancelSignature = await Order.GetCancelSignature(
            //     deserialized.data.orders,
            //     privateKey,
            //     chainId // use 137 if mainnet. 80001 = mumbai testnet, 137 = polygon mainnet
            // );
            Console.WriteLine("Cancel signature: " + cancelSignature);

            var cancelOrderResult = await api.CancelOrder(
                deserialized.data.orders,
                CancelOrdersV1.PROMPT,
                cancelSignature
            );
            Console.WriteLine("Cancel API call result: " + cancelOrderResult);
        }
    }
}
