using APC.Kernel;
using ATM.Rancher.Models;
using RestSharp;

namespace ATM.Rancher; 

public class RancherProcessor {
  private readonly GithubClient gh_client_;
  private readonly RestClient http_client_ = new();
  private readonly string save_dir_;
  private readonly string repo_;
  private readonly string file_;

  public RancherProcessor(string repo, string file) {
    repo_ = repo;
    gh_client_ = new(repo_);
    file_ = file;
    save_dir_ = Path.Join(Configuration.GetAPCVar(ApcVariable.APC_ACM_DIR), ".meta/", repo_);
  }
  
  public async Task<List<string>> CheckReleases() {
    Directory.CreateDirectory(save_dir_);
    List<GithubRelease> releases = await gh_client_.GetRancherReleases();
    List<string> new_releases = new List<string>();
    foreach (GithubRelease release in releases) {
      string url = release.GetRancherImageFile(file_);
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
      await using FileStream output = File.Open(save_path, FileMode.Create);
      await stream.CopyToAsync(output);
    } catch (Exception e) {
      if (File.Exists(save_path)) {
        File.Delete(save_path);
      }
      Console.WriteLine(e);
      throw;
    }
    return save_path;
  }

  private string GetSavePath(string filename) {
    return Path.Join(save_dir_, filename);
  }
}
