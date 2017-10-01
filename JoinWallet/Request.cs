using System;

namespace CoPay.JoinWallet
{
    public class Request
    {
        public String walletId { get; set; }

        public String name { get; set; }

        public String xPubKey { get; set; }

        public String requestPubKey { get; set; }

        public String copayerSignature { get; set; }
    }
}