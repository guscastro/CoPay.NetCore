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
        private readonly Network network = Network.TestNet;
        public const string BWS_INSTANCE_URL = "https://bws.bitpay.com/bws/api";

        public Credentials credentials;

        private readonly string baseUrl;

        public Client(String baseUrl = BWS_INSTANCE_URL)
        {
            this.baseUrl = baseUrl;
        }

        public void SetCredentials(string xPrivKey, Key walletPrivKey, string copayerName)
        {
            this.credentials = Credentials.FromExtendedPrivateKey(xPrivKey, walletPrivKey, copayerName, this.network);
        }

        public async Task<CreateWallet.Response> createWallet(
            String walletName,
            String copayerName,
            Int16 m,
            Int16 n
        ) {
            CreateWallet.Request request = new CreateWallet.Request()
            {
                m = m,
                n = n,
                name = walletName,
                pubKey = this.credentials.walletPubKey,
                network = this.network == Network.Main ? "livenet" : "testnet"
            };

            String url = this.baseUrl + "/v2/wallets/";

            HttpClient client = new HttpClient();

            String json = JsonConvert.SerializeObject(request);
            StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("Create wallet request");
            Console.WriteLine(json);

            using (HttpResponseMessage responseMessage = await client.PostAsync(url, requestContent))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<CreateWallet.Response>(responseContent);
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

        public async Task<dynamic> RequestNewAddress()
        {
            var url = "/v1/addresses";
            return await this.DoPostRequest<dynamic, dynamic>(url);
        }

        public async Task<dynamic> GetWalletAddresses()
        {
            var url = "/v1/addresses";
            var client = BuildClient("get", url);
            using (var responseMessage = await client.GetAsync(this.baseUrl + url))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    String responseContent = await responseMessage.Content.ReadAsStringAsync();
                    Console.Out.WriteLine("wallet addresses");
                    Console.Out.WriteLine(responseContent);
                    return JsonConvert.DeserializeObject<dynamic>(responseContent);
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

        private async Task<TResponse> DoPostRequest<TRequest, TResponse>(string url, TRequest request = default(TRequest))
        {
            var client = BuildClient("post", url, request);

            var fullUrl = this.baseUrl + url;

            string json = string.Empty;
            if (request != null)
            {
                json = JsonConvert.SerializeObject(request);
            }
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
                    var errorMessage = await responseMessage.Content.ReadAsStringAsync();
                    Console.Out.WriteLine(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private HttpClient BuildClient(string method, string url, object request = null)
        {
            var client = new HttpClient();
            if (this.credentials != null)
            {
                client.DefaultRequestHeaders.Add("x-identity", this.credentials.copayerId);
                var reqSignature = Utils.SignRequest(method, url, request, this.credentials.requestPrivKey);
                client.DefaultRequestHeaders.Add("x-signature", reqSignature);
            }

            return client;
        }
    }
}
