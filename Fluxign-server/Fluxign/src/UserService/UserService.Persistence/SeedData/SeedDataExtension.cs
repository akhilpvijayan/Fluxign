using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Persistence.Data;

namespace UserService.Persistence.SeedData
{
    public class SeedDataExtension
    {
        public static void Initialize(UserServiceDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (!dbContext.UserRoles.Where(x=>x.Role == "User").Any())
            {
                dbContext.UserRoles.AddRange(
                    new UserRole { Role = "User" }
                );
                dbContext.SaveChanges();
            }
            if (!dbContext.UserRoles.Where(x => x.Role == "Client").Any())
            {
                dbContext.UserRoles.AddRange(
                    new UserRole { Role = "Client" }
                );
                dbContext.SaveChanges();
            }
        }
    }
}
