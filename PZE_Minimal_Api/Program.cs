﻿using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();



var app = builder.Build();


app.UseCors(builder => builder
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();





#region Users
//User Dump data
List< User > users = new List<User>();
users.Add(new User { Id = Guid.NewGuid(), Name = "Pawel", Surname="nazwisko", Email = "pawel@gmail.com", Password = "pawel", Role = "Student" });
users.Add(new User { Id = Guid.NewGuid(), Name = "Adam", Surname = "nazwisko", Email = "adam@gmail.com", Password = "adam", Role = "Teacher" });
users.Add(new User { Id = Guid.NewGuid(), Name = "Tomek", Surname = "nazwisko", Email = "tomek@gmail.com", Password = "tomek", Role = "Admin" });

app.MapPost("/GetJwt", (UserDto userDto) =>
{
    var user = users.FirstOrDefault(u => u.Email == userDto.Email && u.Password == userDto.Password);
    if (user != null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_to_generate_jwt_tokens"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ),
            }),
            Expires = DateTime.UtcNow.AddHours(6),  

            SigningCredentials = credentials
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return Results.Ok(jwtToken);
    }
    else {
        return Results.Unauthorized();
    }
});


app.MapPost("/Register-user", (RegisterDto loginDto) => {
    users.Add(new User { Id = Guid.NewGuid(), Name = loginDto.Name, Surname = loginDto.Surname, Email = loginDto.Email, Password = loginDto.Password, Role = "Student" });
    return ("Register user ok");
});

app.MapGet("/GetAllUsers", () =>
{
    return (users);

});
#endregion




#region courses 
app.MapGet("/UserCourses", () =>
{

});

app.MapGet("/AllCourses", () =>
{

});
#endregion

app.Run();


record RegisterDto (string Name, string Surname, string Email, string Password);
record UserDto(string Email, string Password);




public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
