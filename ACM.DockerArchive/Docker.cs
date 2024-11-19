using ACM.Kernel;
using APC.Skopeo;
using APC.Skopeo.Models;
using Minio.Exceptions;

namespace ACM.DockerArchive;

public class Docker {
  private readonly string dir_;
  private readonly FileSystem fs_;
  private readonly SkopeoClient skopeo_;
  private readonly ILogger<Docker> logger_;
  
  public Docker(FileSystem fs, SkopeoClient skopeo, ILogger<Docker> logger) {
    fs_ = fs;
    skopeo_ = skopeo;
    dir_ = fs_.GetModuleDir("docker-archive", true);
    logger_ = logger;
  }

  public async Task GetTarArchive(string remote_image) {
    SkopeoArchive archive = await skopeo_.CopyToTar(remote_image, dir_);
    bool success = await PushToStorage(archive.TarPath);
    if (!success) {
      throw new ApplicationException($"Failed to fetch {remote_image}");
    }
    
    await fs_.CreateDeltaLink("docker-archive",
                              $"docker-archive://{archive.TarName}.tar");
  }

  private async Task<bool> PushToStorage(string tar_path) {
    if (!File.Exists(tar_path)) {
      throw new FileNotFoundException(
        $"{tar_path} not found on disk.");
    }
    logger_.LogDebug("Opening: {BundleFilePath}", tar_path);
    await using Stream stream = File.OpenRead(tar_path);
    string storage_path =
      Path.Join("docker-archive", Path.GetFileName(tar_path));
    bool success = await fs_.PutFile(storage_path, stream);
    stream.Close();
    /* If S3 upload failed */
    if (!success) {
      throw new MinioException($"Failed to upload {tar_path}");
    }
    return success;
  }
}