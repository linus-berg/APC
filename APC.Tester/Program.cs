// See https://aka.ms/new-console-template for more information

using ACM.Git;
using ACM.Kernel;
using APC.Kernel;
using Foundatio.Storage;

/* SETUP STORAGE */
MinioFileStorageConnectionStringBuilder connection = new();

connection.Region = Configuration.GetApcVar(ApcVariable.ACM_S3_REGION);
connection.AccessKey = Configuration.GetApcVar(ApcVariable.ACM_S3_ACCESS_KEY);
connection.SecretKey = Configuration.GetApcVar(ApcVariable.ACM_S3_SECRET_KEY);
connection.EndPoint = Configuration.GetApcVar(ApcVariable.ACM_S3_ENDPOINT);
connection.Bucket = Configuration.GetApcVar(ApcVariable.ACM_S3_BUCKET);

MinioFileStorageOptions minio_options = new() {
  AutoCreateBucket = true,
  ConnectionString = connection.ToString()
};
MinioFileStorage storage = new(minio_options);

FileSystem fs = new(storage);
Git git = new(fs);

await git.Mirror("git://github.com/linus-berg/ATM.Npm");
//await client.CopyToRegistry("docker://docker.io/registry:2");
Console.WriteLine("---");