using Serilog;
using Vocational.Assessment.Api.Middlewares;
using Vocational.Assessment.Api.Infrastructure.ExceptionHandling;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando el microservicio Vocational Assessment...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithEnvironmentName()
        // En producción CompactJsonFormatter
        // .WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter())
        .WriteTo.Console());

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();


    var app = builder.Build();


    app.UseExceptionHandler();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) =>
            httpContext.Request.Path.StartsWithSegments("/health")
                ? Serilog.Events.LogEventLevel.Verbose
                : Serilog.Events.LogEventLevel.Information;
    });

    app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fallo crítico: El microservicio no pudo arrancar");
}
finally
{
    Log.CloseAndFlush();
}