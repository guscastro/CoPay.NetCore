using System;
using System.Collections.Generic;
using Xunit;
using NBitcoin;

namespace CoPay.NetCore.Tests
{
    public class SecretDataTests
    {
        [Fact]
        public void Should_Parse_Secret()
        {
            var secretData = SecretData.FromSecret("9TaJJxTQ6uFYg4kabVag98KyF37KpgttZLrn2DtWj9LuwQi53urzHX4xAGJo6WU5VyoP2KH2XETbtc");
            Assert.Equal(secretData.WalletId.ToString(), "447ef7db-85e6-4aad-8547-a503c15c853b");
            var hexKey = System.BitConverter.ToString(secretData.WalletPrivateKey.ToBytes()).Replace("-", string.Empty);
            Assert.Equal(hexKey.ToLower(), "3c5b6a54a3412d5cd7bc7e54624a53ab0e30e9c5e914786bf9ac28887d68b476");
            Assert.Equal(secretData.Coin, "btc");
            Assert.Equal(secretData.Network, Network.TestNet);
        }
    }
}