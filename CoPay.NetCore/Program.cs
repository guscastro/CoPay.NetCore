using System;
using System.Collections.Generic;
using NBitcoin;

namespace CoPay.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // var k = NBitcoin.Key.Parse("KwGv4AGwwjfgxfZnkSM7xB9ZqMAxq2LvhELxBvVCWGxzAa1kuYKV");
            // Console.WriteLine(k.PubKey.ToString(NBitcoin.Network.TestNet));
            // Console.WriteLine(k.PubKey.ToString(NBitcoin.Network.Main));
            CreateTxProposal();
        }

        static Client JoinWallet(string copayerXPrivKey = null) {
            if (copayerXPrivKey == null) {
                var newCopayerKey = new NBitcoin.ExtKey();
                copayerXPrivKey = newCopayerKey.ToString(NBitcoin.Network.TestNet);
            }
            var walletId = Guid.Parse("77534c41-c185-4a0f-8a39-ec0607d69394");
            var walletPrivateKey = "L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh";
            var copayerName = "gus3";
            // var pubkey = "0254fea7b08745c15103765a5c299b354ac6fbd3fa6a33c5ee84b6fa0fd108ab4e";
            // var xPubKey = "xpub661MyMwAqRbcGUAtmn55urDkFGxWFsFf6tJphcdLFvYTYyd45qS4TrqC69eswNyE7Zf3tCtQn29vhy3TjAv75GoigSyNVS5tjcnckt2nczf";
            // var xPrivKey = "xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC";
            var client = new CoPay.Client();
            client.SetCredentials(copayerXPrivKey, walletPrivateKey, copayerName);

            var task = client.doJoinWallet(walletId);

            task.Wait();

            Console.Out.WriteLine(task.Result);

            client.credentials.copayerId = task.Result.copayerId;

            return client;
        }

        static void CreateTxProposal()
        {
            var newCopayerKey = new NBitcoin.ExtKey();
            var copayerXPrivKey = newCopayerKey.ToString(NBitcoin.Network.TestNet);
            Console.WriteLine("copayerXPrivKey");
            Console.WriteLine(copayerXPrivKey);
            var client = JoinWallet(copayerXPrivKey);
            var opts = new TransactionProposal.Options
            {
                txProposalId = Guid.NewGuid().ToString(),
                outputs = new List<TransactionProposal.Options.Output>
                {
                    new TransactionProposal.Options.Output
                    {
                        toAddress = "TODO",
                        amount = 10,
                        message = "Hello Output"
                    }
                },
                message = "Hello Options"
            };

            var task = client.createTransactionProposal(opts);

            task.Wait();

            Console.Out.WriteLine(task.Result);
        }

        static void Generate() {
            var k = new NBitcoin.ExtKey();
            Console.Out.WriteLine("Ext priv");
            Console.Out.WriteLine(k.ToString(NBitcoin.Network.TestNet));
            
            Console.Out.WriteLine("Ext pub");
            Console.Out.WriteLine(k.Neuter());

            var pk = k.PrivateKey;
            Console.Out.WriteLine("PrivK");
            Console.Out.WriteLine(pk.ToString(NBitcoin.Network.TestNet));

            Console.Out.WriteLine("Public");
            Console.Out.WriteLine(pk.PubKey.ToString());
        }

        static void Parse() {
            var k = NBitcoin.ExtKey.Parse("xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC", Network.TestNet);
            Console.Out.WriteLine("Priv");
            Console.Out.WriteLine(k.PrivateKey.ToString(Network.TestNet));
            Console.Out.WriteLine("Pub");
            Console.Out.WriteLine(k.PrivateKey.PubKey.ToString());
            Console.Out.WriteLine("Pub");
            Console.Out.WriteLine("Derived");
            var der = k.Derive(Constants.REQUEST_PATH);
            Console.Out.WriteLine(der.ToString(Network.TestNet));
            Console.Out.WriteLine(der.PrivateKey.ToString(Network.TestNet));
            Console.Out.WriteLine(der.PrivateKey.PubKey.ToString());
        }
    }
}
