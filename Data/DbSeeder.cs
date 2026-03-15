using ELearningPlatform.Models;
using Microsoft.AspNetCore.Identity;

namespace ELearningPlatform.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Instructor", "Student" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = new ApplicationUser
            {
                UserName = "admin@elearning.com",
                Email = "admin@elearning.com",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var instructorUser = new ApplicationUser
            {
                UserName = "instructor@elearning.com",
                Email = "instructor@elearning.com",
                FirstName = "Jane",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var iUser = await userManager.FindByEmailAsync(instructorUser.Email);
            if (iUser == null)
            {
                var createInstructor = await userManager.CreateAsync(instructorUser, "Instructor123!");
                if (createInstructor.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructorUser, "Instructor");
                    iUser = await userManager.FindByEmailAsync(instructorUser.Email);
                }
            }

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Web Development", Description = "Learn to build websites and web applications" },
                    new Category { Name = "Data Science", Description = "Learn data analysis, machine learning, and AI" },
                    new Category { Name = "Design", Description = "Learn graphic design, UI/UX, and more" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                if (!context.Courses.Any() && iUser != null)
                {
                    var courses = new List<Course>
                    {
                        new Course 
                        { 
                            Title = "The Complete Web Developer Bootcamp", 
                            Description = "Learn HTML, CSS, JavaScript, React, Node, and more. A comprehensive guide to full-stack development.",
                            Price = 0m,
                            CategoryId = categories[0].Id,
                            InstructorId = iUser.Id,
                            ImageUrl = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=600&q=80"
                        },
                        new Course 
                        { 
                            Title = "Python for Data Science and Machine Learning", 
                            Description = "Master data analysis with Pandas, NumPy, and Machine Learning with Scikit-Learn.",
                            Price = 0m,
                            CategoryId = categories[1].Id,
                            InstructorId = iUser.Id,
                            ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=600&q=80"
                        },
                        new Course 
                        { 
                            Title = "UI/UX Design Masterclass", 
                            Description = "Learn to design beautiful, engaging user interfaces in Figma and Adobe XD.",
                            Price = 0m,
                            CategoryId = categories[2].Id,
                            InstructorId = iUser.Id,
                            ImageUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=600&q=80"
                        },
                        new Course 
                        { 
                            Title = "Advanced React and Next.js", 
                            Description = "Take your React skills to the next level by building production-ready apps with Next.js.",
                            Price = 0m,
                            CategoryId = categories[0].Id,
                            InstructorId = iUser.Id,
                            ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=600&q=80"
                        }
                    };
                    context.Courses.AddRange(courses);
                    await context.SaveChangesAsync();

                    // Seed lessons for each course
                    if (!context.Lessons.Any())
                    {
                        var lessons = new List<Lesson>
                        {
                            // Course 1: Web Developer Bootcamp
                            new Lesson { CourseId = courses[0].Id, OrderIndex = 1, Title = "Welcome & Course Overview", Description = "Get a full overview of what you'll build and learn in this bootcamp. We cover the full-stack toolkit you'll master.", VideoUrl = "https://www.youtube.com/watch?v=zJSY8tbf_ys" },
                            new Lesson { CourseId = courses[0].Id, OrderIndex = 2, Title = "HTML5 Fundamentals", Description = "Learn the building blocks of the web: HTML tags, semantic elements, forms, and document structure.", VideoUrl = "https://www.youtube.com/watch?v=UB1O30fR-EE" },
                            new Lesson { CourseId = courses[0].Id, OrderIndex = 3, Title = "CSS3 & Flexbox Layout", Description = "Style your pages like a pro with CSS3, Flexbox, and the box model. Make your UI responsive from day one.", VideoUrl = "https://www.youtube.com/watch?v=yU5-ble302U" },
                            new Lesson { CourseId = courses[0].Id, OrderIndex = 4, Title = "JavaScript Essentials", Description = "Learn JavaScript fundamentals: variables, functions, DOM manipulation, events, and ES6+ features.", VideoUrl = "https://www.youtube.com/watch?v=W6NZfCO5SIk" },
                            new Lesson { CourseId = courses[0].Id, OrderIndex = 5, Title = "Building Your First Full-Stack App", Description = "Put it all together — build and deploy a full-stack web app using Node.js, Express, and a database.", VideoUrl = "https://www.youtube.com/watch?v=Oe421EPjeBE" },

                            // Course 2: Python for Data Science
                            new Lesson { CourseId = courses[1].Id, OrderIndex = 1, Title = "Introduction to Python", Description = "Master Python syntax, data types, lists, dicts, loops, functions, and OOP — everything you need for data science.", VideoUrl = "https://www.youtube.com/watch?v=rfscVS0vtbw" },
                            new Lesson { CourseId = courses[1].Id, OrderIndex = 2, Title = "Data Wrangling with Pandas", Description = "Learn how to load, clean, merge, and manipulate datasets using the Pandas library — the industry standard.", VideoUrl = "https://www.youtube.com/watch?v=vmEHCJofslg" },
                            new Lesson { CourseId = courses[1].Id, OrderIndex = 3, Title = "Data Visualisation with Matplotlib", Description = "Create stunning charts, plots, and graphs to reveal insights from your data using Matplotlib and Seaborn.", VideoUrl = "https://www.youtube.com/watch?v=a9UrKTVEeZA" },
                            new Lesson { CourseId = courses[1].Id, OrderIndex = 4, Title = "Machine Learning with Scikit-Learn", Description = "Implement regression, classification, clustering, and evaluation metrics using Scikit-Learn.", VideoUrl = "https://www.youtube.com/watch?v=0Lt9w-BxKFQ" },
                            new Lesson { CourseId = courses[1].Id, OrderIndex = 5, Title = "Deep Learning Intro with TensorFlow", Description = "Get started with neural networks and deep learning using TensorFlow and Keras.", VideoUrl = "https://www.youtube.com/watch?v=aircAruvnKk" },

                            // Course 3: UI/UX Design Masterclass
                            new Lesson { CourseId = courses[2].Id, OrderIndex = 1, Title = "Design Thinking & UX Process", Description = "Learn how to empathise with users, define problems, ideate solutions, prototype and test — the design thinking cycle.", VideoUrl = "https://www.youtube.com/watch?v=_r0VX-aU_T8" },
                            new Lesson { CourseId = courses[2].Id, OrderIndex = 2, Title = "Typography & Color Theory", Description = "Master the fundamentals of typography: font pairing, scale, and hierarchy, plus colour theory for professional design.", VideoUrl = "https://www.youtube.com/watch?v=sByzHoiYFX0" },
                            new Lesson { CourseId = courses[2].Id, OrderIndex = 3, Title = "Wireframing in Figma", Description = "Learn to create wireframes, component libraries, and interactive prototypes directly in Figma.", VideoUrl = "https://www.youtube.com/watch?v=FTFaQWZBqQ8" },
                            new Lesson { CourseId = courses[2].Id, OrderIndex = 4, Title = "Usability Testing", Description = "Run user research sessions and usability tests to gather feedback and iteratively improve your designs.", VideoUrl = "https://www.youtube.com/watch?v=7gLfASBpCE4" },

                            // Course 4: Advanced React & Next.js
                            new Lesson { CourseId = courses[3].Id, OrderIndex = 1, Title = "React Hooks Deep Dive", Description = "Master useState, useEffect, useContext, useReducer and custom hooks to write clean, powerful React components.", VideoUrl = "https://www.youtube.com/watch?v=hQAHSlTtcmY" },
                            new Lesson { CourseId = courses[3].Id, OrderIndex = 2, Title = "State Management with Redux Toolkit", Description = "Manage complex application state efficiently with Redux Toolkit, slices, and async thunks.", VideoUrl = "https://www.youtube.com/watch?v=bbkBuqC1rU4" },
                            new Lesson { CourseId = courses[3].Id, OrderIndex = 3, Title = "Next.js Pages & App Router", Description = "Build server-rendered and statically generated pages using Next.js's file-based routing and App Router.", VideoUrl = "https://www.youtube.com/watch?v=ZVnjOPwW4ZA" },
                            new Lesson { CourseId = courses[3].Id, OrderIndex = 4, Title = "API Routes & Server Actions", Description = "Build secure backend API endpoints and server actions directly inside your Next.js application.", VideoUrl = "https://www.youtube.com/watch?v=wm5gMKuwSYk" },
                            new Lesson { CourseId = courses[3].Id, OrderIndex = 5, Title = "Deploying to Production", Description = "Deploy your Next.js app to Vercel, set up a CI/CD pipeline, and configure environment variables for production.", VideoUrl = "https://www.youtube.com/watch?v=2HBIzEx6IZA" },
                        };
                        context.Lessons.AddRange(lessons);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
