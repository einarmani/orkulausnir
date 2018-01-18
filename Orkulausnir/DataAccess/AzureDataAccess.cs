using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Orkulausnir.Models;

namespace Orkulausnir.DataAccess
{
    public static class AzureDataAccess
    {
        private static readonly CloudBlobContainer BlobContainer;

        static AzureDataAccess()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=orkulausnirstorage;AccountKey=zBo4YEFssrzqSRL1xuexkJVzxr5jPMI1GHJxAXqjD5m3ejSraXE85GibB0uxZSP3Mm4tiVPhLVrUSftm1k4H7g==;EndpointSuffix=core.windows.net");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            BlobContainer = blobClient.GetContainerReference("files");
        }

        public static Dictionary<string, string> GetFileNames()
        {
            var files = new Dictionary<string, string>();

            BlobContinuationToken continuationToken = null;
            BlobResultSegment listBlobsSegmentedAsync = BlobContainer.ListBlobsSegmentedAsync(null, continuationToken).Result;

            foreach (IListBlobItem item in listBlobsSegmentedAsync.Results)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    files.Add(Path.GetFileNameWithoutExtension(item.Uri.AbsoluteUri), Path.GetFileName(item.Uri.AbsoluteUri));
                }
                else
                {
                    throw new Exception($"Get ekki lesið skrána {Path.GetFileName(item.Uri.AbsoluteUri)}.");
                }
            }

            return files;
        }

        public static async Task<byte[]> GetFile(string fileName)
        {
            byte[] file = { };
            bool exists = BlobContainer.GetBlockBlobReference(fileName).ExistsAsync().Result;
            if (exists)
            {
                CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(fileName);

                using (var memoryStream = new MemoryStream())
                {
                    await blockBlob.DownloadToStreamAsync(memoryStream).ConfigureAwait(false);
                    file = memoryStream.ToArray();
                }
            }

            return file;
        }
        

        public static async Task<bool> UploadFile(IFormFile file)
        {
            if (GetFile(file.FileName).Result.Length == 0)
            {
                // filename already exists!
                return false;
            }
            var fileName = Path.GetFileName(file.FileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(fileName);
            
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return true;
        }
    }
}