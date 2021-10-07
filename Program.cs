using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBitcoinTes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExtKey masterKeyTest = new ExtKey();
            ExtPubKey masterPubKeyTest = masterKeyTest.Neuter();
            var extKeys= GeneratorExtendedKeys(masterPubKeyTest.ToBytes(), 5, Network.Main);


            var lis = await Naddress(masterPubKeyTest.ToBytes(),5, Network.Main);
            ExtKey masterKey = new ExtKey();
            Console.WriteLine("Master key : " + masterKey.ToString(Network.TestNet));
            for (int i = 0; i < 5; i++)
            {
                ExtKey key = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + " : " + key.ToString(Network.TestNet));
            }

            ExtPubKey masterPubKey = masterKey.Neuter();
            for (int i = 0; i < 5; i++)
            {
                ExtPubKey pubkey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + " : " + pubkey.ToString(Network.TestNet));
            }

            masterKey = new ExtKey();
            masterPubKey = masterKey.Neuter();

            //The payment server generate pubkey1
            ExtPubKey pubkey1 = masterPubKey.Derive((uint)1);

            //You get the private key of pubkey1
            ExtKey key1 = masterKey.Derive((uint)1);

            //Check it is legit
            Console.WriteLine("Generated address : " + pubkey1.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.TestNet));
            Console.WriteLine("Expected address : " + key1.PrivateKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.TestNet));

            ExtKey parent = new ExtKey();
            ExtKey child11 = parent.Derive(1).Derive(1);

            // OR
            parent = new ExtKey();
            child11 = parent.Derive(new KeyPath("1/1"));

            ExtKey ceoKey = new ExtKey();
            Console.WriteLine("CEO: " + ceoKey.ToString(Network.TestNet));
            ExtKey accountingKey = ceoKey.Derive(0, hardened: true);

            ExtPubKey ceoPubkey = ceoKey.Neuter();

            //ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubkey); //Crash

            var nonHardened = new KeyPath("1/2/3");
            var hardened = new KeyPath("1/2/3'");

            ceoKey = new ExtKey();
            string accounting = "1'";
            int customerId = 5;
            int paymentId = 50;
            KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId);
            //Path : "1'/5/50"
            ExtKey paymentKey = ceoKey.Derive(path);

            ExtKey key4 = masterKey.Derive(path);
            var ss = key4.PrivateKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.TestNet);

            

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemo.DeriveExtKey("my password");
            Console.WriteLine(mnemo);

            mnemo = new Mnemonic("minute put grant neglect anxiety case globe win famous correct turn link",
                Wordlist.English);
            hdRoot = mnemo.DeriveExtKey("my password");

            Console.ReadLine();
        }

        public static async Task<(List<BitcoinAddress>, List<ExtPubKey>)> Naddress(byte[] publicKey, int cant, Network coinNetworkType)
        {
            var listaddress = new List<BitcoinAddress>();
            var listExtPubKeyChilds = new List<ExtPubKey>();
            //ExtKey masterKey = new ExtKey();
            ExtPubKey external = new ExtPubKey(publicKey);
            //ExtPubKey masterPubKey = masterKey.Neuter();
            for (int i = 0; i < cant; i++)
            {
                ExtPubKey pubkeytemp = external.Derive((uint)i);
                listExtPubKeyChilds.Add(pubkeytemp);
                listaddress.Add(pubkeytemp.PubKey.GetAddress(ScriptPubKeyType.Legacy, coinNetworkType));
            }
            return (listaddress, listExtPubKeyChilds);
        }

        public static BitcoinAddress GeneratorExtendedKeys(byte[] publicKey, int cant, Network coinNetworkType)
        {
            ExtPubKey external = new ExtPubKey(publicKey); 
            KeyPath path = new KeyPath($"m/44/0/{cant}/{1}");
            var extpubkeyDerived = external.Derive(path);
            return extpubkeyDerived.PubKey.GetAddress(ScriptPubKeyType.Segwit, coinNetworkType);
        }
    }
}
