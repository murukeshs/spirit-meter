using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Data
{
    public class AzureStorage
    {
        public static string ConnString()  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var StorageKey = configuration.GetSection("StorageAccount").Value;

            return StorageKey;
        }

        public static async Task<string> UploadImage(byte[] file, string DocumentName, string Folder)
        {
            string uri = "";

            try
            {
                var AccountKey = ConnString();

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(AccountKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Folder);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(DocumentName);
               
                //byte[] byteArray = Encoding.ASCII.GetBytes(DocumentBytes);
                //MemoryStream stream = new MemoryStream(byteArray);

                Stream fileStream = new MemoryStream(file);
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                //var fileStream = file.OpenReadStream();
                ////Stream fileStream = new MemoryStream(fileStram);
                //await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                uri = Convert.ToString(cloudBlockBlob.Uri);
                return uri;
            }

            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
