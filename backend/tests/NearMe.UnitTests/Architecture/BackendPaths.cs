namespace NearMe.UnitTests.Architecture;

internal static class BackendPaths
{
    /// <summary>Finds the backend root (the folder containing NearMe.sln) from the test output directory.</summary>
    public static string BackendRoot { get; } = FindBackendRoot();

    public static string SourceRoot => Path.Combine(BackendRoot, "src");

    public static string ProjectFile(string projectName) =>
        Path.Combine(SourceRoot, projectName, $"{projectName}.csproj");

    private static string FindBackendRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NearMe.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Could not locate NearMe.sln above the test output directory.");
    }
}
