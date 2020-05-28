using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Data.Entity
{
    public static class DbContextSeeder
    {
        public static void Seed(MeredithDbContext dbContext, UserManager<User> userManager)
        {
            CreateUser(dbContext, userManager);

            CreateArticleCategories(dbContext);

            CreateRecipients(dbContext);

            dbContext.SaveChanges();
        }

        private static void CreateUser(MeredithDbContext dbContext, UserManager<User> userManager)
        {
            var hasAny = dbContext.Users.Any();

            if (hasAny)
            {
                return;
            }

            var admin = new User
            {
                Id = 1,
                UserName = "admin",
                Email = "admin",
                NormalizedUserName = "ADMIN"
            };

            dbContext.Users.Add(admin);

            var hashPassword = userManager.PasswordHasher.HashPassword(admin, "admin");
            admin.SecurityStamp = Guid.NewGuid().ToString();
            admin.PasswordHash = hashPassword;

            dbContext.Users.Add(admin);

            dbContext.UserRoles.Add(new IdentityUserRole<int>
            {
                RoleId = 1,
                UserId = 1
            });
        }

        private static void CreateArticleCategories(MeredithDbContext dbContext)
        {
            var hasAny = dbContext.Categories.OfType<ArticleCategory>().Any();

            if (hasAny)
            {
                return;
            }

            var result = new List<ArticleCategory>
            {
                new ArticleCategory
                {
                    Slug = "priority",
                    Name = "Priority",
                    Description = "Top daily announcements",
                    Priority = 100,
                    Color = "D84E4E",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/Priority_dpmg21.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "answers-at-a-glance",
                    Name = "Answers At A Glance",
                    Description = "Answer commonly-asked questions",
                    Priority = 90,
                    Color = "011D51",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1586189259/Volkswagen/newsletter1/answers-glance_nqilnt.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "events",
                    Name = "Events",
                    Description = "Company-wide event announcements",
                    Priority = 90,
                    Color = "FFBA03",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/Event_vmgpio.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "plant",
                    Name = "Plant",
                    Description = "Factory-wide reminders and announcements",
                    Priority = 80,
                    Color = "8894A0",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/Plant_fc01dz.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "people",
                    Name = "People",
                    Description = "Stories and spotlights",
                    Priority = 80,
                    Color = "01B1EC",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/People_kehshw.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "community",
                    Name = "Community",
                    Description = "Company culture, volunteering, and more",
                    Priority = 80,
                    Color = "52AE31",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/Community_bwl6tk.png"
                    }
                },
                new ArticleCategory
                {
                    Slug = "one-team",
                    Name = "One Team",
                    Description = "One Team update and announcements",
                    Priority = 80,
                    Color = "AD83C4",
                    Image = new CategoryImage
                    {
                        Url =
                            "https://res.cloudinary.com/whynotearth/image/upload/v1585211750/Volkswagen/newsletter1/One_Team_cplk6n.png"
                    }
                }
            };

            dbContext.Categories.AddRange(result);
        }

        private static void CreateRecipients(MeredithDbContext dbContext)
        {
            var hasAny = dbContext.Recipients.Any();

            if (hasAny)
            {
                return;
            }

            var result = new List<Recipient>
            {
                new Recipient
                {
                    FirstName = "First name",
                    LastName = "Last name",
                    CreationDateTime = DateTime.UtcNow,
                    Email = "email@example.com",
                    DistributionGroup = "Group 1"
                },
            };

            dbContext.Recipients.AddRange(result);
        }
    }
}