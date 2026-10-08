using System.Reflection;
using System.Xml.Linq;

namespace NearMe.UnitTests.Architecture;

/// <summary>
/// Enforces the approved modular-monolith dependency direction (docs/adr/0002):
/// Api → Application/Infrastructure, Application → Domain, Infrastructure → Application/Domain,
/// with Shared available to all. A violation fails the build pipeline instead of relying on review.
/// </summary>
public sealed class ProjectDependencyTests
{
    private static readonly Dictionary<string, string[]> AllowedProjectReferences = new()
    {
        ["NearMe.Shared"] = [],
        ["NearMe.Domain"] = ["NearMe.Shared"],
        ["NearMe.Application"] = ["NearMe.Domain", "NearMe.Shared"],
        ["NearMe.Infrastructure"] = ["NearMe.Application", "NearMe.Domain", "NearMe.Shared"],
        ["NearMe.Api"] = ["NearMe.Application", "NearMe.Infrastructure", "NearMe.Shared"],
    };

    public static TheoryData<string> SourceProjects()
    {
        var data = new TheoryData<string>();
        foreach (var project in AllowedProjectReferences.Keys)
        {
            data.Add(project);
        }

        return data;
    }

    [Fact]
    public void Every_source_project_has_a_dependency_rule()
    {
        var projectsOnDisk = Directory.GetDirectories(BackendPaths.SourceRoot)
            .Select(directory => Path.GetFileName(directory))
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(AllowedProjectReferences.Keys.OrderBy(name => name).ToArray(), projectsOnDisk);
    }

    [Theory]
    [MemberData(nameof(SourceProjects))]
    public void Project_references_only_allowed_projects(string project)
    {
        var disallowed = ReadProjectReferences(project)
            .Except(AllowedProjectReferences[project])
            .ToArray();

        Assert.True(
            disallowed.Length == 0,
            $"{project} must not reference: {string.Join(", ", disallowed)}");
    }

    [Fact]
    public void Domain_has_no_package_or_framework_dependencies()
    {
        var document = XDocument.Load(BackendPaths.ProjectFile("NearMe.Domain"));

        var externalDependencies = document.Descendants()
            .Where(element => element.Name.LocalName is "PackageReference" or "FrameworkReference")
            .Select(element => (string?)element.Attribute("Include"))
            .ToArray();

        Assert.True(
            externalDependencies.Length == 0,
            $"NearMe.Domain must stay free of external dependencies but references: {string.Join(", ", externalDependencies)}");
    }

    [Theory]
    [InlineData("NearMe.Domain", new[] { "NearMe.Application", "NearMe.Infrastructure", "NearMe.Api", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore" })]
    [InlineData("NearMe.Application", new[] { "NearMe.Infrastructure", "NearMe.Api", "Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore.SqlServer", "Azure." })]
    [InlineData("NearMe.Shared", new[] { "NearMe.Domain", "NearMe.Application", "NearMe.Infrastructure", "NearMe.Api" })]
    public void Compiled_assembly_does_not_reference_forbidden_assemblies(string assemblyName, string[] forbiddenPrefixes)
    {
        var references = Assembly.Load(new AssemblyName(assemblyName))
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        var violations = references
            .Where(reference => forbiddenPrefixes.Any(prefix => reference.StartsWith(prefix, StringComparison.Ordinal)))
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"{assemblyName} must not reference: {string.Join(", ", violations)}");
    }

    private static IEnumerable<string> ReadProjectReferences(string project)
    {
        var document = XDocument.Load(BackendPaths.ProjectFile(project));

        return document.Descendants()
            .Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', '/')));
    }
}
