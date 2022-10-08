using System.Text.RegularExpressions;

namespace ACM.Kernel; 

public class FileSystem {
  private static string BASE_DIR_ = "/home/linusberg/Development/APC-Storage/";
  public FileSystem() {
  }

  public void CreateDailyDeposit() {
    string daily_deposit = Path.Join(BASE_DIR_, "Daily");
    Directory.CreateDirectory(GetDailyDeposit());
  }

  private string GetDailyDeposit() {
    string daily_deposit = Path.Join(BASE_DIR_, "Daily");
    return Path.Join(daily_deposit, DateTime.UtcNow.ToString("yyMMdd"));
  }

  public void CreateDailyLink(string module, string uri_str) {
    Uri uri = new Uri(uri_str);
    string location = CleanFilepath(uri.LocalPath);
    string daily_deposit = GetDailyDeposit();
    string link = Path.Join(daily_deposit, module, location);
    Directory.CreateDirectory(Path.GetDirectoryName(link));
    File.CreateSymbolicLink(link, GetArtifactPath(module, uri_str));
  }
 
  public string GetArtifactPath(string module, string uri_str) {
    Uri uri = new Uri(uri_str);
    string location = CleanFilepath(uri.LocalPath);
    CreateModuleDirectory(module);
    string path = GetModulePath(module, location);
    CreateArtifactDirectory(Path.GetDirectoryName(path));
    return path;
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


  private string GetModuleDir(string module) {
    return Path.Join(BASE_DIR_, module);
  }
  
  public bool Exists(string filepath) {
    return File.Exists(filepath);
  }
}