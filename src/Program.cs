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
            var privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY_NO_PREFIX");
            if (privateKey == null)
            {
                throw new Exception("PRIVATE_KEY_NO_PREFIX is not defined");
            }
            var chainIdRaw = Environment.GetEnvironmentVariable("CHAIN_ID");
            if (chainIdRaw == null) {
                throw new Exception("CHAIN_ID is not defined");
            }
            var chainId = Int32.Parse(chainIdRaw);

            EthECKey wallet = new EthECKey(privateKey);
            Console.WriteLine("Wallet address: " + wallet.GetPublicAddress());


            // var order = new Order
            // {
            //     marketHash = "0x88c43af903e373ec2b2717813f6c42188aeb2d818a56eda46b6d63a08ad1e406",
            //     baseToken = "0x5147891461a7C81075950f8eE6384e019e39ab98",
            //     totalBetSize = BigInteger.Parse("100000000"),
            //     percentageOdds = BigInteger.Parse("50000000000000000000"),
            //     maker = wallet.GetPublicAddress(),
            //     executor = "0x3259E7Ccc0993368fCB82689F5C534669A0C06ca",
            //     isMakerBettingOutcomeOne = true,
            //     apiExpiry = DateTimeOffset.Now.AddSeconds(1000).ToUnixTimeSeconds(),
            //     salt = Order.Random32Bytes()
            // };

            // Console.WriteLine("Order hash: " + order.GetHash().ToHex(true));
            // Console.WriteLine("Order signature: " + order.GetSignature(privateKey));

            // var api = new SXBetApi { url = "http://localhost:8080" };
            // var newOrderResult = await api.PostNewOrder(order.GetSignedOrder(privateKey));
            // Console.WriteLine("New order result: " + newOrderResult);

            // var deserialized = JsonConvert.DeserializeAnonymousType(
            //     newOrderResult,
            //     new { status = default(string), data = new { orders = default(string[]) } }
            // );
            // Console.WriteLine("Deserialized new order result: " + deserialized);

            // var ordersList = new List<string>(deserialized.data.orders);

            // var cancelSignature = CancelOrdersV1.GetCancelOrdersEIP712Payload(ordersList, chainId, wallet);

            // Console.WriteLine("Cancel signature: " + cancelSignature);

            // var cancelOrderResult = await api.CancelOrder(
            //     deserialized.data.orders,
            //     CancelOrdersV1.PROMPT,
            //     cancelSignature
            // );
            // Console.WriteLine("Cancel API call result: " + cancelOrderResult);

            var salt = Utilities.Random32Bytes();

            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            var cancelAllOrdersSignature = CancelAllOrders.GetCancelAllOrdersEIP712Payload(chainId, timestamp, wallet, salt);
            var jsCancelAllOrdersSignature = await Order.GetCancelAllSignature(salt.ToHex(), timestamp.ToString(), wallet.GetPrivateKey(), chainId);
            Console.WriteLine("Cancel all orders signature (c#): " + cancelAllOrdersSignature);
            Console.WriteLine("Cancel all orders signature (js): " + jsCancelAllOrdersSignature);

            // var cancelAllOrdersResult = await api.CancelAllOrders(cancelAllOrdersSignature, salt.ToString(), wallet.GetPublicAddress(), timestamp);
            // Console.WriteLine("Cancel all API call result: " + cancelAllOrdersResult);
        }
    }
}
