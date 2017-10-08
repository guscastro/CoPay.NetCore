using System;
using System.Collections.Generic;
using Xunit;

namespace CoPay.NetCore.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void Should_Sign_Message()
        {
            var hex = "3045022100c489117f1e2132dd827a0bc98af7f5a429ef57dce2dcb3cd6d362c42a9cd13ed0220124d2e7c103ee8179ad09e025e8f0cdd7954b28593ad6df8a4620952e7d31e9f";
            var walletPrivateKey = NBitcoin.Key.Parse("L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh", NBitcoin.Network.Main);
            var copayerHash = Utils.getCopayerHash(
                "gus2",
                "xpub69rW9xQLPBr9jJYH6oGQqxbmfEnyeHkSKpm4VrPwbqdBQAXjuG9Zx6DcVf9bbaDVovCq8UbfbGmb2e2jWeLG8DtyV7jbL8UroPAhKxY6aii",
                "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93"
            );

            var signature = Utils.signMessage(copayerHash, walletPrivateKey);

            Assert.Equal(hex, signature);
        }

        [Fact]
        public void Should_Sign_Request() 
        {
            var expected = "304402204762c4fe40378120dbfcec95d7dd7f2e5b134d58b84f5fc877eeef910af0cad7022010fd000c45b9a3321cbaeb34ba2ace19e44decf91f5570071958ebf467827fe1";
            var url = Client.BWS_INSTANCE_URL + "/v2/proposals";
            var args = new TransactionProposal.Options {
                txProposalId = "723bcc40-2da4-4b89-8856-6ef61d1f5ee9",
                outputs = new List<TransactionProposal.Options.Output>
                {
                    new TransactionProposal.Options.Output
                    {
                        toAddress = "TODO",
                        amount = 10,
                        message = "Hello Output"
                    }
                },
                message = "lx045OIjl+qyxxPFLf+jDtmjTCHcQfiDNeTuDxjnpAc=",
                feeLevel = null,
                feePerKb = 0,
                changeAddress = null,
                sendMax = false,
                payProUrl = null,
                excludeUnconfirmedUtxos = false,
                dryRun = false
            };
            var xkey = NBitcoin.ExtKey.Parse("xprv9s21ZrQH143K4QfiqFa9SfBhBhcCUmBjNZ3JbovDKiz7jADZhok1rXLVozgoicjX9WPytAco48GNTJeDUtu5eiU8DV2kznJ1KBv7xUc1Jda");
            var der = xkey.Derive(CoPay.Constants.REQUEST_PATH);
            var actual = Utils.SignRequest("post", url, args, der.PrivateKey);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Convert_Private_Key_To_AES_Key() 
        {
            var input = "L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh";
            var expected = "FIMnYsL8WmcoW0C6C2d/aA==";
            var actual = Utils.PrivateKeyToAESKey(input);
            Assert.Equal(expected, actual);
        }
    }
}
