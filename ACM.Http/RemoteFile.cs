namespace ACM.Http;

internal class RemoteFile {
  private const int BUFFER_SIZE_ = 8192;
  private static readonly HttpClient CLIENT_ = new();
  private readonly string url_;

  public RemoteFile(string url) {
    url_ = url;
  }

  public async Task<bool> Get(string filepath) {
    string tmp_file = filepath + ".tmp";
    HttpResponseMessage response =
      await CLIENT_.GetAsync(url_, HttpCompletionOption.ResponseHeadersRead);
    if (!response.IsSuccessStatusCode ||
        response.Content.Headers.ContentLength == null) {
      return false;
    }

    long size = (long)response.Content.Headers.ContentLength;
    using Stream s = await response.Content.ReadAsStreamAsync();
    try {
      await ProcessStream(s, tmp_file);
    } catch (Exception e) {
      s.Close();
      ClearFile(filepath);
      ClearFile(tmp_file);
      throw;
    }

    /* If downloaded file size matches remote, its complete */
    if (new FileInfo(tmp_file).Length == size) {
      /* Rename */
      File.Move(tmp_file, filepath);
    } else {
      File.Delete(tmp_file);
      return false;
    }

    return true;
  }

  private static async Task
    ProcessStream(Stream s, string filepath) {
    using FileStream fs = new(
      filepath,
      FileMode.Create,
      FileAccess.Write,
      FileShare.None,
      BUFFER_SIZE_,
      true
    );
    /* Progress */
    int total = 0;
    int read = -1;
    byte[] buffer = new byte[BUFFER_SIZE_];
    try {
      while (read != 0) {
        read = await s.ReadAsync(buffer, 0, buffer.Length);
        total += read;
        if (read == 0) {
          break;
        }

        await fs.WriteAsync(buffer, 0, read);
      }
    } catch (Exception e) {
      fs.Close();
      ClearFile(filepath);
      throw;
    }

    fs.Close();
  }

  private static bool ClearFile(string file) {
    if (!File.Exists(file)) {
      return false;
    }

    Console.WriteLine($"Clearing {file}");
    File.Delete(file);
    return true;
  }
}