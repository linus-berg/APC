using APC.Kernel;
using ATM.RKE2.Models;
using RestSharp;

namespace ATM.RKE2; 

public class RancherProcessor {
  private readonly GithubClient gh_client_ = new();
  private readonly RestClient http_client_ = new();
  private readonly string save_dir_ = Path.Join(Configuration.GetAPCVar(ApcVariable.APC_ACM_DIR), "rancher/rke2");
  
  public async Task<List<string>> CheckReleases() {
    Directory.CreateDirectory(save_dir_);
    List<GithubRelease> releases = await gh_client_.GetRancherReleases();
    List<string> new_releases = new List<string>();
    foreach (GithubRelease release in releases) {
      string url = release.GetRancherImageFile();
      string filename = $"{release.tag_name}_{Path.GetFileName(url)}";
      if (url == null || TagExists(filename)) {
        continue;
      }
      Console.WriteLine($"New release found {url}");
      try {
        string rancher_file = await SaveRancherFile(url, filename);
        new_releases.Add(rancher_file);
      } catch (Exception e) {
        
      }
    }
    return new_releases;
  }

  private bool TagExists(string filename) {
    return File.Exists(GetSavePath(filename));
  }

  private async Task<string> SaveRancherFile(string url, string filename) {
    string save_path = GetSavePath(filename);
    Console.WriteLine(save_path);
    try {
      await using Stream stream = await http_client_.DownloadStreamAsync(new RestRequest(url));
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
    await using FileStream output = File.Open(save_path, FileMode.Create);
    await stream.CopyToAsync(output);
    return save_path;
  }

  private string GetSavePath(string filename) {
    return Path.Join(save_dir_, filename);
  }
}
