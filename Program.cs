using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//creation of policy to allow single page application framework(FRONT END) 
builder.Services.AddCors((options)=>
{
    options.AddPolicy("DevCors",(coreBuilder)=>{
        coreBuilder.WithOrigins("http://localhost:4200","http://localhost:3000","http://localhost:8000")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
    });
    options.AddPolicy("ProdCors",(coreBuilder)=>{
        coreBuilder.WithOrigins("https://myProductionSite.com")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
    });
});
builder.Services.AddScoped<IUserRepository,UserRepository>();//establish scoped connection i.e.Method call

string? tokenKeyString=builder.Configuration.GetSection("AppSettings:TokenKey").Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options=>{
           options.TokenValidationParameters=new TokenValidationParameters()
           {
            ValidateIssuerSigningKey=true,
            IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                   (tokenKeyString!=null?tokenKeyString:""
                )),
            ValidateIssuer=false,
            ValidateAudience=false
           };
       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
