using System;
using System.ComponentModel;

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;


namespace testTransfer
{
    [DisplayName("putInfoContract")]

    public class testTransferContract : SmartContract
    {
        private static StorageMap ContractMetadata => new StorageMap(Storage.CurrentContext, "meta");
        private static StorageMap FileData => new StorageMap(Storage.CurrentContext, "files");
        private static Transaction Tx => (Transaction)Runtime.ScriptContainer;

        [DisplayName("DataChanged")]

        public static event Action<UInt160, String> OnDataChanged;

        public static void AddFile(String fileName, ByteString fileData)
        {
            if(FileData.Get(fileName).Length > 0){
                throw new Exception("File alredy exist");
            }

            FileData.Put(fileName, fileData);
        }

        public static ByteString GetFileData(String fileName)
        {
            return FileData.Get(fileName);
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
