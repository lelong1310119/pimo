using Autofac;
using Microsoft.Extensions.Hosting;

namespace PismoWebInput.Core;

public class AppModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(AppModule).Assembly)
            .PublicOnly()
            .Where(t => t.Name.EndsWith("Service") && !t.IsAssignableTo<IHostedService>())
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}