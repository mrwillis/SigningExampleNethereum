using Nethereum.Signer.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using System.Numerics;

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
        private static TypedData<DomainWithNameVersionAndChainId> GetTypedDefinition(BigInteger chainId)
        {
            return new TypedData<DomainWithNameVersionAndChainId>
            {
                Domain = new DomainWithNameVersionAndChainId
                {
                    Name = "CancelOrderSportX",
                    Version = "1.0",
                    ChainId = chainId,
                },
                Types = MemberDescriptionFactory.GetTypesMemberDescription(typeof(DomainWithNameVersionAndChainId), typeof(Details)),
                PrimaryType = nameof(Details),
            };
        }

        public static string GetCancelOrdersEIP712Payload(List<string> orders, BigInteger chainId, EthECKey key)
        {
            var typedData = GetTypedDefinition(chainId);

            var mail = new Details
            {
                Message = PROMPT,
                Orders = orders
            };
      
            return _signer.SignTypedDataV4(mail, typedData, key);
        }

    }
}
