using Microsoft.OpenApi;
using API.Common;
using API.Interfaces;
using API.Mapper;
using API.Services;
using API.Services.InforErp;
using DotNetEnv;

// Load local.env before CreateBuilder so env vars are available to IConfiguration.
// In Docker, env vars are injected by the container runtime — local.env won't be
// present and this line is a no-op.
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "local.env");
if (File.Exists(envFile))
    Env.NoClobber().Load(envFile);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("InforApi")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Salesforce Infor Erp API",
            Version = "v1"
        });

    // Add custom header parameter "token"
    c.AddSecurityDefinition("token", new OpenApiSecurityScheme
    {
        Name = "token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter your Salesforce OAuth token (no 'Bearer' prefix needed)"
    });

    // Require the header globally for all endpoints
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("token"),
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Register configuration manager
builder.Services.AddSingleton<IAppSettingsManager, AppSettingsManager>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IItemLotLocationService, ItemLotLocationService>();
builder.Services.AddScoped<IItemsSearchService, ItemsSearchService>();
builder.Services.AddScoped<ITaxLookupService, TaxLookupService>();
builder.Services.AddScoped<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<IInforErpService, InforErpService>();
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<IJobVarianceService, JobVarianceService>();
builder.Services.AddScoped<ILookupService, LookupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
