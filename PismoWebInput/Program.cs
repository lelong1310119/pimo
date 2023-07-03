using Autofac.Extensions.DependencyInjection;
using Autofac;
using Hellang.Middleware.ProblemDetails;
using Serilog;
using PismoWebInput.Core;
using PismoWebInput.Core.Infrastructure.Extensions;
using OfficeOpenXml;

StartupExtensions.ConfigureLogging();

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    builder.Logging.ClearProviders().SetMinimumLevel(LogLevel.Debug);
    builder.Host
        .UseSerilog()
        .ConfigureHostOptions(x => x.ShutdownTimeout = TimeSpan.FromSeconds(15))
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>(c => c.ConfigureContainer());

    builder.ConfigureServices();
    
    var app = builder.Build();
    app.UseProblemDetails();
    app.UseExceptionHandler("/Home/Error");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSession();
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    await app.Init();
    await app.RunAsync();

    Log.Information("Host terminated successfully");
}
catch (Exception e)
{
    Log.CloseAndFlush();
}
finally
{
    Log.CloseAndFlush();
}