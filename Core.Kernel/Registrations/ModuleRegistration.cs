using System.Reflection;
using Core.Kernel.Constants;

namespace Core.Kernel.Registrations;

public class ModuleRegistration {
  public readonly string name;
  private readonly string prefix_;

  public ModuleRegistration(ModuleType type, Type consumer) {
    prefix_ = type.ToString().ToLower();
    this.consumer = consumer;
    name = Assembly.GetEntryAssembly().GetName().Name;
  }

  public Type consumer { get; }
  public List<Endpoint> endpoints { get; } = new();

  public void AddEndpoint(string name, int concurrency = 10) {
    endpoints.Add(
      new Endpoint {
        name = $"{prefix_}-{name}",
        concurrency = concurrency
      }
    );
  }
}