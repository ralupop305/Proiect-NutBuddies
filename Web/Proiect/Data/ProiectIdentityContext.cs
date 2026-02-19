using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Proiect.Data;

public class ProiectIdentityContext : IdentityDbContext<IdentityUser>
{
    public ProiectIdentityContext(DbContextOptions<ProiectIdentityContext> options)
        : base(options)
    {
    }
}
