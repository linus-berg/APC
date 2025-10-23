namespace Core.Infrastructure;

public class DatabaseFactory {
  private readonly string db_str_;

  public DatabaseFactory(string db_str) {
    db_str_ = db_str;
  }
}