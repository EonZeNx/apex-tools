namespace EonZeNx.ApexTools.Managers;

public class ApexMultiPathManager
{
    private IEnumerable<string> Paths { get; }

    public ApexMultiPathManager(IEnumerable<string> paths)
    {
        Paths = paths;
    }
    
    // ProcessPaths that performs a function on each path
    public void ProcessPaths()
    {
        foreach (var path in Paths)
        {
            var pathManager = new ApexPathManager(path);
            pathManager.ProcessPath();
        }
    }
}
