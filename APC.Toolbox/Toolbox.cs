using ACM.Kernel;
using Foundatio.Storage;

namespace APC.Toolbox;

public class Toolbox {
  private readonly FileSystem fs_;

  public Toolbox(FileSystem fs) {
    fs_ = fs;
  }

  public async Task<int> CreateFakeData(string processor, int amount) {
    int created = 0;
    for (int i = 0; i < amount; i++) {
      string random_file_name = Path.GetRandomFileName();
      await fs_.PutString(Path.Join(processor, random_file_name), i.ToString());
      created++;
    }

    return created;
  }

  public async Task<int> CreateIndex(string processor, int page_size = 10000) {
    PagedFileListResult paged_file_list =
      await fs_.GetPagedFileList(processor, page_size);
    do {
      foreach (FileSpec file in paged_file_list.Files) {
        Console.WriteLine(file.Path);
      }

      if (!paged_file_list.HasMore) {
        break;
      }

      await paged_file_list.NextPageAsync();
    } while (paged_file_list.HasMore);

    return 0;
  }
}