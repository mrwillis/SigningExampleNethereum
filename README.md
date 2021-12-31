# SportX.bet C# examples

This repo contains some examples for signing various order operations on sportx.bet using C#. Currently has examples for the following:

1. Signing orders as a market maker using Nethereum. Required [here](https://sportx-bet.github.io/slate/#post-a-new-order).
2. Signing cancellation requests as a market maker (using a Node.JS bridge). Required [here](https://sportx-bet.github.io/slate/#cancel-orders).

Supports the following targets:

- .NET Standard 2.0
- .NET Framework 4.6.1
- .NET Core 3.1
- .NET 5.0
- .NET 6.0

## Setup

For the cancellation requests to work, you'll need to call a JavaScript file using Node.JS. Unfortunately at the moment Nethereum does not support the full suite of EIP712 Signing (https://github.com/Nethereum/Nethereum/pull/731), so for now we can simply start up a child Node.JS process and offload the work there via HTTP.

*Ensure you have Node 12+ intalled.*

To set it up, simply 

```bash
dotnet restore
cd node_signatures
npm i
npm run compile
```

Edit the file in `Order.cs` to point to the location of the .js file in the `dist` folder under `node_signatures` in the `GlobalVar` static class.

## Run

`dotnet run`

## Performance

Speed is very fast after an initial signing which starts up the process. Consider during the set-up of your bot to first sign a dummy order hash to start the process and cache the file. 

Signing speed is on the order of 4ms, which is definitely much slower than native code, but should be fine for most purposes until Nethereum supports the ful EIP712 suite.


