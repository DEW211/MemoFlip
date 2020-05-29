using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;

namespace XabeTest
{
    class Program
    {
        public static IConnection CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            return factory.CreateConnection();
        }

        private static IConnection GetConnection()
        {
            IConnection connection;
            bool isConnected = false;
            while (!isConnected)
            {

                try
                {
                    connection = CreateConnection();
                    isConnected = true;
                    return connection;
                }
                catch (BrokerUnreachableException e)
                {
                    Console.WriteLine("Error, retrying in 5 seconds");
                    Console.WriteLine(e.Message);
                    Thread.Sleep(5000);
                }
            }
            return null;
        }
        static void Main(string[] args)
        {
            
            using (var connection = GetConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    string[] tmp = message.Split(' ');
                    var msg = tmp[0];
                    var title = tmp[1];
                    var owner = tmp[2];

                    BlobInfo videoInfo = downloadVideo(msg, title, owner).GetAwaiter().GetResult();
                    Console.WriteLine(message);
                    Console.WriteLine(videoInfo);
                    var picturesDirPath = ExtractFrames(videoInfo.VideoPath, msg).GetAwaiter().GetResult();

                    List<string> uriList = UploadPictures(picturesDirPath).GetAwaiter().GetResult();

                    CallAlbumAPI(uriList, videoInfo).Wait();

                    DeleteVideo(videoInfo.VideoPath);
                    DeletePictures(picturesDirPath);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "task_queue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }

        private static void DeletePictures(string picturesDirPath)
        {
            var files = Directory.GetFiles(picturesDirPath);
            foreach(string file in files)
            {
                File.Delete(file);
            }
        }

        private static void DeleteVideo(string videoPath)
        {
            File.Delete(videoPath);
        }

        private static async Task<List<string>> UploadPictures(string picturesPath)
        {
            List<string> res = new List<string>();


            var files = Directory.GetFiles(picturesPath);

            string containerName = "albums";
            // Retrieve storage account information from connection string
            var storageCredentials = new StorageCredentials("picturestoragernalbi", "PiyHABH1PBZGqYLvt0KTR8SNMFPbViQrtK1JKAxmtcPzAj9joZ+l4V8+gEgs0IGL9CWR1M47sfKdnWkemiQc1g==");
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);//Common.CreateStorageAccountFromConnectionString();

            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            foreach(string filePath in files)
            {
                FileInfo info = new FileInfo(filePath);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(info.Name);
                blockBlob.Properties.ContentType = "image/png";
                await blockBlob.UploadFromFileAsync(info.FullName);
                res.Add(blockBlob.Uri.ToString());
            }
            //upload


            return res;
        }

        private static async Task CallAlbumAPI(List<string> uriList, BlobInfo info)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (string uri in uriList)
                {
                    var content = new StringContent($"\"{uri}\"", Encoding.UTF8, "application/json");
                    var result = await httpClient.PatchAsync($"http://localhost:32768/api/album/{info.MetaOwner}/{info.MetaAlbum}", content);
                }
            }
        }

        private static async Task<BlobInfo> downloadVideo(string blobName, string title, string owner)
        {
            string containerName = "videos";
            //letölt, return path
            // Retrieve storage account information from connection string
            var storageCredentials = new StorageCredentials("picturestoragernalbi", "PiyHABH1PBZGqYLvt0KTR8SNMFPbViQrtK1JKAxmtcPzAj9joZ+l4V8+gEgs0IGL9CWR1M47sfKdnWkemiQc1g==");
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);//Common.CreateStorageAccountFromConnectionString();

            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            

            var directory = Directory.GetCurrentDirectory();
            var dirInfo = Directory.CreateDirectory(directory + "\\videos");

            await blockBlob.DownloadToFileAsync($"{dirInfo}\\{blobName}", FileMode.Create);

            return new BlobInfo()
            {
                VideoPath = $"{dirInfo}\\{blobName}",
                MetaAlbum = title,
                MetaOwner = owner
            };
        }

        private static async Task<string> ExtractFrames(string path, string namePrefix)
        {
            var directory = Directory.GetCurrentDirectory();
            var dirInfo = Directory.CreateDirectory(directory + "\\output");
            
            Func<string, string> outputFileNameBuilder = (number) => { return $"{dirInfo.FullName}\\{namePrefix}{number}.png"; };
            IMediaInfo info = await MediaInfo.Get(path).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Png);

            IConversionResult conversionResult = await Conversion.New()
                .AddStream(videoStream)
                .ExtractEveryNthFrame(10, outputFileNameBuilder)
                .Start();
            Console.WriteLine(conversionResult.Arguments);
            return dirInfo.FullName;

        }

        
    }
}
