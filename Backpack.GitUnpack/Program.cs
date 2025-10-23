using APC.GitUnpack;
using APC.GitUnpack.Services;

namespace Backpack.GitUnpack;

public class Program {
  public static void Main(string[] args) {
    IHost host = Host.CreateDefaultBuilder(args)
                     .ConfigureServices(
                       services => {
                         services.AddSingleton<Unpacker>();
                         services.AddHostedService<Worker>();
                       }
                     )
                     .Build();

    host.Run();
  }
}