using System.Text.RegularExpressions;
using APC.Kernel;

namespace ACM.Kernel;

public class FileSystem {
  private static readonly string BASE_DIR_ =
    Configuration.GetApcVar(ApcVariable.APC_ACM_DIR);

  static FileSystem() {
    Console.WriteLine($"Storage Directory: {BASE_DIR_}");
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
    CreateModuleDirectory(module);
    string path = GetModulePath(module, location);
    CreateArtifactDirectory(Path.GetDirectoryName(path));
    return path;
  }

  private string GetDiskLocation(Uri uri) {
    return $"{uri.Host}{CleanFilepath(uri.LocalPath)}";
  }

  private string CleanFilepath(string location) {
    return Regex.Replace(location, @"\/-\/", "/");
  }

  private string GetModulePath(string module, string filepath) {
    return Path.Join(GetModuleDir(module), filepath);
  }

  private void CreateArtifactDirectory(string path) {
    Directory.CreateDirectory(path);
  }

  private void CreateModuleDirectory(string module) {
    Directory.CreateDirectory(GetModuleDir(module));
  }


  public string GetModuleDir(string module, bool create = false) {
    string dir = Path.Join(BASE_DIR_, module);
    if (create) {
      Directory.CreateDirectory(dir);
    }

    return dir;
  }

  public bool Exists(string filepath) {
    return File.Exists(filepath);
  }
}