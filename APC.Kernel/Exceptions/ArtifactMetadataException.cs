namespace APC.Kernel.Exceptions; 

public class ArtifactMetadataException : Exception {
  public ArtifactMetadataException() {
  }

  public ArtifactMetadataException(string? message) : base(message) {
  }

  public ArtifactMetadataException(string? message, Exception? innerException) : base(message, innerException) {
  }
}