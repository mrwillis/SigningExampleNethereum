using Nethereum.Signer.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using System.Numerics;

namespace SigningExampleNethereum
{
    public static class CancelAllOrders
    {

        private static readonly Eip712TypedDataSigner _signer = new Eip712TypedDataSigner();

        //Message types for easier input
        private class Details
        {
            [Parameter("uint256", "timestamp", 1)]
            public BigInteger Timestamp { get; set; }
        }

        //The generic Typed schema defintion for this message
        private static TypedData<SaltDomain> getTypedDefinition(byte[] salt, BigInteger chainId)
        {
            return new TypedData<SaltDomain>
            {
                Domain = new SaltDomain
                {
                    Name = "CancelAllOrdersSportX",
                    Version = "1.0",
                    ChainId = chainId,
                    Salt = salt
                },
                Types = MemberDescriptionFactory.GetTypesMemberDescription(typeof(SaltDomain), typeof(Details)),
                PrimaryType = nameof(Details),
            };
        }

        public static string GetCancelAllOrdersEIP712Payload(BigInteger chainId, BigInteger timestamp, EthECKey key, byte[] salt)
        {
            var typedData = getTypedDefinition(salt, chainId);

            var details = new Details
            {
                Timestamp = timestamp
            };
      
            return _signer.SignTypedDataV4(details, typedData, key);
        }
    }
}
