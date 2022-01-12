import ethSigUtil from "eth-sig-util";

function getCancelOrderEIP712Payload(orders: string[], chainId: number) {
  const payload = {
    types: {
      EIP712Domain: [
        { name: "name", type: "string" },
        { name: "version", type: "string" },
        { name: "chainId", type: "uint256" },
      ],
      Details: [
        { name: "message", type: "string" },
        { name: "orders", type: "string[]" },
      ],
    },
    primaryType: "Details",
    domain: {
      name: "CancelOrderSportX",
      version: "1.0",
      chainId,
    },
    message: {
      orders,
      message: "Are you sure you want to cancel these orders",
    },
  };
  return payload;
}

export = function getCancelOrderSignature(
  callback: any,
  orders: string[],
  privateKey: string,
  chainId: number
) {
  const payload = getCancelOrderEIP712Payload(orders, chainId);
  const bufferPrivateKey = Buffer.from(privateKey.substring(2), "hex");
  const signature = (ethSigUtil as any).signTypedData_v4(bufferPrivateKey, {
    data: payload,
  });
  callback(null, signature);
};
