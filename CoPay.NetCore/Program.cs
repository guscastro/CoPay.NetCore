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

        static string JoinWallet(string copayerXPrivKey = null) {
            if (copayerXPrivKey == null) {
                var newCopayerKey = new NBitcoin.ExtKey();
                copayerXPrivKey = newCopayerKey.ToString(NBitcoin.Network.Main);
            }
            var walletId = Guid.Parse("5f8f1c96-1e8b-481c-9b0b-febbcbdc47c3");
            var walletPrivateKey = "L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh";
            var copayerName = "gus3";
            // var pubkey = "0254fea7b08745c15103765a5c299b354ac6fbd3fa6a33c5ee84b6fa0fd108ab4e";
            // var xPubKey = "xpub661MyMwAqRbcGUAtmn55urDkFGxWFsFf6tJphcdLFvYTYyd45qS4TrqC69eswNyE7Zf3tCtQn29vhy3TjAv75GoigSyNVS5tjcnckt2nczf";
            // var xPrivKey = "xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC";
            var client = new CoPay.Client();
            var task = client.doJoinWallet(
                walletId,
                walletPrivateKey,
                copayerXPrivKey,
                copayerName
            );

            task.Wait();

            Console.Out.WriteLine(task.Result);

            return task.Result.copayerId;
        }

        static void CreateTxProposal()
        {
            var newCopayerKey = new NBitcoin.ExtKey();
            var copayerXPrivKey = newCopayerKey.ToString(NBitcoin.Network.Main);
            Console.WriteLine("copayerXPrivKey");
            Console.WriteLine(copayerXPrivKey);
            var copayerId = JoinWallet(copayerXPrivKey);
            var walletPrivateKey = "L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh";
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

            var client = new CoPay.Client();
            var task = client.createTransactionProposal(opts, walletPrivateKey, copayerId, copayerXPrivKey);

            task.Wait();

            Console.Out.WriteLine(task.Result);
        }

        static void Generate() {
            var k = new NBitcoin.ExtKey();
            Console.Out.WriteLine("Ext priv");
            Console.Out.WriteLine(k.ToString(NBitcoin.Network.Main));
            
            Console.Out.WriteLine("Ext pub");
            Console.Out.WriteLine(k.Neuter());

            var pk = k.PrivateKey;
            Console.Out.WriteLine("PrivK");
            Console.Out.WriteLine(pk.ToString(NBitcoin.Network.Main));

            Console.Out.WriteLine("Public");
            Console.Out.WriteLine(pk.PubKey.ToString());
        }

        static void Parse() {
            var k = NBitcoin.ExtKey.Parse("xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC", Network.Main);
            Console.Out.WriteLine("Priv");
            Console.Out.WriteLine(k.PrivateKey.ToString(Network.Main));
            Console.Out.WriteLine("Pub");
            Console.Out.WriteLine(k.PrivateKey.PubKey.ToString());
            Console.Out.WriteLine("Pub");
            Console.Out.WriteLine("Derived");
            var der = k.Derive(Constants.REQUEST_PATH);
            Console.Out.WriteLine(der.ToString(Network.Main));
            Console.Out.WriteLine(der.PrivateKey.ToString(Network.Main));
            Console.Out.WriteLine(der.PrivateKey.PubKey.ToString());
        }
    }
}
