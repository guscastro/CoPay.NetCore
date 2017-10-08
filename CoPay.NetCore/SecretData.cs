// { walletId: 'b4a56a6e-b409-4263-8478-3e9184b8983a',
//   walletPrivKey: <PrivateKey: 1c1c8b7500fe8e402c6c9d5736ce5ba00b6cbd0ecd714a82c157b506e9a82997, network: livenet>,
//   coin: 'btc',
//   network: 'livenet' }
using System;
using NBitcoin;
using System.Collections.Generic;
using System.Linq;

namespace CoPay
{
    public class SecretData
    {
        public Guid WalletId { get; set; }
        public Key WalletPrivateKey { get; set; }
        public string Coin { get; set; }
        public Network Network { get; set; }

        public static SecretData FromSecret(string secret)
        {
            try
            {
                var secretSplit = Split(secret, new int[] { 22, 74, 75 });
                var widBase58 = secretSplit[0].Replace("0", "");
                var base58Encoder = new NBitcoin.DataEncoders.Base58Encoder();
                var widHex = System.BitConverter.ToString(base58Encoder.DecodeData(widBase58)).Replace("-", string.Empty);
                var walletId = string.Join("-", Split(widHex, new int[] { 8, 12, 16, 20 }));
                var walletPrivKey = NBitcoin.Key.Parse(secretSplit[1]);
                var networkChar = secretSplit[2];
                var coin = (secretSplit.Count >= 4 ? secretSplit[3] : null) ?? "btc";

                return new SecretData {
                    WalletId = Guid.Parse(walletId),
                    WalletPrivateKey = walletPrivKey,
                    Coin = coin,
                    Network = networkChar == "T" ? Network.TestNet : Network.Main
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid secret", ex);
            }
        }

        private static List<string> Split(string secret, IEnumerable<int> indexes)
        {
            var parts = new List<string>();
            var indexesCopy = indexes.ToList();
            indexesCopy.Add(secret.Length);
            for (int i = 0; i < indexesCopy.Count; i++)
            {
                var startIndex = i == 0 ? 0 : indexesCopy[i - 1];
                var endIndex = indexesCopy[i];
                parts.Add(secret.Substring(startIndex, endIndex - startIndex));
            }
            return parts;
        } 
    }
}