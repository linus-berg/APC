using Collector.Kernel;

namespace Collector.Http;

public class RemoteFile {
  private static readonly HttpClient S_CLIENT_ = new();
  private readonly FileSystem fs_;
  private readonly string url_;

  public RemoteFile(string url, FileSystem fs) {
    url_ = url;
    fs_ = fs;
  }

  public async Task<bool> Get(string path) {
    HttpResponseMessage response =
      await S_CLIENT_.GetAsync(url_, HttpCompletionOption.ResponseHeadersRead);
    if (!response.IsSuccessStatusCode) {
      return false;
    }

    try {
      Stream? body = await response.Content.ReadAsStreamAsync();
      bool result = await fs_.PutFile(path, body);

      if (!result) {
        await ClearFile(path);
        throw new HttpRequestException($"{url_} failed to collect.");
      }

      return result;
    } catch (Exception) {
      await ClearFile(path);
      throw;
    }
  }

  private async Task<bool> ClearFile(string file) {
    if (!await fs_.Exists(file)) {
      return false;
    }

    Console.WriteLine($"Clearing {file}");
    await fs_.Delete(file);
    return true;
  }
}