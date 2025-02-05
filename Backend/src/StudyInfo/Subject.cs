using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenQA.Selenium.DevTools.V130.Page;

namespace WebApi.StudyInfo;

public class Subject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    //[Required]
    //public string? Info { get; set; }
    //public double GradeNumber { get; set; }
    //public char GradeChar { get; set; }
    [Required]
    public string? Date { get; set; }
    [Required]
    public int Points { get; set; }
    [Required]
    public string? Semester { get; set; }
    [Required]
    public string? CourseName { get; set; }
    [Required]
    public Degree? Degree { get; set; }
}