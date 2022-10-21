namespace ACM.Http; 

internal class RemoteFile {
  private const int BUFFER_SIZE = 8192;
  private static readonly HttpClient CLIENT_ = new();
  private readonly string URL_;

  public RemoteFile(string url) {
    URL_ = url;
  }

  public async Task<bool> Get(string filepath) {
    string tmp_file = filepath + ".tmp";
    HttpResponseMessage response =
      await CLIENT_.GetAsync(URL_, HttpCompletionOption.ResponseHeadersRead);
    if (response.Content.Headers.ContentLength == null) return false;
    long size = (long)response.Content.Headers.ContentLength;
    using Stream s = await response.Content.ReadAsStreamAsync();
    try {
      await ProcessStream(s, (int)size, tmp_file);
    }
    catch (Exception e) {
      throw;
    }
    finally {
      s.Close();
    }

    /* If downloaded file size matches remote, its complete */
    if (new FileInfo(tmp_file).Length == size)
      /* Rename */
      File.Move(tmp_file, filepath);
    else
      File.Delete(tmp_file);

    return true;
  }

  private static async Task
    ProcessStream(Stream s, int size, string filepath) {
    using FileStream fs = new(
      filepath,
      FileMode.Create,
      FileAccess.Write,
      FileShare.None,
      BUFFER_SIZE,
      true
    );
    /* Progress */
    int total = 0;
    int read = -1;
    byte[] buffer = new byte[BUFFER_SIZE];
    try {
      while (read != 0) {
        read = await s.ReadAsync(buffer, 0, buffer.Length);
        total += read;
        if (read == 0) break;
        await fs.WriteAsync(buffer, 0, read);
      }
    } catch (Exception e) {
      fs.Close();
      if (File.Exists(filepath)) {
        Console.WriteLine($"Clearing {filepath}");
        File.Delete(filepath);
      }
      throw;
    }
    fs.Close();
  }
}
