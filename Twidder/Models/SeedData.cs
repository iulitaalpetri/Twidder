﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Twidder.Data;

namespace Twidder.Models
{
    public static class SeedData
    {

        public static void Initialize(IServiceProvider
       serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un 
                //rol
                // insemnand ca a fost rulat codul 
                // De aceea facem return pentru a nu insera rolurile 
                // inca o data
                // Acesta metoda trebuie sa se execute o singura data 
                if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }

                // CREAREA ROLURILOR IN BD
                // adaugam rolurile de admin si user
                context.Roles.AddRange(
                new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7212", Name = "User", NormalizedName = "User".ToUpper() }
                );

                // o noua instanta pe care o vom utiliza pentru 
                //crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                var hasher = new PasswordHasher<ApplicationUser>();

                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                    // primary key
                    UserName = "admin",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    UsernameChangeLimit = 100,
                    Email = "admin@test.com",
                    NormalizedEmail = "ADMIN@TEST.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    ProfilePictureFilePath = "wwwroot/upload/bpet0lwj.n32download.png",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },
                            new ApplicationUser
                            {
                                Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                                // primary key
                                UserName = "user",
                                NormalizedUserName = "user@TEST.COM",
                                UsernameChangeLimit = 100,
                                Email = "user@test.com",
                                NormalizedEmail = "user1@TEST.COM",
                                EmailConfirmed = true,
                                PhoneNumberConfirmed = true,
                                ProfilePictureFilePath = "wwwroot/upload/bpet0lwj.n32download.png",
                                PasswordHash = hasher.HashPassword(null, "Admin21!")
                            }

               ) ;

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                },

               new IdentityUserRole<string>
               {
                   RoleId = "2c5e174e-3b0e-446f-86af483d56fd7212",
                   UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
               }
                );
                context.SaveChanges();
            }
        }
    }
}