using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace WebApi.StudyInfo;

public class Degree
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public int CurrentPoints { get; set; }
    [Required]
    public int TotalPoints { get; set; }
}