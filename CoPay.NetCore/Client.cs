using NBitcoin;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoPay
{
    public class Client
    {
        // TODO get from settings or something?
        private readonly Network network = Network.Main;
        public const string BWS_INSTANCE_URL = "https://bws.bitpay.com/bws/api";

        private readonly string baseUrl;

        public Client(String baseUrl = BWS_INSTANCE_URL)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<string> createWallet(String walletName, String copayerName, Int16 m, Int16 n, opts opts)
        {
            Cred cred = new Cred()
            {
                copayerId = copayerName,
                network = "testnet",
                requestPrivKey = "tprv8dxkXXLevuHXR3tLvBkaDLyCnQxsQQVafnDMEQNds8r8tjSPfNTGD5ShtpP8QeTdtCoWGmrMC5gs9j7ap8ATdSsAD2KCv87BGdzPWwmdJt2",
                xPrivKey = "cNaQCDwmmh4dS9LzCgVtyy1e1xjCJ21GUDHe9K98nzb689JvinGV"
            };

            CreateWallet.Request request = new CreateWallet.Request()
            {
                m = m,
                n = n,
                name = walletName,
                pubKey = "02fcba7ecf41bc7e1be4ee122d9d22e3333671eb0a3a87b5cdf099d59874e1940f",
                network = "testnet"
            };

            String url = this.baseUrl + "/v2/wallets/";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-identity", cred.copayerId);

            String json = JsonConvert.SerializeObject(request);
            StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage responseMessage = await client.PostAsync(url, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();

                    CreateWallet.Response response = JsonConvert.DeserializeObject<CreateWallet.Response>(responseContent);
                    String share = buildSecret(response.walletId, cred.xPrivKey, "testnet");
                    return share;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public async Task<JoinWallet.Response> doJoinWallet(
            Guid walletId,
            String walletPrivKey,
            String copayerXPrivKey,
            String copayerName
        ) {
            var copayerKey = ExtKey.Parse(copayerXPrivKey);
            var derivedKey = copayerKey.Derive(Constants.REQUEST_PATH);

            var xPubKey = copayerKey.Neuter().ToString(this.network);
            var reqPubKey = derivedKey.PrivateKey.PubKey.ToString(this.network);
            var reqPrivKey = derivedKey.PrivateKey.ToString(this.network);

            Console.WriteLine("join reqpriv");
            Console.WriteLine(reqPrivKey);
 
            var sharedEncryptingKey = Utils.PrivateKeyToAESKey(walletPrivKey);
            
            var encCopayerName = Utils.encryptMessage(copayerName, sharedEncryptingKey);

            var hash = Utils.getCopayerHash(copayerName, xPubKey, reqPubKey);

            var wkey = Key.Parse(walletPrivKey, this.network);
            var copayerSignature = Utils.signMessage(hash, wkey);

            var request = new JoinWallet.Request {
                walletId = walletId.ToString(),
                name = copayerName,
                xPubKey = xPubKey,
                requestPubKey = reqPubKey,
                copayerSignature = copayerSignature
            };
            
            var url = this.baseUrl + String.Format("/v2/wallets/{0}/copayers", walletId);
            var copayerId = Utils.XPubKeyToCopayerId(xPubKey);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-identity", copayerId);

            var json = JsonConvert.SerializeObject(request);
            Console.Out.WriteLine(json);
            StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage responseMessage = await client.PostAsync(url, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();
                    Console.Out.WriteLine(responseContent);
                    return JsonConvert.DeserializeObject<JoinWallet.Response>(responseContent);
                }
                else
                {
                    Console.Out.WriteLine("Error");
                    Console.Out.WriteLine(responseMessage.StatusCode);
                    Console.Out.WriteLine(await responseMessage.Content.ReadAsStringAsync());
                    throw new InvalidOperationException();
                }
            }
        }

        public async Task<TransactionProposal.Response> createTransactionProposal(
            TransactionProposal.Options opts,
            string walletPrivKey,
            string copayerId,
            string copayerXPrivKey
        ) {
            var args = opts.DeepClone();

            var copayerKey = ExtKey.Parse(copayerXPrivKey);
            var derivedKey = copayerKey.Derive(Constants.REQUEST_PATH);

            var reqPrivKey = derivedKey.PrivateKey;
            var sharedEncryptingKey = Utils.PrivateKeyToAESKey(walletPrivKey);

            Console.WriteLine("tx reqpriv");
            Console.WriteLine(reqPrivKey.ToString(this.network));

            args.message = Utils.encryptMessage(args.message, sharedEncryptingKey);
            // args.message = "{\"iv\":\"eOaM99CkJ8TRQzGa6M3iNQ==\",\"v\":1,\"iter\":1,\"ks\":128,\"ts\":64,\"mode\":\"ccm\",\"adata\":\"\",\"cipher\":\"aes\",\"ct\":\"OLvkOOOyKM9ZDTEhRSEFG+qX1Z1Z\"}";

            foreach (var output in args.outputs)
            {
                output.message = output.message ?? Utils.encryptMessage(output.message, sharedEncryptingKey);
            }
            
            var relativeUrl = "/v2/txproposals";
            var url = this.baseUrl + relativeUrl;
            var reqSignature = Utils.signRequest("post", relativeUrl, args, reqPrivKey);

            var client = new HttpClient();
            // client.DefaultRequestHeaders.Add("x-identity", copayerId);
            client.DefaultRequestHeaders.Add("x-identity", "a308c35633a9f116721cc8ecdd6261c806ac823d9ed90f9257834d8813b3f9d0");
            client.DefaultRequestHeaders.Add("x-signature", reqSignature);
            // client.DefaultRequestHeaders.Add("x-signature", "30450221009b691ee3a86fcbe1112e07efcf14e4e6fd45780d074f4a9a203f5d7fc14693b3022070556748e95f310bdae56058331a74f76ea342464c60088460093d3c46ad1e1a");

            var json = JsonConvert.SerializeObject(args);
            Console.Out.WriteLine(json);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var responseMessage = await client.PostAsync(url, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();
                    Console.Out.WriteLine(responseContent);
                    return JsonConvert.DeserializeObject<TransactionProposal.Response>(responseContent);
                }
                else
                {
                    Console.Out.WriteLine("Error");
                    Console.Out.WriteLine(responseMessage.StatusCode);
                    Console.Out.WriteLine(await responseMessage.Content.ReadAsStringAsync());
                    throw new InvalidOperationException();
                }
            }
        }

        private static string buildSecret(Guid walletId, String walletPrivKey, string network)
        {
            string widHx = walletId.ToString("N");

            string widBase58 = Encoders.Base58.EncodeData(Encoders.Hex.DecodeData(widHx));

            return widBase58 + walletPrivKey + "L";
        }
    }
}
