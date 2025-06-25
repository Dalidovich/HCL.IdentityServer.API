using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace HCL.IdentityServer.API.Test.IntegrationTest
{
    public class TestContainerBuilder
    {
        public static readonly string npgsqlUser = "postgres";
        public static readonly string npgsqlPassword = "pg";
        public static readonly string npgsqlDB = "HCL_IdentityServer";

        public static IContainer CreatePostgreSQLContainer()
        {

            return new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("postgres:latest")
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(5433)
                .WithAutoRemove(true)
                .WithPortBinding(5432, true)
                .WithEnvironment("POSTGRES_USER", npgsqlUser)
                .WithEnvironment("POSTGRES_PASSWORD", npgsqlPassword)
                .WithEnvironment("POSTGRES_DB", npgsqlDB)
                .WithTmpfsMount("/pgdata")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("psql -U postgres -c \"select 1\""))
                .Build();
        }
    }
}
