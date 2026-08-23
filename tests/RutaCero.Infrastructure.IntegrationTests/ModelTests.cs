using Microsoft.EntityFrameworkCore;
using RutaCero.Infrastructure.Persistence;
using RutaCero.Domain.Users;

namespace RutaCero.Infrastructure.IntegrationTests;

public sealed class ModelTests
{
    [Fact]
    public void Ef_model_can_be_created_without_schema_mutation()
    {
        var options = new DbContextOptionsBuilder<RutaCeroDbContext>()
            .UseNpgsql("Host=localhost;Database=rutacero;Username=test;Password=test").Options;
        using var context = new RutaCeroDbContext(options);
        Assert.NotEmpty(context.Model.GetEntityTypes());
    }

    [Fact]
    public void Refresh_token_model_declares_user_foreign_key()
    {
        var options=new DbContextOptionsBuilder<RutaCeroDbContext>().UseNpgsql("Host=localhost;Database=rutacero;Username=test;Password=test").Options;
        using var context=new RutaCeroDbContext(options);
        var entity=context.Model.FindEntityType(typeof(RefreshToken));
        Assert.Contains(entity!.GetForeignKeys(),x=>x.PrincipalEntityType.ClrType==typeof(User)&&x.Properties.Single().Name==nameof(RefreshToken.UserId));
    }
}
