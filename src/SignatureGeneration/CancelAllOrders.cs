using Nethereum.Signer.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using System.Numerics;
using System.Security.Cryptography;

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
        private static TypedData<SaltDomain> getTypedDefinition(BigInteger salt, BigInteger chainId)
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
                Types = new Dictionary<string, MemberDescription[]>
                {
                    ["EIP712Domain"] = new[]
                    {
                        new MemberDescription {Name = "name", Type = "string"},
                        new MemberDescription {Name = "version", Type = "string"},
                        new MemberDescription {Name = "chainId", Type = "uint256"},
                        new MemberDescription {Name = "salt", Type = "bytes32"},
                    },
                    ["Details"] = new[]
                    {
                        new MemberDescription {Name = "timestamp", Type = "uint256"},
                    },
                },
                PrimaryType = "Details",
            };
        }

        private static string getCancelAllOrdersEIP712Payload(BigInteger chainId, BigInteger timestamp, EthECKey key)
        {
            var typedData = getTypedDefinition(Utilities.Random32Bytes(), chainId);

            var mail = new Details
            {
                Timestamp = timestamp
            };
      
            var signature = _signer.SignTypedDataV4(mail, typedData, key);
            var addressRecovered = _signer.RecoverFromSignatureV4(mail, typedData, signature);
            var address = key.GetPublicAddress();
            return signature;
        }

    }
}
