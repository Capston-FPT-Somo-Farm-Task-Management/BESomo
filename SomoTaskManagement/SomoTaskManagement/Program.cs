using Microsoft.OpenApi.Models;
using SomoTaskManagement.Data.Mapper;
using SomoTaskManagement.Infrastructure.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using Google.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SomoTaskManagement", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token only",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.DescribeAllParametersInCamelCase();
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new string[] { } } });
});

builder.Services.RegisterContextDb(builder.Configuration);
builder.Services.RegisterDI();
builder.Services.RegisterTokenBear(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.RegisterCache(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(MapperApplication));

builder.Services.RegisterQuartz();

string filePath = "SecretKey/somotaskmanagement-firebase-adminsdk-z13iv-aa59ff5b48.json";

FirebaseApp appFirebase = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(filePath)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
;

app.RegisterWebSocket();


app.UseSwaggerUI();

app.UseSwagger();

app.UseCors(options => options
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();