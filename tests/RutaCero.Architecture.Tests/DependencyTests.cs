using Microsoft.AspNetCore.Mvc;
using RutaCero.Api.Controllers;
using RutaCero.Application.Accounts;
using RutaCero.Domain.ValueObjects;
using RutaCero.Infrastructure.Persistence;

namespace RutaCero.Architecture.Tests;

public sealed class DependencyTests
{
    [Fact]
    public void Domain_has_no_outward_project_dependencies() =>
        Assert.Empty(ProjectReferences(typeof(Money).Assembly));

    [Fact]
    public void Application_only_references_domain() =>
        Assert.All(ProjectReferences(typeof(AccountService).Assembly), x => Assert.Equal("RutaCero.Domain", x));

    [Fact]
    public void Controllers_do_not_depend_on_db_context()
    {
        var controllers = typeof(AccountsController).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ControllerBase)));
        Assert.DoesNotContain(controllers.SelectMany(x => x.GetConstructors()).SelectMany(x => x.GetParameters()),
            x => x.ParameterType == typeof(RutaCeroDbContext));
    }

    private static IEnumerable<string> ProjectReferences(System.Reflection.Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(x => x.Name!).Where(x => x.StartsWith("RutaCero."));
}
