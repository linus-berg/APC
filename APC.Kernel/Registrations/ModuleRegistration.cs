using APC.Kernel.Constants;

namespace APC.Kernel.Registrations;

public class ModuleRegistration {
  private readonly string prefix_;

  public ModuleRegistration(ModuleType type, Type consumer) {
    prefix_ = type.ToString().ToLower();
    this.consumer = consumer;
  }

  public Type consumer { get; }
  public List<Endpoint> endpoints { get; } = new();

  public void AddEndpoint(string name, int concurrency = 10) {
    endpoints.Add(new Endpoint {
      name = $"{prefix_}-{name}",
      concurrency = concurrency
    });
  }
}