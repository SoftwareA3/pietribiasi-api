using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using apiPB.Data;
using apiPB.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using apiPB.Authentication;
using apiPB.Repository.Abstraction;
using apiPB.Repository.Implementation;


var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>("BasicAuthentication", null);
    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<ApplicationDbContext> (options => {options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));}); 
    builder.Services.AddScoped<LogService>();
    builder.Services.AddScoped<IRmWorkersFieldRepository, RmWorkersFieldRepository>();
    builder.Services.AddScoped<IVwApiWorkerRepository, VwApiWorkerRepository>();
    builder.Services.AddScoped<IVwApiJobRepository, VwApiJobRepository>();
    builder.Services.AddScoped<IVwApiMoRepository, VwApiMoRepository>();
    builder.Services.AddScoped<IVwApiMostepRepository, VwApiMostepRepository>();
    builder.Services.AddScoped<IVwApiMocomponentRepository, VwApiMocomponentRepository>();
    builder.Services.AddScoped<IVwApiMoStepsComponentRepository, VwApiMoStepsComponentRepository>();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => {options.ConfigObject.PersistAuthorization = false;});
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.Urls.Add("http://localhost:5245");

    app.MapControllers();

    app.Run();
}