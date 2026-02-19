using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Products");
    options.Conventions.AllowAnonymousToPage("/Products/Index");
    options.Conventions.AllowAnonymousToPage("/Products/Details");
    options.Conventions.AuthorizeFolder("/Customers", "AdminPolicy");

});

// App data context (categories, products, orders, etc.)
builder.Services.AddDbContext<NutBuddiesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("NutBuddiesContext")));

// Identity context (authentication/authorization tables)
builder.Services.AddDbContext<ProiectIdentityContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("IdentityContextConnection")));

// Identity + Roles
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ProiectIdentityContext>();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Keep Identity cookies as the default (set by AddDefaultIdentity) and just add JWT for APIs
    builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

 

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    

    var ralucaAdminEmail = "ralucaAdmin@gmail.com";
    var ralucaAdminUser = await userManager.FindByEmailAsync(ralucaAdminEmail);

    if (ralucaAdminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = ralucaAdminEmail,
            Email = ralucaAdminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, "ParolaAdmin123!");

        if (result.Succeeded)
        {
            // atribuire rol de Admin
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    var admin2Email = "raluca@gmail.com";

    var admin2User = await userManager.FindByEmailAsync(admin2Email);

    if (admin2User != null)
    {
        if (!await userManager.IsInRoleAsync(admin2User, "Admin"))
        {
            await userManager.AddToRoleAsync(admin2User, "Admin");
        }
    }

    var regularUserEmail = "client@gmail.com";
    var regularUser = await userManager.FindByEmailAsync(regularUserEmail);

    if (regularUser == null)
    {
        var newUser = new IdentityUser
        {
            UserName = regularUserEmail,
            Email = regularUserEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newUser, "User123!");

    }
}



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    
    
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();


app.Run();