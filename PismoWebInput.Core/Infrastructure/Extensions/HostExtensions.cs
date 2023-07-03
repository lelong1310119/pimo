using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Persistence;

namespace PismoWebInput.Core.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task Init(this IHost host)
        {
            host.EnsureAutoMapper();

            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            await DbSeeder.Migrate(serviceProvider);
        }
    }
}
