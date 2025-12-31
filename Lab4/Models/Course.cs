using System;
using System.Collections.Generic;

namespace Lab4.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
