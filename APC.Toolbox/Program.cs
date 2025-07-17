using ACM.Kernel;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Toolbox;

internal class Program {
  private static async Task Main(string[] args) {
    ServiceCollection services = new();
    services.AddStorage();
    services.AddLogging();
    services.AddSingleton<FileSystem>();
    services.AddSingleton<Toolbox>();
    // Build the service provider
    IServiceProvider sp = services.BuildServiceProvider();

    Toolbox tb = sp.GetRequiredService<Toolbox>();

    ParserResult<object>? result = Parser.Default
                                         .ParseArguments<CreateIndexOptions,
                                           DataGeneratorOptions>(args);
    await result
      .MapResult(
        async (CreateIndexOptions opts) => await tb.CreateIndex(opts.processor),
        async (DataGeneratorOptions opts) =>
          await tb.CreateFakeData(opts.processor, opts.files),
        _ => MakeError()
      );
  }

  private static Task<int> MakeError() {
    return Task.FromResult(-1);
  }
}