using System.Text.RegularExpressions;
using APC.Kernel;
using Foundatio.Storage;

namespace ACM.Kernel;

public class FileSystem {
  private readonly string BASE_DIR_ =
    Configuration.GetApcVar(ApcVariable.APC_ACM_DIR);

  private readonly IFileStorage storage_backend_;

  public FileSystem(IFileStorage storage_backend) {
    Console.WriteLine($"Storage Directory: {BASE_DIR_}");
    storage_backend_ = storage_backend;
  }

  public async Task<bool> Exists(string path) {
    return await storage_backend_.ExistsAsync(path);
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

  public async Task<bool> PutFile(string path, Stream stream) {
    return await storage_backend_.SaveFileAsync(path, stream);
  }
  

  private string GetDailyDeposit(string module) {
    string daily_deposit = Path.Join(BASE_DIR_, "Daily", module);
    return Path.Join(daily_deposit, DateTime.UtcNow.ToString("yyyy_MM_dd"));
  }

  public void CreateDailyLink(string module, string uri_str) {
    Uri uri = new(uri_str);
    string location = GetDiskLocation(uri);
    string daily_deposit = GetDailyDeposit(module);
    string link = Path.Join(daily_deposit, location);
    Directory.CreateDirectory(Path.GetDirectoryName(link));
    File.CreateSymbolicLink(link, GetArtifactPath(module, uri_str));
  }

  public string GetArtifactPath(string module, string uri_str) {
    Uri uri = new(uri_str);
    string location = GetDiskLocation(uri);
    string path = GetModulePath(module, location);
    return path;
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