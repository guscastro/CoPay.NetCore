using System;
using System.Collections.Generic;
using NBitcoin;

namespace CoPay.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = SecretData.FromSecret("9TaJJxTQ6uFYg4kabVag98KyF37KpgttZLrn2DtWj9LuwQi53urzHX4xAGJo6WU5VyoP2KH2XETbtc");

            var signingClient = new Client();
            // client.credentials = Credentials.FromTestCredentials();
            signingClient.credentials = Credentials.Create("gus", Network.TestNet);
            signingClient.credentials.PopulateSecretData(s);

            var copayerClient = new Client();
            copayerClient.credentials = Credentials.Create("gus2", Network.TestNet);
            copayerClient.credentials.PopulateSecretData(s);

            var walletId = CreateWallet(signingClient);
            JoinWallet(signingClient, walletId);
            JoinWallet(copayerClient, walletId);
            RequestNewAddress(signingClient);
            // GetWalletAddresses();
            // CreateTxProposal();
            SubscribeToNotifications(signingClient, "TODO get from app");
        }

        static Guid CreateWallet(Client client)
        {
            var task = client.createWallet("gus", "gus", 1, 2);
            task.Wait();
            return task.Result.walletId;
        }

        static void JoinWallet(Client client, Guid walletId)
        {
            var task = client.doJoinWallet(walletId);

            task.Wait();

            Console.Out.WriteLine(task.Result);

            client.credentials.copayerId = task.Result.copayerId;
        }

        static void RequestNewAddress(Client client)
        {
            var task = client.RequestNewAddress();
            task.Wait();
        }

        static void GetWalletAddresses(Client client)
        {
            var task = client.GetWalletAddresses();
            task.Wait();
        }

        static void CreateTxProposal(Client client)
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

        static void SubscribeToNotifications(Client client, string token, string deviceType = "ios")
        {
            var task = client.SubscribeToNotifications(deviceType, token);
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
