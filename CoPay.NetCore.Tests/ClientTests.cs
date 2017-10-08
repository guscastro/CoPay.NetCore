using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NBitcoin;

namespace CoPay.NetCore.Tests
{
    public class ClientTests
    {
        private readonly Client client;
        public ClientTests()
        {
            this.client = new Client();
        }

        [Fact]
        public async Task Should_Join_Wallet()
        {
            this.client.credentials = Credentials.Create("test", Network.TestNet);
            var response = await this.client.createWallet("test", "test", 1, 1);
            await this.client.doJoinWallet(response.walletId);
        }

        [Fact]
        public async Task Should_Create_Tx_Proposal()
        {
            this.client.credentials = Credentials.FromTestCredentials();
            var response = await this.client.createWallet("test", "test", 1, 1);
            var opts = new TransactionProposal.Options
            {
                txProposalId = Guid.NewGuid().ToString(),
                outputs = new List<TransactionProposal.Options.Output>
                {
                    new TransactionProposal.Options.Output
                    {
                        toAddress = "mqmc1EiKHHoi1dqZQgstfAAj8qajpVwagf",
                        amount = 10000,
                        message = "Hello Output"
                    }
                },
                message = "Hello Options"
            };
            await this.client.createTransactionProposal(opts);
        }
    }
}