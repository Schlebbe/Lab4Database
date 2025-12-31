using Lab4.Data;
using Lab4.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Threading.Tasks;

namespace Lab4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {
                        "List count of staff by role", "View all students", "View all active courses", "Grade a student", "Exit"
                    }));

            using (var context = new SchoolDbContext())
            {
                switch (option)
                {
                    case "List count of staff by role":
                        PrintEmployeesByRole(context);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        Main(args);
                        break;
                    case "View all students":
                        PrintAllStudents(context);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        Main(args);
                        break;
                    case "View all active courses":
                        PrintAllActiveCourses(context);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        Main(args);
                        break;
                    case "Grade a student":
                        GradeStudent(context);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        Main(args);
                        break;
                    case "Exit":
                        return;
                }
            }
        }

        private static async Task GradeStudent(SchoolDbContext context)
        {
            var selectedTeacher = AnsiConsole.Prompt(
                new SelectionPrompt<Staff>()
                    .Title("Select a [green]Teacher[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more teachers)[/]")
                    .UseConverter(t => $"{t.FirstName} {t.LastName}")
                    .AddChoices(context.Staff.Where(s => s.Role.ToLower() == "teacher").ToList()));

            var selectedStudent = AnsiConsole.Prompt(
                new SelectionPrompt<Student>()
                    .Title("Select a [green]student[/] to [yellow]grade[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more students)[/]")
                    .UseConverter(s => $"{s.FirstName} {s.LastName}")
                    .AddChoices(context.Students.Include(s => s.Grades).ThenInclude(g => g.Course).ToList()));

            var selectedCourse = AnsiConsole.Prompt(
                new SelectionPrompt<Course>()
                    .Title("Select a [green]course[/] to assign a [yellow]grade[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more courses)[/]")
                    .UseConverter(c => c.CourseName)
                    .AddChoices(context.Courses.ToList()));

            var gradeValue = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a [green]grade[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more grade values)[/]")
                    .AddChoices(new[] { "A", "B", "C", "D", "E", "F" }));

            var grade = new Grade
            {
                StudentId = selectedStudent.StudentId,
                CourseId = selectedCourse.CourseId,
                GradeValue = gradeValue,
                DateGiven = DateOnly.FromDateTime(DateTime.Now),
                TeacherId = selectedTeacher.StaffId
            };

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                context.Grades.Add(grade);
                context.SaveChanges();
                await transaction.CommitAsync();
                AnsiConsole.MarkupLine("[green]Grade assigned successfully![/]");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                AnsiConsole.MarkupLine("[red]Error assigning grade. Transaction rolled back.[/]");
            }
        }

        private static void PrintAllActiveCourses(SchoolDbContext context)
        {
            var courses = context.Courses.Where(c => c.IsActive).ToList();
            var table = new Table();
            table.AddColumns("[green]Course ID[/]", "[green]Course Name[/]", "[green]Course Code[/]");
            foreach (var course in courses)
            {
                table.AddRow(course.CourseId.ToString(), course.CourseName, course.CourseCode);
            }
            AnsiConsole.Write(table);
        }

        private static void PrintAllStudents(SchoolDbContext context)
        {
            var students = context.Students.Include(s => s.Class).Include(s => s.Grades).ThenInclude(g => g.Course).ToList();
            var table = new Table();
            table.AddColumns("[green]Student ID[/]", "[green]First Name[/]", "[green]Last Name[/]", "[green]Personal Number[/]", "[green]Class[/]", "[green]Course (Grade)[/]");
            foreach (var student in students)
            {
                table.AddRow(student.StudentId.ToString(), student.FirstName, student.LastName, student.PersonalNumber, student.Class.ClassName);
                foreach (var grade in student.Grades)
                {
                    table.AddRow("", "", "", "", "", $"[yellow]{grade.Course.CourseName.ToString()} ({grade.GradeValue})[/]");
                }
            }
            AnsiConsole.Write(table);
        }

        private static void PrintEmployeesByRole(SchoolDbContext context)
        {
            var staffCountByRole = context.Staff
                .GroupBy(s => s.Role)
                .Select(g => new
                {
                    Role = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var table = new Table();
            table.AddColumns("[green]Role[/]", "[green]Count[/]");
            foreach (var item in staffCountByRole)
            {
                table.AddRow(item.Role, item.Count.ToString());
            }
            AnsiConsole.Write(table);
        }
    }
}
