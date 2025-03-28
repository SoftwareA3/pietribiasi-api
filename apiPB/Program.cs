using Microsoft.EntityFrameworkCore;
using apiPB.Data;
using apiPB.Services;
using Microsoft.AspNetCore.Authentication;
using apiPB.Authentication;
using apiPB.Repository.Abstraction;
using apiPB.Repository.Implementation;
using apiPB.Mappers.Filters;
using apiPB.Mappers.Filter;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Request.Implementation;
using apiPB.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>("BasicAuthentication", null);
    
    // Authorization
    builder.Services.AddAuthorization();
    
    // Controllers
    builder.Services.AddControllers();
    
    builder.Services.AddEndpointsApiExplorer();
    
    // Swagger
    builder.Services.AddSwaggerGen();
    
    // DbContext
    builder.Services.AddDbContext<ApplicationDbContext> (options => {options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));}); 
    
    // Repositories
    builder.Services.AddScoped<IRmWorkersFieldRepository, RmWorkersFieldRepository>();
    builder.Services.AddScoped<IVwApiWorkerRepository, VwApiWorkerRepository>();
    builder.Services.AddScoped<IVwApiJobRepository, VwApiJobRepository>();
    builder.Services.AddScoped<IVwApiMoRepository, VwApiMoRepository>();
    builder.Services.AddScoped<IVwApiMostepRepository, VwApiMostepRepository>();
    builder.Services.AddScoped<IVwApiMocomponentRepository, VwApiMocomponentRepository>();
    builder.Services.AddScoped<IVwApiMoStepsComponentRepository, VwApiMoStepsComponentRepository>();
    
    // Services
    builder.Services.AddScoped<LogService>();
    builder.Services.AddScoped<IMocomponentRequestService, MocomponentRequestService>();
    builder.Services.AddScoped<IMoRequestService, MoRequestService>();
    builder.Services.AddScoped<IMostepRequestService, MostepRequestService>();
    builder.Services.AddScoped<IMoStepsComponentRequestService, MoStepsComponentRequestService>();
    builder.Services.AddScoped<IPasswordWorkersRequestService, PasswordWorkersRequestService>();
    
    
    // AutoMappers
    builder.Services.AddAutoMapper(typeof(WorkerFiltersMapper));
    builder.Services.AddAutoMapper(typeof(JobFiltersMapper));
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