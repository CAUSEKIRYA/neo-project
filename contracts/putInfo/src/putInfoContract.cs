using System;
using System.ComponentModel;
using System.Numerics;


using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract;


namespace testTransfer
{
    [DisplayName("putInfoContract")]
    [ManifestExtra("Author", "Your name")]
    [ManifestExtra("Email", "your@address.invalid")]
    [ManifestExtra("Description", "Describe your contract...")]
    public class testTransferContract : SmartContract
    {
        private static StorageMap ContractStorage => new StorageMap(Storage.CurrentContext, "test");
        private static StorageMap ContractMetadata => new StorageMap(Storage.CurrentContext, "meta");
        private static Transaction Tx => (Transaction)Runtime.ScriptContainer;

        [DisplayName("DataChanged")]

        public static event Action<UInt160, String> OnDataChanged;

        public static bool PutInformation(String info)
        {
            if (info.Length < 0)
            {
                throw new Exception("Empty information.");
            }

            ContractStorage.Put(Tx.Sender, info);
            OnDataChanged(Tx.Sender, info);
            return true;
        }

        public static String GetLast()
        {   
            return ContractStorage.Get(Tx.Sender);
        }

        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            ContractMetadata.Put("Owner", (ByteString) Tx.Sender);
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            ByteString owner = ContractMetadata.Get("Owner");

            if (!Tx.Sender.Equals(owner))
            {
                throw new Exception("Only the contract owner can update the contract");
            }

            ContractManagement.Update(nefFile, manifest, null);
        }
    }
}
