using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace NearMe.UnitTests.Architecture;

/// <summary>
/// Controllers stay thin: they may depend on Application use cases, never on Infrastructure
/// types such as NearMeDbContext. (No controllers exist in Phase 1; this guards every future one.)
/// </summary>
public sealed class ControllerDependencyTests
{
    private const string InfrastructureAssembly = "NearMe.Infrastructure";

    [Fact]
    public void Controllers_do_not_depend_on_infrastructure_types()
    {
        var controllers = typeof(Program).Assembly.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

        var violations = new List<string>();

        foreach (var controller in controllers)
        {
            var dependencyTypes = controller.GetConstructors()
                .SelectMany(constructor => constructor.GetParameters())
                .Select(parameter => parameter.ParameterType)
                .Concat(controller
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Select(field => field.FieldType));

            violations.AddRange(dependencyTypes
                .Where(type => type.Assembly.GetName().Name == InfrastructureAssembly)
                .Select(type => $"{controller.Name} -> {type.FullName}"));
        }

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }
}
