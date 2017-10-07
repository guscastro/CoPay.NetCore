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

        public string requestPrivKey { get; set; }

        public string requestPubKey { get; set; }

        public Key walletPrivKey { get; set; }

        public string sharedEncryptingKey { get; set; }

        public string coin { get; set; }

        public uint account { get; set; }

        public string copayerName { get; set; }

        public static Credentials FromExtendedPrivateKey(
            string xPrivKey,
            string walletPrivKey,
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

            cred.walletPrivKey = Key.Parse(walletPrivKey, network);
            cred.sharedEncryptingKey = Utils.PrivateKeyToAESKey(walletPrivKey);

            var extkey = ExtKey.Parse(xPrivKey);
            var coinPath = network == Network.Main ? "0" : "1";
            var copayerPath = "m/44'/" + coinPath + "'/0'";
            var derivedCopayerKey = extkey.Derive(new KeyPath(copayerPath));

            cred.xPubKey = derivedCopayerKey.Neuter().ToString(network);

            var derivedRequestKey = extkey.Derive(Constants.REQUEST_PATH);
            cred.requestPrivKey = extkey.PrivateKey.ToString(network);
            cred.requestPubKey = extkey.PrivateKey.PubKey.ToString(network);

            return cred;
        }
    }
}
