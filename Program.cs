using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;

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
            var marketHash = "0x2f89ac57b87363c7e0e88e67aff7424bf977605de87285485181da598fd1f5c7";
            var baseToken = "0x6A383cf1F8897585718DCA629a8f1471339abFe4";
            var totalBetSize = BigInteger.Parse("1000000000000000000000");
            var percentageOdds = BigInteger.Parse("51302654005356720000");
            var expiry = BigInteger.Parse("2209006800");
            var salt = BigInteger.Parse(
                "11088371513674221491423763797610298526004404436355568229714515396917752811892"
            );
            var maker = "0x9883D5e7dC023A441A01Ef95aF406C69926a0AB6";
            var executor = "0x3259E7Ccc0993368fCB82689F5C534669A0C06ca";
            var isMakerBettingOutcomeOne = true;
            var order = new Order(
                marketHash,
                baseToken,
                totalBetSize,
                percentageOdds,
                maker,
                executor,
                isMakerBettingOutcomeOne
            );
            Console.WriteLine("Order hash: " + order.GetHash().ToHex(true));
            Console.WriteLine("Order signature: " + order.GetSignature(privateKey));
            var result = await Order.GetCancelSignature(
                new string[]
                {
                    "0x2bceef90e74501b1aeed3ef791be02e9f88b42a7edb94943ce2f64d15bdaed7e",
                    "0x09ab58e1d404063ae36783cecf9dd3e758075bef891d3962a26035be008ad02f"
                },
                privateKey
            );
            Console.WriteLine("Cancel signature: " + result);
        }
    }
}
