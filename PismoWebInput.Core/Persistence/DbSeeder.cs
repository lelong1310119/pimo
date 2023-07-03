using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Extensions;
using PismoWebInput.Core.Persistence.Contexts;

namespace PismoWebInput.Core.Persistence;

public static class DbSeeder
{
    public static async Task Migrate(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<EfContext>();
        await context.Database.MigrateAsync();
        await IdentityInitialize(serviceProvider);
        await MasterTypeInitialize(serviceProvider);
        await MasterContentTypeInitialize(serviceProvider);
        await PickingStatusInitialize(serviceProvider);
    }

    public static async Task PickingStatusInitialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<EfContext>();
        if (context != null)
        {
            var masterSet = context.PickingStatuses;
            if (await masterSet.AnyAsync()) return;

            var seed = new List<string> { "Front", "Back", "QC", "Error" };

            foreach (var item in seed)
            {
                await masterSet.AddAsync(new PickingStatus
                {
                    PickingStatusID = 1,
                    PickingInstructionNo = item,
                    MFInstructionNo = item,
                }); ;
            }

            await context.SaveChangesAsync();
        }
    }
    public static async Task MasterContentTypeInitialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<EfContext>();
        if (context != null)
        {
            var masterSet = context.MasterTypes;
            if (await masterSet.AnyAsync()) return;

            var seed = new List<string> { "Front", "Back", "QC", "Error" };

            foreach (var item in seed)
            {
                await masterSet.AddAsync(new MasterType
                {
                    Name = item,
                    Description = item
                });
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task MasterTypeInitialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<EfContext>();
        if (context != null)
        {
            var masterSet = context.Masters;
            if (await masterSet.AnyAsync()) return;

            var front = new List<string> { "MQC SR", "MQC Coverlay", "MQC Gold" };
            var back = new List<string>
            {
                "MQC Punch 1", "MQC after Shield film", "MQC after press Shield film", "MQC Punch 2", "MQC Punch 3",
                "MQC Punch 4", "MQC Punch 5", "MQC Punch outline", "MQC Pasting SUS", "MQC SUS Press", "MQC 2DID SUS",
                "MQC E-test"
            };
            var qc = new List<string> { "MS Inspection", "VS Inspection", "OQC", "MQC Baking" };
            var errorCodes = new List<Master>
            {
                new()
                {
                    Code = "1",
                    Name = "Error1",
                    Type = MasterTypeEnum.ErrorCode
                },
                new()
                {
                    Code = "2",
                    Name = "Error2",
                    Type = MasterTypeEnum.ErrorCode
                },
                new()
                {
                    Code = "3",
                    Name = "Error3",
                    Type = MasterTypeEnum.ErrorCode
                },
                new()
                {
                    Code = "4",
                    Name = "Error4",
                    Type = MasterTypeEnum.ErrorCode
                },
            };

            foreach (var item in front)
            {
                await masterSet.AddAsync(new Master
                {
                    Code = item.ToUpper(),
                    Name = item,
                    Type = MasterTypeEnum.Front
                });
            }

            foreach (var item in back)
            {
                await masterSet.AddAsync(new Master
                {
                    Code = item.ToUpper(),
                    Name = item,
                    Type = MasterTypeEnum.Back
                });
            }

            foreach (var item in qc)
            {
                await masterSet.AddAsync(new Master
                {
                    Code = item.ToUpper(),
                    Name = item,
                    Type = MasterTypeEnum.QC
                });
            }

            await masterSet.AddRangeAsync(errorCodes);

            await context.SaveChangesAsync();
        }
    }

    public static async Task IdentityInitialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<EfContext>();
        if (context != null)
        {
            var roles = Enum<AppRoleEnum>.ToNameList;

            foreach (var role in roles)
            {
                var roleStore = new RoleStore<AppRole>(context);

                if (!await context.Roles.AnyAsync(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new AppRole { Name = role, NormalizedName = role.ToUpper() });
                }
            }


            var user = new AppUser
            {
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };


            if (!await context.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Open4me!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(context);
                await userStore.CreateAsync(user);

            }

            await AssignRoles(serviceProvider, user.Id, roles);

            await context.SaveChangesAsync();
        }
    }

    public static async Task<IdentityResult?> AssignRoles(IServiceProvider services, string userId, List<string> roles)
    {
        var userManager = services.GetService<UserManager<AppUser>>();
        if (userManager == null) return null;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;
        var result = await userManager.AddToRolesAsync(user, roles);
        return result;
    }
}