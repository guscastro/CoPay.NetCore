using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace CoPay
{
    public class Credentials
    {
        public Network network { get; set; }

        public string xPrivKey { get; set; }

        public string xPubKey { get; set; }

        public string copayerId { get; set; }

        public Key requestPrivKey { get; set; }

        public string requestPubKey { get; set; }

        public Guid WalletId { get; set; }

        public string WalletAddress { get; set; }

        public Key walletPrivKey { get; set; }

        public string walletPubKey { get; set; }

        public string sharedEncryptingKey { get; set; }

        public string coin { get; set; }

        public uint account { get; set; }

        public string copayerName { get; set; }

        public void PopulateSecretData(SecretData secretData)
        {
            this.WalletId = secretData.WalletId;
            this.AddWalletPrivateKey(secretData.WalletPrivateKey);
            this.coin = secretData.Coin;
            this.network = secretData.Network;
        }

        public void AddWalletPrivateKey(Key walletPrivKey)
        {
            this.walletPrivKey = walletPrivKey;
            this.walletPubKey = walletPrivKey.PubKey.ToHex();
            this.sharedEncryptingKey = Utils.PrivateKeyToAESKey(walletPrivKey.ToString(network));
        }
        public static Credentials FromExtendedPrivateKey(
            string xPrivKey,
            Key walletPrivKey,
            string copayerName,
            Network network,
            string coin = "btc",
            uint account = 0
        ) {
            var cred = new Credentials();

            cred.xPrivKey = xPrivKey;
            cred.coin = coin;
            cred.account = account;
            cred.copayerName = copayerName;
            cred.network = network;

            cred.AddWalletPrivateKey(walletPrivKey);

            var extkey = ExtKey.Parse(xPrivKey);
            var coinPath = network == Network.Main ? "0" : "1";
            var copayerPath = "m/44'/" + coinPath + "'/0'";
            var derivedCopayerKey = extkey.Derive(new KeyPath(copayerPath));

            cred.xPubKey = derivedCopayerKey.Neuter().ToString(network);

            var derivedRequestKey = extkey.Derive(Constants.REQUEST_PATH);
            cred.requestPrivKey = extkey.PrivateKey;
            cred.requestPubKey = extkey.PrivateKey.PubKey.ToHex();

            cred.copayerId = Utils.XPubKeyToCopayerId(cred.xPubKey);

            return cred;
        }

        public static Credentials Create(string copayerName, Network network)
        {
            var newCopayerKey = new NBitcoin.ExtKey();
            var walletKey = new NBitcoin.Key();
            var copayerXPrivKey = newCopayerKey.ToString(network);
            return Credentials.FromExtendedPrivateKey(copayerXPrivKey, walletKey, copayerName, network);
        }

        public static Credentials FromTestCredentials()
        {
            var cred = Credentials.FromExtendedPrivateKey(
                "tprv8ZgxMBicQKsPcxukSA32PC1SSEzng3oGsud3VRrougq7e5UEp6yCF4RXMLFx7HMhTDKBacpPS2KQeQJkeQWkSi9Eu12oN12eV1cHS1vTPKb",
                Key.Parse("cViJGiPK63CNYQbGhZdfphovh1iz3hjRJgoeHCk6YKtemaVhnJBp"),
                "gus",
                Network.TestNet
            );

            cred.WalletId = Guid.Parse("3a015f4f-1416-484b-85c4-9c90490e7be3");
            cred.WalletAddress = "mu9kXtgtXUFPk8kBtqQn8qVD3tFGLQcbZc";
            cred.copayerId = "13d3de948a85c8ae3d0086b2964eaaa032ab210329e2862583b00dc1ddacdefd";

            return cred;
        }
    }
}
