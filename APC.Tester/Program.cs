// See https://aka.ms/new-console-template for more information

using ACM.Http;
using ACM.Kernel;
using APC.Kernel.Models;
using APC.Skopeo;
using APM.Jetbrains.IDE;
using APM.Pypi;
using Foundatio;
using Foundatio.Storage;
using IJetbrains = APM.Jetbrains.IDE.IJetbrains;
MinioFileStorageConnectionStringBuilder connection =
  new MinioFileStorageConnectionStringBuilder();
connection.Region = "bleh";
connection.AccessKey = "minio-apc";
connection.SecretKey = "minio-apc";
connection.EndPoint = "localhost:9000";
connection.Bucket = "APC";

MinioFileStorageOptions minio_options = new MinioFileStorageOptions() {
  AutoCreateBucket = true,
  ConnectionString = connection.ToString()
};
IFileStorage storage = new MinioFileStorage(minio_options);
FileSystem fs = new FileSystem(storage);


string url = "http://localhost:9001/static/js/main.379b5c5e.js";
RemoteFile file = new RemoteFile(url, fs);
string output = fs.GetArtifactPath("npm", url);

await file.Get(output);
//Artifact p = await pypi.ProcessArtifact(artifact);
Console.WriteLine("---");