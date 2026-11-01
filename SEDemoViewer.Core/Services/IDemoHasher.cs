namespace SEDemoViewer.Core.Services;

public interface IDemoHasher
{
    /// <summary>
    /// Computes a fast hash of the file length plus its first
    /// and last 1MiB as lowercase hex.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    string ComputeQuickHash(string path);
    
    /// <summary>
    /// Computes a hash of the entire file, as lowercase hex. Used only to confirm duplicates.
    /// </summary>
    string ComputeFullHash(string path);
}