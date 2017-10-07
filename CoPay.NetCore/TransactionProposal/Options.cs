using System.Collections.Generic;
using System.Linq;

namespace CoPay.TransactionProposal {
    public class Options {
        public class Output {
            public string toAddress { get; set; }

            public uint amount { get; set; }

            public string message { get; set; } 

            public Output DeepClone()
            {
                return (Output) this.MemberwiseClone();
            }
        }

        public string txProposalId { get; set; }

        public IEnumerable<Output> outputs { get; set; }

        public string message { get; set; }

        public string feeLevel { get; set; }

        public uint feePerKb { get; set; }

        public string changeAddress { get; set; }

        public bool sendMax { get; set; }

        public string payProUrl { get; set; }

        public bool excludeUnconfirmedUtxos { get; set; }

        public bool dryRun { get; set; }

        public Options DeepClone()
        {
            var clone = (Options) this.MemberwiseClone();
            clone.outputs = this.outputs.Select(o => o.DeepClone());
            return clone;
        }
    }
}

/**
 * Create a transaction proposal
 *
 * @param {Object} opts
 * @param {string} opts.txProposalId - Optional. If provided it will be used as this TX proposal ID. Should be unique in the scope of the wallet.
 * @param {Array} opts.outputs - List of outputs.
 * @param {string} opts.outputs[].toAddress - Destination address.
 * @param {number} opts.outputs[].amount - Amount to transfer in satoshi.
 * @param {string} opts.outputs[].message - A message to attach to this output.
 * @param {string} opts.message - A message to attach to this transaction.
 * @param {number} opts.feeLevel[='normal'] - Optional. Specify the fee level for this TX ('priority', 'normal', 'economy', 'superEconomy').
 * @param {number} opts.feePerKb - Optional. Specify the fee per KB for this TX (in satoshi).
 * @param {string} opts.changeAddress - Optional. Use this address as the change address for the tx. The address should belong to the wallet. In the case of singleAddress wallets, the first main address will be used.
 * @param {Boolean} opts.sendMax - Optional. Send maximum amount of funds that make sense under the specified fee/feePerKb conditions. (defaults to false).
 * @param {string} opts.payProUrl - Optional. Paypro URL for peers to verify TX
 * @param {Boolean} opts.excludeUnconfirmedUtxos[=false] - Optional. Do not use UTXOs of unconfirmed transactions as inputs
 * @param {Boolean} opts.validateOutputs[=true] - Optional. Perform validation on outputs.
 * @param {Boolean} opts.dryRun[=false] - Optional. Simulate the action but do not change server state.
 * @param {Array} opts.inputs - Optional. Inputs for this TX
 * @param {number} opts.fee - Optional. Use an fixed fee for this TX (only when opts.inputs is specified)
 * @param {Boolean} opts.noShuffleOutputs - Optional. If set, TX outputs won't be shuffled. Defaults to false
 * @returns {Callback} cb - Return error or the transaction proposal
 */