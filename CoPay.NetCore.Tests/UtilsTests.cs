using System;
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
    }
}
