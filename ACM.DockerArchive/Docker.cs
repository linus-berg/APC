using ACM.Kernel;
using APC.Skopeo;
using APC.Skopeo.Exceptions;
using APC.Skopeo.Models;
using Minio.Exceptions;

namespace ACM.DockerArchive;

public class Docker {
  private readonly string dir_;
  private readonly FileSystem fs_;
  private readonly ILogger<Docker> logger_;
  private readonly SkopeoClient skopeo_;

  public Docker(FileSystem fs, SkopeoClient skopeo, ILogger<Docker> logger) {
    fs_ = fs;
    skopeo_ = skopeo;
    dir_ = fs_.GetModuleDir("docker-archive", true);
    logger_ = logger;
  }

  public async Task<bool> GetTarArchive(string remote_image) {
    SkopeoArchive archive;
    try {
      archive = await skopeo_.CopyToTar(remote_image, dir_);
    } catch (SkopeoArchiveExistsException ex) {
      /* Ignore if file exists */
      return true;
    }

    bool success = await PushToStorage(archive);
    if (!success) {
      throw new ApplicationException($"Failed to fetch {remote_image}");
    }

    success = await fs_.CreateDeltaLink(
                "docker-archive",
                $"docker-archive://{archive.TarWithHost}"
              );
    return success;
  }

  private async Task<bool> PushToStorage(SkopeoArchive archive) {
    if (!File.Exists(archive.TarPath)) {
      throw new FileNotFoundException($"{archive.TarPath} not found on disk.");
    }

    logger_.LogDebug("Opening: {BundleFilePath}", archive.TarPath);
    await using Stream stream = File.OpenRead(archive.TarPath);
    string storage_path = Path.Join("docker-archive", archive.TarWithHost);
    bool success = await fs_.PutFile(storage_path, stream);
    stream.Close();
    /* If S3 upload failed */
    if (!success) {
      throw new MinioException($"Failed to upload {archive.TarPath}");
    }

    return success;
  }
}