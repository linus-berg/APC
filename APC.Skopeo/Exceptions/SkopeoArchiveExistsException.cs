namespace APC.Skopeo.Exceptions;

public class SkopeoArchiveExistsException : Exception {
  public SkopeoArchiveExistsException(string message) : base(message) {
  }
}