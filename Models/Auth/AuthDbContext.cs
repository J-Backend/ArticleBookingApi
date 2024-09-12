using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace api_layaway.Models.Auth
{
    public class AuthDbContext:IdentityDbContext
    {
            public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options){}
    }
}