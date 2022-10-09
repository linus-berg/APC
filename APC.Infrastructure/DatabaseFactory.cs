namespace APC.Infrastructure;

public class DatabaseFactory {
  private readonly string DB_STR_;

  public DatabaseFactory(string db_str) {
    DB_STR_ = db_str;
  }
}