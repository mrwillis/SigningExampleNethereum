using Nethereum.Signer.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using System.Numerics;
using System.Security.Cryptography;

namespace SigningExampleNethereum
{
    public static class CancelOrdersV1
    {
        private static readonly Eip712TypedDataSigner _signer = new Eip712TypedDataSigner();

        public static readonly string PROMPT = "Are you sure you want to cancel these orders";

        //Message types for easier input
        private class Details
        {
            [Parameter("string", "message", 1)]
            public string Message { get; set; }

            [Parameter("string[]", "orders", 2)]
            public List<string> Orders { get; set; }
        }

        //The generic Typed schema defintion for this message
        private static TypedData<Domain> GetTypedDefinition(BigInteger chainId)
        {
            return new TypedData<Domain>
            {
                Domain = new Domain
                {
                    Name = "CancelOrderSportX",
                    Version = "1.0",
                    ChainId = chainId,
                },
                Types = new Dictionary<string, MemberDescription[]>
                {
                    ["EIP712Domain"] = new[]
                    {
                        new MemberDescription {Name = "name", Type = "string"},
                        new MemberDescription {Name = "version", Type = "string"},
                        new MemberDescription {Name = "chainId", Type = "uint256"},
                    },
                    ["Details"] = new[]
                    {
                        new MemberDescription {Name = "message", Type = "string"},
                        new MemberDescription {Name = "orders", Type = "string[]"},
                    },
                },
                PrimaryType = "Details",
            };
        }

        public static string GetCancelOrdersEIP712Payload(List<string> orders, BigInteger chainId, EthECKey key)
        {
            var typedData = GetTypedDefinition(chainId);

            Console.WriteLine(typedData);
            Console.WriteLine(PROMPT);

            var mail = new Details
            {
                Message = PROMPT,
                Orders = orders
            };

            Console.WriteLine(mail.Orders.Count);
      
            var signature = _signer.SignTypedDataV4(mail, typedData, key);
            var addressRecovered = _signer.RecoverFromSignatureV4(mail, typedData, signature);
            var address = key.GetPublicAddress();
            return signature;
        }

    }
}
