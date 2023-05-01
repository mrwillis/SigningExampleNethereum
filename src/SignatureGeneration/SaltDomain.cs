using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Nethereum.ABI.EIP712
{
    [Struct("EIP712Domain")]
    public class SaltDomain : IDomain
    {

        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; }

        [Parameter("string", "version", 2)]
        public virtual string Version { get; set; }

        [Parameter("uint256", "chainId", 3)]
        public virtual BigInteger? ChainId { get; set; }

        [Parameter("bytes32", "salt", 4)]
        public virtual byte[] Salt { get; set; }
    }
}