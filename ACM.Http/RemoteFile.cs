using ACM.Kernel;

namespace ACM.Http;

public class RemoteFile {
  private const int BUFFER_SIZE_ = 8192;
  private static readonly HttpClient CLIENT_ = new();
  private readonly string url_;
  private readonly FileSystem fs_;

  public RemoteFile(string url, FileSystem fs) {
    url_ = url;
    fs_ = fs;
  }

  public async Task<bool> Get(string filepath) {
    HttpResponseMessage response =
      await CLIENT_.GetAsync(url_, HttpCompletionOption.ResponseHeadersRead);
    if (!response.IsSuccessStatusCode ||
        response.Content.Headers.ContentLength == null) {
      return false;
    }

    long size = (long)response.Content.Headers.ContentLength;
    await using Stream s = await response.Content.ReadAsStreamAsync();
    try {
      await ProcessStream(s, filepath);
    } catch (Exception e) {
      s.Close();
      await ClearFile(filepath);
      throw;
    }

    /* If downloaded file size matches remote, its complete */
    if (await fs_.GetFileSize(filepath) == size) {
      return true;
    }

    await ClearFile(filepath);
    return false;
  }

  private async Task
    ProcessStream(Stream s, string filepath) {
    
    try {
      await fs_.PutFile(filepath, s); 
    } catch (Exception e) {
      await ClearFile(filepath);
      throw;
    }
  }

  private async Task<bool> ClearFile(string file) {
    
    if (!(await fs_.Exists(file))) {
      return false;
    }

    Console.WriteLine($"Clearing {file}");
    await fs_.Delete(file);
    return true;
  }
}