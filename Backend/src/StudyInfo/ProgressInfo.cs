using System.Text;
using WebApi.Database;
using WebApi.Enviroment;
using WebApi.Services.EmailServices;

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
        Degree degreeForPoints = DynamicDatabaseTool.SelectExistingRow<Degree>("Name", degreeName, _dbContext);
        Degree degree = new Degree()
        {
            Name = degreeName,
            CurrentPoints = degreeForPoints.CurrentPoints,
            TotalPoints = degreeForPoints.TotalPoints
        };

        degree = DynamicDatabaseTool.AddOrUpdate(degree, "Name", degree.Name, _dbContext);

        return degree;
    }

    public async Task<IList<Subject>> RegistrateSubjects(IList<string> subjectsList, Degree degree)
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
        Dictionary<string, int> subjectsWithPoints = new Dictionary<string, int>();

        foreach (Subject subject in subjects)
        {
            Console.WriteLine(subject.CourseName);
            Subject temp = DynamicDatabaseTool.SelectExistingRow<Subject>("CourseName", subject.CourseName, _dbContext);

            if (temp != null)
            {
                if (subject.Points > temp.Points)
                {
                    subjectsWithPoints[subject.CourseName] = subject.Points;
                }
            }
            else
            {
                subjectsWithPoints[subject.CourseName] = subject.Points;
            }

            DynamicDatabaseTool.AddOrUpdate(subject, "CourseName", subject.CourseName, _dbContext);

            totalPoints += subject.Points;
        }

        await CalculateNewPoints(degree, totalPoints, subjectsWithPoints);

        return subjects;
    }

    private async Task CalculateNewPoints(Degree degree, int totalPoints, Dictionary<string, int> subjectsWithPoints)
    {
        if (degree.CurrentPoints < totalPoints && degree.CurrentPoints != 0)
        {
            using (EnvConfig env = new EnvConfig())
            using (EmailService emailService = new EmailService(new EnvConfig()))
            {
                degree.CurrentPoints = totalPoints;
                Console.WriteLine($"Points: {degree.CurrentPoints}, {totalPoints}");
                if (degree.CurrentPoints == totalPoints)
                {
                    await emailService.Send(env.Get("PERSONAL_EMAIL"), $"Updated Study Points {degree.Name}", CreatePointsUpdateEmail(degree, totalPoints, subjectsWithPoints));
                    DynamicDatabaseTool.AddOrUpdate(degree, "Name", degree.Name, _dbContext);
                }
            }
        }
    }

    private string CreatePointsUpdateEmail(Degree degree, int totalPoints, Dictionary<string, int> subjectWithPoints)
    {
        StringBuilder listed = new();

        foreach (var subject in subjectWithPoints)
        {
            listed.Append($"<li>{subject.Key}: {subject.Value} points</li>");
        }
        
        listed.ToString();
        decimal percentage = (decimal)totalPoints / degree.TotalPoints * 100;

        string body = @$"<h1>Update for {degree.Name}</h1>
        <h2>Points Summery</h2>
        <ul>
            <li>Old points: {degree.CurrentPoints}</li>
            <li>New points: {totalPoints}</li>
            <li>Points to get: {degree.TotalPoints - totalPoints}</li>
            <li>Overview: {totalPoints}/{degree.TotalPoints}</li>
        </ul>
        <p>You have currently {percentage}% of your {degree.Name}</p>
        
        <h2>Subject(s) that gave you the new points</h2>
        <ul>{listed}</ul>
        ";

        return body;
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