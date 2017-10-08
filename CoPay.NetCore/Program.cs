using System;
using System.Collections.Generic;
using NBitcoin;

namespace CoPay.NetCore
{
    class Program
    {
        static Client client;

        static void Main(string[] args)
        {
            client = new Client();
            client.credentials = Credentials.FromTestCredentials();
            // client.credentials = Credentials.Create("gus", Network.TestNet);

            Console.WriteLine("credentials");
            Console.WriteLine(client.credentials.walletPrivKey.ToString(Network.TestNet));

            // var walletId = CreateWallet();
            // JoinWallet(walletId);
            // RequestNewAddress();
            // GetWalletAddresses();
            CreateTxProposal();
        }

        static Guid CreateWallet()
        {
            var task = client.createWallet("gus", "gus", 1, 1);
            task.Wait();
            return task.Result.walletId;
        }

        static void JoinWallet(Guid walletId)
        {
            var task = client.doJoinWallet(walletId);

            task.Wait();

            Console.Out.WriteLine(task.Result);

            client.credentials.copayerId = task.Result.copayerId;
        }

        static void RequestNewAddress()
        {
            var task = client.RequestNewAddress();
            task.Wait();
        }

        static void GetWalletAddresses()
        {
            var task = client.GetWalletAddresses();
            task.Wait();
        }

        static void CreateTxProposal()
        {
            var opts = new TransactionProposal.Options
            {
                txProposalId = Guid.NewGuid().ToString(),
                outputs = new List<TransactionProposal.Options.Output>
                {
                    new TransactionProposal.Options.Output
                    {
                        toAddress = "mqmc1EiKHHoi1dqZQgstfAAj8qajpVwagf",
                        amount = 10000,
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
