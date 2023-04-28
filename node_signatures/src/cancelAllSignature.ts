import ethSigUtil from "eth-sig-util";

function getCancelAllOrdersEIP712Payload(
  salt: string,
  timestamp: number,
  chainId: number
) {
  const payload = {
    types: {
      EIP712Domain: [
        { name: "name", type: "string" },
        { name: "version", type: "string" },
        { name: "chainId", type: "uint256" },
        { name: "salt", type: "bytes32" },
      ],
      Details: [{ name: "timestamp", type: "uint256" }],
    },
    primaryType: "Details",
    domain: {
      name: "CancelAllOrdersSportX",
      version: "1.0",
      chainId,
      salt,
    },
    message: { timestamp },
  };
  return payload;
}

export = function getCancelAllOrdersSignature(
  callback: any,
  salt: string,
  timestamp: number,
  privateKey: string,
  chainId: number
) {
  const payload = getCancelAllOrdersEIP712Payload(salt, timestamp, chainId);
  const bufferPrivateKey = Buffer.from(privateKey.substring(2), "hex");
  const signature = (ethSigUtil as any).signTypedData_v4(bufferPrivateKey, {
    data: payload,
  });
  callback(null, signature);
};
