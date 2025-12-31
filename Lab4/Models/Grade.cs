using System;
using System.Collections.Generic;

namespace Lab4.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public string GradeValue { get; set; } = null!;

    public DateOnly DateGiven { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int TeacherId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Staff Teacher { get; set; } = null!;
}
