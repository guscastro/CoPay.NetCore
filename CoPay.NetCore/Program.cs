using System;
using NBitcoin;

namespace CoPay.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Call();
        }

        static void Call() {
            var privkey = "L4vrWtn7zFnboSucZ84XGHzt13HEWMoD48HgXZ49u4JZoA6A9dMh";
            var pubkey = "0254fea7b08745c15103765a5c299b354ac6fbd3fa6a33c5ee84b6fa0fd108ab4e";
            var xPubKey = "xpub661MyMwAqRbcGUAtmn55urDkFGxWFsFf6tJphcdLFvYTYyd45qS4TrqC69eswNyE7Zf3tCtQn29vhy3TjAv75GoigSyNVS5tjcnckt2nczf";
            var xPrivKey = "xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC";
            var copayerName = "gus2";
            var walletId = Guid.Parse("f44cb0f0-4913-45f9-923a-b23715d0c8b0");
            var client = new CoPay.Client();
            var task = client.doJoinWallet(walletId, privkey, "xpub69rW9xQLPBr9jJYH6oGQqxbmfEnyeHkSKpm4VrPwbqdBQAXjuG9Zx6DcVf9bbaDVovCq8UbfbGmb2e2jWeLG8DtyV7jbL8UroPAhKxY6aii", copayerName);

            task.Wait();

            Console.Out.WriteLine(task.Result);
        }

        static void Generate() {
            var k = new NBitcoin.ExtKey();
            Console.Out.WriteLine("Ext priv");
            Console.Out.WriteLine(k.ToString(NBitcoin.Network.Main));

            // Console.Out.WriteLine("Ext pub");
            // Console.Out.WriteLine(k);

            var pk = k.PrivateKey;
            Console.Out.WriteLine("PrivK");
            Console.Out.WriteLine(pk.ToString(NBitcoin.Network.Main));

            Console.Out.WriteLine("Public");
            Console.Out.WriteLine(pk.PubKey.ToString());
        }

        static void Parse() {
            var k = NBitcoin.ExtKey.Parse("xprv9s21ZrQH143K3z6RfkY5YiH1hF81rQXojfPDuEDihb1UgBHuYJ7ov4WiEuizAcoJh4gHwhusHJwukqG8zBGwETh7RZPcGGmgbRUiE5t4SWC", Network.Main);
            Console.Out.WriteLine("Priv");
            Console.Out.WriteLine(k.PrivateKey.ToString(Network.Main));
            Console.Out.WriteLine("Pub");
            Console.Out.WriteLine(k.PrivateKey.PubKey.ToString());
            Console.Out.WriteLine("Pub");
            var kp = new KeyPath("m/1'/0");
            Console.Out.WriteLine("Derived");
            var der = k.Derive(kp);
            Console.Out.WriteLine(der.ToString(Network.Main));
            Console.Out.WriteLine(der.PrivateKey.ToString(Network.Main));
            Console.Out.WriteLine(der.PrivateKey.PubKey.ToString());
        }
    }
}
