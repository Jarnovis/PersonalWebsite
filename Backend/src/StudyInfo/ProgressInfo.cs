using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApi.Database;

namespace WebApi.StudyInfo;

public class ProgressInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public Degree Degree { get; set; }
    [Required]
    public IList<Subject> Subjects = new List<Subject>();
    [Required]
    public DateOnly Date { get; set; }

    private readonly DatabaseContext _dbContext;

    public ProgressInfo(DatabaseContext databaseContext) {
        _dbContext = databaseContext;
    }

    public ProgressInfo(IList<string> degreeInfo, DatabaseContext databaseContext)
    {
        _dbContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));

        Date = new DateOnly();

        Degree = new Degree()
        {
            Name = degreeInfo[0],
            CurrentPoints = 1,
            TotalPoints = 2
        };

        AddDegreeIfNotExistsInDatabase(Degree);
        Console.WriteLine(degreeInfo.Count);

        for (int subject = 1; subject < degreeInfo.Count; subject += 4)
        {
            Subject subjectInfo = new Subject()
            {
                Points = Convert.ToInt32(degreeInfo[subject][0].ToString()),
                Date = degreeInfo[subject + 2],
                Semester = degreeInfo[subject + 1],
                CourseName = degreeInfo[subject + 3]
            };

            Subjects.Add(subjectInfo);
        }

        foreach (Subject subject in Subjects)
        {
            Console.WriteLine(subject.CourseName);
            AddSubjectIfNotExistsInDatabase(subject);
        }

    }



    // Functies hieronder is voor als je de cijfers meegeeft inplaats van de studiepunten
    /*private double CreateGradeFormat(string grade)
    {
        string numbers = grade.Trim(',');

        try
        {
            return Convert.ToDouble(numbers) / 10;
        }
        catch (OverflowException ex)
        {
            Console.WriteLine(ex.Message);
            return -0.1;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -0.2;
        }
    }

    private void SplitSubjectInfo(string info)
    {
        info = System.Net.WebUtility.HtmlDecode(info);

        string assesmentRating = info.Substring(0, 3).Trim();
        string subjectInfo = info.Substring(3).Trim();
        string pattern = @"^(V|O)$";

        if (Regex.IsMatch(assesmentRating, pattern))
        {
            Subjects.Add(new Subject(){ Info = subjectInfo.ToString(), GradeChar = assesmentRating[0]});
        }
        else
        {
            Subjects.Add(new Subject() {Info = subjectInfo.ToString(), GradeNumber = CreateGradeFormat(assesmentRating)});
        }
    } */

    private void AddDegreeIfNotExistsInDatabase(Degree degree)
    {
        var existingDegree = _dbContext.Degree.FirstOrDefault(d => d.Name == degree.Name);

        if (existingDegree == null)
        {
            _dbContext.Degree.Add(degree);
            _dbContext.SaveChanges();
        }
        else
        {
            UpdateValue(degree, existingDegree);
        }
    }

    private void AddSubjectIfNotExistsInDatabase(Subject subject)
    {
        Subject? existingSubject = _dbContext.Subject.FirstOrDefault(s => s.CourseName == subject.CourseName);

        if (existingSubject == null)
        {
            _dbContext.Subject.Add(subject);
            _dbContext.SaveChanges();
        }
        else
        {
            Console.WriteLine($"Updating existing subject: {subject.CourseName}");
            UpdateValue(subject, existingSubject);
        }
    }

    /// <summary>
    /// Checks if the incomming values are different from the values in the database.
    /// When the incomming values are different, the database will be updated with the rigth values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="detected"></param>
    /// <param name="database"></param>
    private void UpdateValue<T>(T detected, T database) where T : class
    {
        Dictionary<string, object> changedValues = new Dictionary<string, object>();
        
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            var detectedValue = prop.GetValue(detected);
            var databaseValue = prop.GetValue(database);

            if (prop.GetCustomAttribute<KeyAttribute>() != null)
            {
                continue;
            }

            if (detectedValue != databaseValue && detectedValue != null && databaseValue != null)
            {
                changedValues[prop.Name] = detectedValue;

                Console.WriteLine($"{detectedValue}, {databaseValue}, {prop.Name}");
            }
        }

        if (changedValues.Count > 0)
        {
            UpdateDatabase(changedValues, database);
        }
    }

    private void UpdateDatabase<T>(Dictionary<string, object> updates, T row) where T : class
    {
        foreach (var entry in updates)
        {
            var prop = typeof(T).GetProperty(entry.Key);

            prop?.SetValue(row, entry.Value);
        }

        _dbContext.Attach(row);
        _dbContext.SaveChanges();
    }
}