using Newtonsoft.Json;
using System.Text;

namespace NethereumSample
{
    class SportXAPI
    {
        static readonly HttpClient client = new HttpClient();

        public string url;

        public async Task<string> PostNewOrder(SignedOrder order)
        {
            var obj = new {orders = new SignedOrder[] {order}};
            var serialized = JsonConvert.SerializeObject(obj);
            Console.WriteLine(serialized);
            var httpContent = new StringContent(serialized, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(
                url + "/orders/new",
                httpContent
            );
            if (response.Content != null)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("Content is null");
            }
        }

        public async Task<string> CancelOrder(
            string[] orderHashes,
            string message,
            string signature
        )
        {
            var obj = new { orders = orderHashes, message, cancelSignature = signature };
            var serialized = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(serialized, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(
                url + "/orders/cancel",
                httpContent
            );
            if (response.Content != null)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("Content is null");
            }
        }
    }
}
