using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.ABI.EIP712
{
    [Struct("EIP712Domain")]
    public class SaltDomain : Domain
    {
        [Parameter("bytes32", "salt", 4)]
        public virtual BigInteger Salt { get; set; }
    }
}