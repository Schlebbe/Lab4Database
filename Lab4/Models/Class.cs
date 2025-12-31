using System;
using System.Collections.Generic;

namespace Lab4.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int TeacherId { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Staff Teacher { get; set; } = null!;
}
