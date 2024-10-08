using ACM.Kernel;

namespace ACM.Http;

public class RemoteFile {
  private static readonly HttpClient S_CLIENT_ = new();
  private readonly FileSystem fs_;
  private readonly string url_;

  public RemoteFile(string url, FileSystem fs) {
    url_ = url;
    fs_ = fs;
    //S_CLIENT_.DefaultRequestHeaders.Add("User-Agent", "APC/1.0");
  }

  public async Task<bool> Get(string path) {
    HttpResponseMessage response =
      await S_CLIENT_.GetAsync(url_, HttpCompletionOption.ResponseHeadersRead);
    if (!response.IsSuccessStatusCode ||
        response.Content.Headers.ContentLength == null) {
      return false;
    }

    long remote_size = (long)response.Content.Headers.ContentLength;
    bool is_empty = remote_size == 0;
    await using Stream remote_stream =
      await response.Content.ReadAsStreamAsync();
    bool result;
    try {
      result = await ProcessStream(path, remote_stream);
    } catch (Exception e) {
      remote_stream.Close();
      await ClearFile(path);
      throw;
    }

    if (is_empty && !result) {
      result = await fs_.PutString(path, "-");
    }

    if (result) {
      /* If downloaded file size matches remote, its complete */
      long size = await fs_.GetFileSize(path);
      if (size == remote_size) {
        return true;
      }
    } else {
      await ClearFile(path);
      throw new HttpRequestException($"{url_} failed to collect.");
    }

    return false;
  }

  private async Task<bool>
    ProcessStream(string path, Stream remote_stream) {
    bool result;
    try {
      result = await fs_.PutFile(path, remote_stream);
    } catch (Exception e) {
      await ClearFile(path);
      throw;
    }

    return result;
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