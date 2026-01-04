using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funifest.Domain.Models;

/// <summary>
/// Rola skoczka używana głównie do logiki planowania wylotu po stronie frontendu.
/// </summary>
public enum SkydiverRole
{
    /// <summary>Student AFF Entry</summary>
    StudentAffEntry = 0,

    /// <summary>Student AFF Advanced</summary>
    StudentAffAdvanced = 1,

    /// <summary>Student (po AFF / ogólnie uczeń)</summary>
    Student = 2,

    /// <summary>FunJumper</summary>
    FunJumper = 3,

    /// <summary>Instructor</summary>
    Instructor = 4,

    /// <summary>Examiner</summary>
    Examiner = 5
}
