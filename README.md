# Learnify

A complete ASP.NET Core MVC Web Application for E-Learning designed for Instructors to upload courses and students to learn and take quizzes.

## Features Included
1. **User Roles:** Admin, Instructor, and Student.
2. **Dashboard Views:** Custom dashboards for each role.
3. **Course Management:** Instructors can create courses, structure lessons, upload video files, and attach PDF documents.
4. **Quiz Engine:** Instructors can build multi-choice quizzes for lessons. Students can take them, and they are auto-graded.
5. **UI & Theme:** Custom Bootstrap 5 layout with responsive cards and clean typography.
6. **Data Layer:** Entity Framework Core with SQL Server Express LocalDB.
7. **File Storage:** Local server storage mapping `wwwroot/uploads/` directory for media handling.

---

## 🚀 How to Run in Visual Studio

Follow these steps carefully to run the platform on your local machine:

### 1. Open the Project
1. Open **Visual Studio 2022**.
2. Click on **Open a project or solution**.
3. Navigate to `C:\Users\hraj5\OneDrive\Desktop\ELearningPlatform` and select `ELearningPlatform.csproj`.

### 2. Update the Database (Run Migrations)
Because this project uses Entity Framework Core for its database, you **must run the migrations** before starting the app to create the tables in your local SQL Server Express.

1. Go to the top menu in Visual Studio: **Tools** > **NuGet Package Manager** > **Package Manager Console**.
2. Wait for the console to initialize.
3. In the console, type the following command to create the initial migration:
   ```powershell
   Add-Migration InitialCreate
   ```
   > **Note:** If you get a build error, ensure all packages are restored first by right-clicking the Solution in Solution Explorer and selecting "Restore NuGet Packages".
4. After the migration file is created successfully, execute this command to create the database:
   ```powershell
   Update-Database
   ```

### 3. Run the Application
1. Press `F5` or `Ctrl + F5` to run the project.
2. The web browser will open to the home page.

### 4. Admin User Details
The system will automatically seed default roles and an initial Admin account the first time you run it.

* **Email:** `admin@elearning.com`
* **Password:** `Admin123!`

**To fully test the platform, I recommend registering two additional accounts:**
1. One account as an **Instructor** (to create a course and add videos/quizzes).
2. One account as a **Student** (to browse the catalog, enroll, watch videos, and take quizzes).

### 5. Media Upload Limits
Currently, the `[RequestSizeLimit(100_000_000)]` attribute allows up to 100MB per video upload. If you encounter issues uploading larger videos while running in IIS Express, you may need to increase the `maxAllowedContentLength` in `web.config` or within the project properties.
