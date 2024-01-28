using System.Text.RegularExpressions;
using APC.Kernel;
using Foundatio.Storage;
using Polly;
using Polly.Registry;

namespace ACM.Kernel;

public class FileSystem {
  private readonly string? BASE_DIR_ =
    Configuration.GetApcVar(ApcVariable.APC_ACM_DIR);

  private readonly IFileStorage storage_backend_;
  private readonly ResiliencePipeline<bool> storage_pipeline_;

  public FileSystem(IFileStorage storage_backend,
                    ResiliencePipelineProvider<string> polly) {
    storage_backend_ = storage_backend;
    storage_pipeline_ = polly.GetPipeline<bool>("storage-pipeline");
  }

  public async Task<bool> Exists(string path) {
    return await storage_backend_.ExistsAsync(path);
  }

  public async Task<IReadOnlyCollection<FileSpec>> GetFileList(
    string search_pattern) {
    return await storage_backend_.GetFileListAsync(search_pattern);
  }

  public async Task<bool> Delete(string path) {
    return await storage_backend_.DeleteFileAsync(path);
  }

  public async Task<bool> Rename(string a, string b) {
    return await storage_backend_.RenameFileAsync(a, b);
  }

  public async Task<Stream> GetStream(string path) {
    return await storage_backend_.GetFileStreamAsync(path);
  }

  public async Task<string> GetString(string path) {
    return await storage_backend_.GetFileContentsAsync(path);
  }

  public async Task<bool> PutString(string path, string content) {
    return await storage_pipeline_.ExecuteAsync(
             static async (state, _) =>
               await state.storage_backend_.SaveFileAsync(
                 state.path, state.content), (storage_backend_, path, content));
  }

  public async Task<bool> PutFile(string path, Stream stream) {
    return await storage_pipeline_.ExecuteAsync(
             static async (state, token) =>
               await state.storage_backend_.SaveFileAsync(
                 state.path, state.stream, token),
             (storage_backend_, path, stream));
  }


  private string GetDeltaDeposit(string module) {
    string daily_deposit = Path.Join("delta", module);
    return Path.Join(daily_deposit, DateTime.UtcNow.ToString("yyyy_MM_dd"));
  }

  public async Task<bool> CreateDeltaLink(string module, string uri_str) {
    Uri uri = new(uri_str);
    string location = GetDiskLocation(uri);
    string daily_deposit = GetDeltaDeposit(module);
    string link = Path.Join(daily_deposit, location);
    string target = GetArtifactPath(module, uri_str);
    return await CreateS3Link(link, target);
  }

  private async Task<bool> CreateS3Link(string link, string target) {
    return await PutString(link, target);
  }

  public string GetArtifactPath(string module, string uri_str) {
    Uri uri = new(uri_str);
    string location = GetDiskLocation(uri);
    return GetModulePath(module, location);
  }

  public async Task<long> GetFileSize(string filepath) {
    FileSpec spec = await storage_backend_.GetFileInfoAsync(filepath);
    return spec.Size;
  }

  private string GetDiskLocation(Uri uri) {
    return $"{uri.Host}{CleanFilepath(uri.LocalPath)}";
  }

  private string CleanFilepath(string location) {
    return Regex.Replace(location, @"\/-\/", "/");
  }

  private string GetModulePath(string module, string filepath) {
    return Path.Join(module, filepath);
  }

  public string GetModuleDir(string module, bool create = false) {
    string dir = Path.Join(BASE_DIR_, module);
    if (create) {
      Directory.CreateDirectory(dir);
    }

    return dir;
  }
}