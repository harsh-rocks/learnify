using ELearningPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Data
{
    public static class CourseDataSeeder
    {
        public static async Task SeedNewCoursesAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            // Get an instructor user (Jane Doe created in DbSeeder)
            var instructor = await userManager.FindByEmailAsync("instructor@elearning.com");
            if (instructor == null) return;

            // Ensure categories exist
            var aiCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Artificial Intelligence");
            if (aiCategory == null)
            {
                aiCategory = new Category { Name = "Artificial Intelligence", Description = "Learn Artificial Intelligence, Machine Learning, and Deep Learning" };
                context.Categories.Add(aiCategory);
            }
            
            var mlCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Machine Learning");
            if (mlCategory == null)
            {
                mlCategory = new Category { Name = "Machine Learning", Description = "Core machine learning algorithms and tools" };
                context.Categories.Add(mlCategory);
            }

            var dlCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Deep Learning");
            if (dlCategory == null)
            {
                dlCategory = new Category { Name = "Deep Learning", Description = "Neural networks, CNNs, RNNs, and more" };
                context.Categories.Add(dlCategory);
            }

            var nlpCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "NLP / AI");
            if (nlpCategory == null)
            {
                nlpCategory = new Category { Name = "NLP / AI", Description = "Natural Language Processing and LLMs" };
                context.Categories.Add(nlpCategory);
            }

            var aiDevCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "AI Development");
            if (aiDevCategory == null)
            {
                aiDevCategory = new Category { Name = "AI Development", Description = "Building applications with AI tools" };
                context.Categories.Add(aiDevCategory);
            }

            var programmingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Programming");
            if (programmingCategory == null)
            {
                // Fallback to existing or create new
                programmingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Web Development") 
                    ?? new Category { Name = "Programming", Description = "Programming languages and foundations" };
            }

            var javaCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Java Development");
            if (javaCategory == null)
            {
                javaCategory = new Category { Name = "Java Development", Description = "Java programming and ecosystem" };
                context.Categories.Add(javaCategory);
            }

            var backendCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Backend Development");
            if (backendCategory == null)
            {
                backendCategory = new Category { Name = "Backend Development", Description = "Server-side and API development" };
                context.Categories.Add(backendCategory);
            }

            await context.SaveChangesAsync();

            // Check if AI Gen Course exists to prevent duplicate seeding
            var existingCourse = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Generative AI & Prompt Engineering Masterclass");
            if (existingCourse != null) return;

            var newCourses = new List<Course>
            {
                // AI / Machine Learning Courses
                new Course 
                { 
                    Title = "Generative AI & Prompt Engineering Masterclass", 
                    Description = "Learn how to use Generative AI models and master prompt engineering techniques to build AI-powered applications. Tags: AI, Generative AI, Prompt Engineering, LLM, ChatGPT",
                    Level = "Beginner to Intermediate",
                    Language = "English",
                    Duration = 600, // 10 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = aiCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=600&q=80"
                },
                new Course 
                { 
                    Title = "Machine Learning with Python (Beginner to Advanced)", 
                    Description = "Understand core machine learning algorithms, model training, evaluation, and implementation using Python libraries. Tags: Machine Learning, Python, Data Science, AI, Scikit-Learn",
                    Level = "Beginner to Advanced",
                    Language = "English",
                    Duration = 1200, // 20 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = mlCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1555949963-ff9fe0c870eb?w=600&q=80"
                },
                new Course 
                { 
                    Title = "Deep Learning & Neural Networks with TensorFlow", 
                    Description = "Build and train neural networks using TensorFlow and learn concepts like CNNs, RNNs, and model optimization. Tags: Deep Learning, TensorFlow, Neural Networks, AI, Computer Vision",
                    Level = "Intermediate",
                    Language = "English",
                    Duration = 900, // 15 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = dlCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1620712943543-bcc4688e7485?w=600&q=80"
                },
                new Course 
                { 
                    Title = "Natural Language Processing with Transformers & LLMs", 
                    Description = "Learn text processing, transformer models, and how modern large language models work. Tags: NLP, Hugging Face, Transformers, LLMs, Text Processing",
                    Level = "Intermediate",
                    Language = "English",
                    Duration = 840, // 14 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = nlpCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=600&q=80"
                },
                new Course 
                { 
                    Title = "AI Chatbot Development using LangChain & OpenAI", 
                    Description = "Build intelligent AI chatbots using LangChain frameworks and large language models. Tags: AI Chatbots, LangChain, OpenAI, GPT-4, App Development",
                    Level = "Intermediate",
                    Language = "English",
                    Duration = 720, // 12 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = aiDevCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1531746790731-6c087fecd65a?w=600&q=80"
                },
                
                // Java & OOP Courses
                new Course 
                { 
                    Title = "Complete Java Programming with OOP Concepts", 
                    Description = "Learn Java programming from basics to advanced concepts including object-oriented programming principles. Tags: Java, Programming, OOP, Software Development, Backend",
                    Level = "Beginner to Advanced",
                    Language = "English",
                    Duration = 1500, // 25 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = programmingCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=600&q=80"
                },
                new Course 
                { 
                    Title = "Object-Oriented Programming in Java (Core to Advanced)", 
                    Description = "Master OOP principles such as encapsulation, inheritance, polymorphism, and abstraction using Java. Tags: Java, OOP, Design Patterns, Coding, Software Architecture",
                    Level = "Intermediate",
                    Language = "English",
                    Duration = 600, // 10 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = javaCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1605379399642-870262d3d051?w=600&q=80"
                },
                new Course 
                { 
                    Title = "Java Backend Development with OOP & Spring Boot", 
                    Description = "Learn how to build scalable backend APIs using Java, Spring Boot, and object-oriented design. Tags: Java, Spring Boot, API, Backend, Web Development",
                    Level = "Advanced",
                    Language = "English",
                    Duration = 1800, // 30 hours
                    Price = 0m,
                    IsFree = true,
                    IsPublished = true,
                    CategoryId = backendCategory.Id,
                    InstructorId = instructor.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=600&q=80"
                }
            };

            context.Courses.AddRange(newCourses);
            await context.SaveChangesAsync();

            // --- Generate Modules (Sections) and Lessons ---
            
            var allSections = new List<Section>();
            var allLessons = new List<Lesson>();
            var random = new Random();

            foreach (var course in newCourses)
            {
                // Create 5 modules per course
                for (int m = 1; m <= 5; m++)
                {
                    var section = new Section
                    {
                        CourseId = course.Id,
                        Title = $"Module {m}: {GetModuleName(course.Title, m)}",
                        Order = m
                    };
                    context.Sections.Add(section);
                    await context.SaveChangesAsync(); // Save to get the ID

                    // Create 4 lessons per module
                    for (int l = 1; l <= 4; l++)
                    {
                        var lesson = new Lesson
                        {
                            CourseId = course.Id,
                            SectionId = section.Id,
                            OrderIndex = ((m - 1) * 4) + l,
                            Title = $"Lesson {l}: {GetLessonName(course.Title, m, l)}",
                            Description = $"This lesson covers the essential concepts of {GetLessonName(course.Title, m, l)}.",
                            VideoUrl = "https://www.w3schools.com/html/mov_bbb.mp4", // Placeholder video
                            Duration = random.Next(5, 25), // 5-25 minutes
                            IsPreview = (m == 1 && l == 1) // First lesson is preview
                        };
                        allLessons.Add(lesson);
                    }
                }
            }

            context.Lessons.AddRange(allLessons);
            await context.SaveChangesAsync();
        }

        private static string GetModuleName(string courseTitle, int moduleIndex)
        {
            if (courseTitle.Contains("Generative AI"))
            {
                string[] modules = { "Introduction to Generative AI", "Understanding Large Language Models", "Prompt Engineering Fundamentals", "Advanced Prompting Techniques", "Building AI Applications" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Machine Learning"))
            {
                string[] modules = { "Python Data Science Stack", "Supervised Learning", "Unsupervised Learning", "Model Evaluation & Tuning", "Deployment & Best Practices" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Deep Learning"))
            {
                string[] modules = { "Neural Network Foundations", "TensorFlow Basics", "Convolutional Neural Networks (CNNs)", "Recurrent Neural Networks (RNNs)", "Advanced Architectures" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Natural Language"))
            {
                string[] modules = { "NLP Fundamentals", "Word Embeddings", "Sequence Models", "Transformers Architecture", "Hugging Face Ecosystem" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("LangChain"))
            {
                string[] modules = { "Introduction to LangChain", "Working with Models & Prompts", "Memory & Chains", "Agents & Tools", "Building a Complete Chatbot App" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Complete Java Programming"))
            {
                string[] modules = { "Java Basics & Setup", "Control Flow & Loops", "Functions & Arrays", "Collections Framework", "File I/O & Exceptions" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Object-Oriented Programming in Java"))
            {
                string[] modules = { "Classes & Objects", "Encapsulation & Access Modifiers", "Inheritance", "Polymorphism", "Abstraction & Interfaces" };
                return modules[moduleIndex - 1];
            }
            if (courseTitle.Contains("Java Backend"))
            {
                string[] modules = { "Introduction to Spring Boot", "RESTful APIs", "Data Access with Spring Data JPA", "Security with Spring Security", "Testing & Deployment" };
                return modules[moduleIndex - 1];
            }
            
            return $"Core Concepts Part {moduleIndex}";
        }

        private static string GetLessonName(string courseTitle, int moduleIndex, int lessonIndex)
        {
            return $"Deep Dive Topic {lessonIndex}";
        }
    }
}
