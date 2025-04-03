using AMESWEB.Data;
using AMESWEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AMESWEB.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AdmUserGroup>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdmUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Ensuring database migration...");
                await context.Database.MigrateAsync();

                // Seed Companies
                logger.LogInformation("Seeding companies...");
                var companies = new[]
                {
                    new { Id = 1, Code = "COMPA", Name = "CompanyA", RegNo = "REG-001", TaxNo = "TAX-001", IsActive = true },
                    new { Id = 2, Code = "COMPB", Name = "CompanyB", RegNo = "REG-002", TaxNo = "TAX-002", IsActive = true }
                };

                foreach (var company in companies)
                {
                    if (!context.AdmCompany.Any(c => c.CompanyCode == company.Code))
                    {
                        context.AdmCompany.Add(new AdmCompany
                        {
                            CompanyId = Convert.ToByte(company.Id),
                            CompanyCode = company.Code,
                            CompanyName = company.Name,
                            RegistrationNo = company.RegNo,
                            TaxRegistrationNo = company.TaxNo,
                            AddressId = 0,
                            Remarks = "System-seeded company",
                            IsActive = company.IsActive,
                            CreateById = 0, // System-generated
                            CreateDate = DateTime.Now,
                            EditById = null,
                            EditDate = null
                        });
                    }
                }
                await context.SaveChangesAsync();

                // Seed Roles
                logger.LogInformation("Seeding roles...");
                var roles = new[]
                {
                    new{ Id = 1,roleName="Admin", roleCode="ADM" },
                    new{ Id = 2,roleName = "User",roleCode= "USR" },
                    new{ Id = 3,roleName = "Operation Manager",roleCode= "OPM" },
                    new{ Id = 4,roleName = "Account Manager",roleCode= "ACM" }
                };

                foreach (var role in roles)
                {
                    await AddRoleAsync(roleManager, role.Id, role.roleName, role.roleCode);
                }

                // Seed Users
                logger.LogInformation("Seeding users...");
                logger.LogInformation("Seeding admin user...");
                var users = new[]
                {
                    new { Id = 1, FullName="Admin", Email = "admin@xyz.com",UserGroupId=1, Role = "Admin", Code = "ADM001" },
                    new { Id = 2, FullName="Users", Email = "user@xyz.com", UserGroupId=2,Role = "User", Code = "USR001" },
                    new { Id = 3, FullName="Operations", Email = "operationmanager@xyz.com", UserGroupId=3,Role = "Operation Manager", Code = "OPM001" },
                    new { Id = 4, FullName="Account", Email = "accountmanager@xyz.com",UserGroupId=4, Role = "Account Manager", Code = "ACM001" }
            };

                foreach (var user in users)
                {
                    if (await userManager.FindByEmailAsync(user.Email) == null)
                    {
                        var newUser = new AdmUser
                        {
                            Id = Convert.ToInt16(user.Id),
                            FullName = user.FullName,
                            UserName = user.Email,
                            Email = user.Email,
                            UserCode = user.Code,
                            IsActive = true,
                            CreateById = 0,
                            CreateDate = DateTime.Now,
                            EmailConfirmed = true,
                            Remarks = "System-seeded users",
                            UserGroupId = Convert.ToInt16(user.UserGroupId)
                        };

                        var result = await userManager.CreateAsync(newUser, "Admin@123");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(newUser, user.Role);
                        }
                    }
                }

                // Seed Modules and Transactions
                logger.LogInformation("Seeding modules and transactions...");

                // First ensure transaction category exists
                const int defaultCategoryId = 1;
                if (!context.AdmTransactionCategory.Any(c => c.TransCategoryId == defaultCategoryId))
                {
                    context.AdmTransactionCategory.Add(new AdmTransactionCategory
                    {
                        TransCategoryId = defaultCategoryId,
                        TransCategoryCode = "DFLT",
                        TransCategoryName = "Default",
                        IsActive = true,
                        Remarks = "System-seeded transCategory",
                        CreateById = 0,
                        CreateDate = DateTime.Now
                    });
                    await context.SaveChangesAsync();
                }

                var modules = new[]
                {
                    new {
                        Id = 1,
                        Code = "MAS",
                        Name = "Master",
                        SeqNo = (short)1,
                        Transactions = new[] {
                            new { Id = 1, Name = "Country", Code = "CTR" },
                            new { Id = 2, Name = "Company", Code = "CMP" }
                        }
                    },
                    new {
                        Id = 2,
                        Code = "ACC",
                        Name = "Account",
                        SeqNo = (short)2,
                        Transactions = new[] {
                            new { Id = 3, Name = "Invoice", Code = "INV" }
                        }
                    },
                    new {
                        Id = 3,
                        Code = "ADM",
                        Name = "Admin",
                        SeqNo = (short)3,
                        Transactions = new[] {
                            new { Id = 4, Name = "User Group Rights", Code = "UGR" },
                            new { Id = 5, Name = "User Rights", Code = "URT" }
                        }
                    },
                    new {
                        Id = 4,
                        Code = "SET",
                        Name = "Setting",
                        SeqNo = (short)4,
                        Transactions = new[] {
                            new { Id = 6, Name = "Change Password", Code = "CHP" },
                            new { Id = 7, Name = "User Profile", Code = "USP" }
                        }
                    }
                };

                foreach (var module in modules)
                {
                    // Handle module
                    var dbModule = await context.AdmModule
                        .FirstOrDefaultAsync(m => m.ModuleCode == module.Code);

                    if (dbModule == null)
                    {
                        dbModule = new AdmModule
                        {
                            ModuleId = Convert.ToByte(module.Id),
                            ModuleCode = module.Code,
                            ModuleName = module.Name,
                            SeqNo = Convert.ToByte(module.SeqNo),
                            IsActive = true,
                            CreateById = 0,
                            CreateDate = DateTime.Now,
                            Remarks = "System-seeded module"
                        };
                        context.AdmModule.Add(dbModule);
                        await context.SaveChangesAsync();
                    }

                    // Handle transactions with explicit TransactionId
                    short transactionId = 1;
                    foreach (var transaction in module.Transactions)
                    {
                        // Check existence using composite key
                        var exists = await context.AdmTransaction
                            .AnyAsync(t => t.ModuleId == dbModule.ModuleId &&
                                          t.TransactionId == transactionId);

                        if (!exists)
                        {
                            var newTransaction = new AdmTransaction
                            {
                                ModuleId = dbModule.ModuleId,
                                TransactionId = transactionId,
                                TransactionCode = transaction.Code,
                                TransactionName = transaction.Name,
                                TransCategoryId = defaultCategoryId,
                                SeqNo = transactionId,  // Or separate sequence if needed
                                IsActive = true,
                                CreateById = 0,
                                CreateDate = DateTime.Now,
                                Remarks = "System-seeded transaction",
                                IsNumber = false
                            };

                            context.AdmTransaction.Add(newTransaction);
                        }
                        transactionId++;
                    }
                    await context.SaveChangesAsync();
                }

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seeding failed");
                throw;
            }
        }

        private static async Task AddRoleAsync(RoleManager<AdmUserGroup> roleManager, int roleId, string roleName, string roleCode)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new AdmUserGroup
                {
                    Id = Convert.ToInt16(roleId),
                    Name = roleName,
                    UserGroupCode = roleCode.ToUpper(),
                    NormalizedName = roleName.ToUpper(),
                    UserGroupName = roleName,
                    Remarks = "System-seeded roles",
                    IsActive = true,
                    CreateById = 0,
                    CreateDate = DateTime.Now
                };

                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Role creation failed: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}