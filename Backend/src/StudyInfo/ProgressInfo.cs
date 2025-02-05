using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;

namespace WebApi.StudyInfo;

public class ProgressInfo
{    
    private readonly DatabaseContext _dbContext;

    public ProgressInfo(DatabaseContext databaseContext) {
        _dbContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    public ProgressInfo(IList<string> degreeInfo, DatabaseContext databaseContext)
    {
        _dbContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        IList<Subject> subjects = new List<Subject>();

        Degree degree = RegistrateDegree(degreeInfo[0]);
        RegistrateSubjects(degreeInfo, degree);
    }

    public Degree RegistrateDegree(string degreeName)
    {
        Degree degree = new Degree()
        {
            Name = degreeName
        };

        degree = DynamicDatabaseTool.AddOrUpdate(degree, "Name", degree.Name, _dbContext);

        return degree;
    }

    public IList<Subject> RegistrateSubjects(IList<string> subjectsList, Degree degree)
    {
        IList<Subject> subjects = new List<Subject>();

        for (int subject = 1; subject < subjectsList.Count; subject += 4)
        {
            Subject subjectInfo = new Subject()
            {
                Points = Convert.ToInt32(subjectsList[subject][0].ToString()),
                Date = subjectsList[subject + 2],
                Semester = subjectsList[subject + 1],
                CourseName = subjectsList[subject + 3],
                Degree = degree
            };

            subjects.Add(subjectInfo);
        }

        int totalPoints = 0;

        foreach (Subject subject in subjects)
        {
            DynamicDatabaseTool.AddOrUpdate(subject, "CourseName", subject.CourseName, _dbContext);
            totalPoints += subject.Points;
        }

        CalculateNewPoints(degree, totalPoints);

        return subjects;
    }

    private void CalculateNewPoints(Degree degree, int totalPoints)
    {
        if (degree.CurrentPoints < totalPoints)
        {
            degree.CurrentPoints = totalPoints;
            DynamicDatabaseTool.AddOrUpdate(degree, "Name", degree.Name, _dbContext);
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
}