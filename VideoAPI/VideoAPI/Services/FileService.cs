using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoAPI.Services
{
    public class FileService
    {

        public string SaveFile(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine("videos", subDirectory);

            Directory.CreateDirectory(target);

            files.ForEach( file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                     file.CopyTo(stream);
                }
            });

            return target;
        }

        public void DeleteFileAndDirectory(string target)
        {
            var newFiles = Directory.GetFiles(target);
            foreach (string f in newFiles)
            {
                File.Delete(f);
            }
            Directory.Delete(target);
        }

        public void AddJobToMQ(string blobName)
        {
            var Factory = new ConnectionFactory() { HostName = "rabbitmq" };


            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(blobName);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                    
                channel.BasicPublish(exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);
            }
        }

        public async Task UploadVideoToBlobStorage(string videoPath, string blobName, string owner, string title)
        {

            string videoToUpload = Directory.GetFiles(videoPath)[0];
            FileInfo file = new FileInfo(videoToUpload);
            string contentType;
            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(file.Name, out contentType))
            {
                contentType = "application/octet-stream";
            }
            string containerName = "videos";

            // Retrieve storage account information from connection string
            var storageCredentials = new StorageCredentials("picturestoragernalbi", "PiyHABH1PBZGqYLvt0KTR8SNMFPbViQrtK1JKAxmtcPzAj9joZ+l4V8+gEgs0IGL9CWR1M47sfKdnWkemiQc1g==");
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);//Common.CreateStorageAccountFromConnectionString();

            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            try
            {
                BlobRequestOptions requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
                await container.CreateIfNotExistsAsync(requestOptions, null);
            }
            catch (StorageException)
            {
                throw;
            }

            // Upload a BlockBlob to the newly created container

            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{blobName}");

            // Set the blob's content type so that the browser knows to treat it as an image.
            blockBlob.Properties.ContentType = contentType;

            blockBlob.Metadata.Add("owner", owner);
            blockBlob.Metadata.Add("album", title);

            await blockBlob.UploadFromFileAsync(videoToUpload);
        }

        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
    }
}

