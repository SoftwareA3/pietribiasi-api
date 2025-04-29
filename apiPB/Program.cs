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
    builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
    builder.Services.AddScoped<IJobRepository, JobRepository>();
    builder.Services.AddScoped<IMoStepRepository, MoStepRepository>();
    builder.Services.AddScoped<IMostepsMocomponentRepository, MostepsMocomponentRepository>();
    builder.Services.AddScoped<IRegOreRepository, RegOreRepository>();
    builder.Services.AddScoped<IPrelMatRepository, PrelMatRepository>();
    builder.Services.AddScoped<IGiacenzeRepository, GiacenzeRepository>();
    
    // Services
    builder.Services.AddScoped<LogService>();
    builder.Services.AddScoped<IWorkersRequestService, WorkersRequestService>();
    builder.Services.AddScoped<IJobRequestService, JobRequestService>();
    builder.Services.AddScoped<IMoStepRequestService, MoStepRequestService>();
    builder.Services.AddScoped<IMostepsMocomponentRequestService, MostepsMocomponentRequestService>();
    builder.Services.AddScoped<IRegOreRequestService, RegOreRequestService>();
    builder.Services.AddScoped<IPrelMatRequestService, PrelMatRequestService>();
    builder.Services.AddScoped<IGiacenzeRequestService, GiacenzeRequestService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
    
    
    // AutoMappers
    builder.Services.AddAutoMapper(typeof(WorkerMapperFilters));
    builder.Services.AddAutoMapper(typeof(JobMapperFilters));
    builder.Services.AddAutoMapper(typeof(MoStepMapperFilters));
    builder.Services.AddAutoMapper(typeof(MostepsMocomponentMapperFilters));
    builder.Services.AddAutoMapper(typeof(RegOreMapperFilters));
    builder.Services.AddAutoMapper(typeof(PrelMatMapperFilters));
    builder.Services.AddAutoMapper(typeof(GiacenzeMapperFilters));
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => {options.ConfigObject.PersistAuthorization = false;});
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.Urls.Add("http://localhost:5245");

    app.MapControllers();

    app.Run();
}