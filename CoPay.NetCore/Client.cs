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

        public Credentials credentials;

        private readonly string baseUrl;

        public Client(String baseUrl = BWS_INSTANCE_URL)
        {
            this.baseUrl = baseUrl;
        }

        public void SetCredentials(string xPrivKey, string walletPrivKey, string copayerName)
        {
            this.credentials = Credentials.FromExtendedPrivateKey(xPrivKey, walletPrivKey, copayerName, this.network);
        }

        public async Task<CreateWallet.Response> createWallet(String walletName, String copayerName, Int16 m, Int16 n, opts opts)
        {
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

            String json = JsonConvert.SerializeObject(request);
            StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage responseMessage = await client.PostAsync(url, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<CreateWallet.Response>(responseContent);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public async Task<JoinWallet.Response> doJoinWallet(Guid walletId)
        {
            var encCopayerName = Utils.encryptMessage(this.credentials.copayerName, this.credentials.sharedEncryptingKey);

            var hash = Utils.getCopayerHash(encCopayerName, this.credentials.xPubKey, this.credentials.requestPubKey);
            var copayerSignature = Utils.signMessage(hash, this.credentials.walletPrivKey);

            var request = new JoinWallet.Request {
                walletId = walletId.ToString(),
                name = encCopayerName,
                xPubKey = this.credentials.xPubKey,
                requestPubKey = this.credentials.requestPubKey,
                copayerSignature = copayerSignature
            };
            
            var url = string.Format("/v2/wallets/{0}/copayers", walletId);

            return await this.DoPostRequest<JoinWallet.Request, JoinWallet.Response>(url, request);
        }

        public async Task<TransactionProposal.Response> createTransactionProposal(
            TransactionProposal.Options opts
        ) {
            var args = opts.DeepClone();
            args.message = Utils.encryptMessage(args.message, this.credentials.sharedEncryptingKey);
            foreach (var output in args.outputs)
            {
                output.message = output.message ?? Utils.encryptMessage(output.message, this.credentials.sharedEncryptingKey);
            }

            var url = "/v2/txproposals";

            return await this.DoPostRequest<TransactionProposal.Options, TransactionProposal.Response>(url, args);
        }

        private static string buildSecret(Guid walletId, String walletPrivKey, string network)
        {
            string widHx = walletId.ToString("N");

            string widBase58 = Encoders.Base58.EncodeData(Encoders.Hex.DecodeData(widHx));

            return widBase58 + walletPrivKey + "L";
        }

        private async Task<TResponse> DoPostRequest<TRequest, TResponse>(string url, TRequest request)
        {
            var client = new HttpClient();
            if (this.credentials != null)
            {
                client.DefaultRequestHeaders.Add("x-identity", this.credentials.copayerId);
                var reqSignature = Utils.SignRequest("post", url, request, this.credentials.requestPrivKey);
                client.DefaultRequestHeaders.Add("x-signature", reqSignature);
            }

            var fullUrl = this.baseUrl + url;

            var json = JsonConvert.SerializeObject(request);
            Console.Out.WriteLine(json);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            using (var responseMessage = await client.PostAsync(fullUrl, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();
                    Console.Out.WriteLine(responseContent);
                    return JsonConvert.DeserializeObject<TResponse>(responseContent);
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
    }
}
