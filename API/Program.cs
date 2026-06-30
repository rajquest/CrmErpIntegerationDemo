using Microsoft.OpenApi;
using API.Common;
using API.Interfaces;
using API.Mapper;
using API.Services;
using API.Services.Auth;
using API.Services.Infor;
using API.Services.InforErp;
using API.Services.Lookup;
using DotNetEnv;

// Load local.env before CreateBuilder so env vars are available to IConfiguration.
// In Docker, env vars are injected by the container runtime — local.env won't be
// present and this line is a no-op.
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "local.env");
if (File.Exists(envFile))
    Env.NoClobber().Load(envFile);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var disableSslValidation = builder.Configuration.GetValue<bool>("InforErp:DisableSslValidation");
builder.Services.AddHttpClient("InforApi")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = disableSslValidation
            ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            : null
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

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Register configuration manager
builder.Services.AddSingleton<IAppSettingsManager, AppSettingsManager>();
builder.Services.AddSingleton<IInforTokenService, InforTokenService>();
builder.Services.AddSingleton<ISalesforceTokenService, SalesforceTokenService>();
builder.Services.AddScoped<IItemLotLocationService, ItemLotLocationService>();
builder.Services.AddScoped<IItemsSearchService, ItemsSearchService>();
builder.Services.AddScoped<ITaxLookupService, TaxLookupService>();
builder.Services.AddScoped<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<IInforErpService, InforErpService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvoiceEnrichmentService, InvoiceEnrichmentService>();
builder.Services.AddScoped<IJobVarianceService, JobVarianceService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();

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
