using api_layaway.Helpers;
using api_layaway.Interfaces;
using api_layaway.Models;
using api_layaway.Services;
using Microsoft.EntityFrameworkCore;
// Identity imports
using Microsoft.AspNetCore.Identity;
using api_layaway.Models.Auth;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;


var builder = WebApplication.CreateBuilder(args);
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new InvalidOperationException("IdentityConnection string is not found");
}

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(identityConnectionString));



// DbContext settings---------------------------------------start
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(defaultConnectionString))
{
    throw new InvalidOperationException("DefaultConnection string is not found");
}

builder.Services.AddDbContext<LayawayDbContext>(options => options.UseSqlServer(defaultConnectionString));
// DbContext settings---------------------------------------start


// Services settings---------------------------------------start
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
builder.Services.AddScoped(typeof(IHttpResult<>), typeof(HttpResult<>));
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILayawayService, LayawayService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
// Services settings---------------------------------------end


builder.Services.AddControllers();


builder.Services.AddAuthorization();
// Identity settings---------------------------------------end
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
.AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Cors settings-------------------------------------------start
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// Cors settings-------------------------------------------end

builder.Services.AddSwaggerGen(options=>{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme{
    
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

});

// App settings---------------------------------------start
var app = builder.Build();

app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();

    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Layaway API v1");
            c.RoutePrefix = string.Empty; 
    });
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// App settings---------------------------------------end

app.Run();



