using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Mysqlx;
using WebApi.Database;

namespace WebApi.StudyInfo;

public class CollectStudyInfoProgression
{
    private readonly DatabaseContext _dbContext;
    public CollectStudyInfoProgression(DatabaseContext databaseContext)
    {
        _dbContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    public async Task LoadProgressionPage()
    {
        HtmlDocument document = new HtmlDocument();
        using (OpenWebsite website = new OpenWebsite())
        {
            try
            {
                bool succesfullLogin = website.SignInMicrosoftAuthenticator("https://hhs.osiris-student.nl");

                if (succesfullLogin)
                {
                    string html = website.GetInfoFromWebPage("https://hhs.osiris-student.nl/voortgang");
                    document.LoadHtml(html);
                    await CollectProgressInfo(document);
                }
                else
                {
                    throw new Exception ("No succesfull login");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await LoadProgressionPage();
            }
        }
    }

    private async Task<HashSet<string>> GetSelectedNodesHashSetAsync(HtmlDocument document, string xpath)
    {
        HashSet<string> data = new HashSet<string>();

        var foundedNodes = document.DocumentNode.SelectNodes(xpath);

        if (foundedNodes != null)
        {
            foreach (var node in foundedNodes)
            {
                string content = node.InnerText.Trim();

                data.Add(content);
            }
        }

        return data;
    }

    private async Task<List<string>> GetSelectedNodesListAsync(HtmlDocument document, string xpath)
    {
        List<string> data = new List<string>();

        var foundedNodes = document.DocumentNode.SelectNodes(xpath);

        if (foundedNodes != null)
        {
            foreach (var node in foundedNodes)
            {
                string content = node.InnerText.Trim();

                data.Add(content);
            }
        }

        return data;
    }

    private static bool IsFirstYear(string semester)
    {
        string lower = semester.ToLower();

        return lower.Contains("semester 1") || lower.Contains("semester 2");
    }

    private IList<IList<string>> SetDegrees(HashSet<string> degrees)
    {
        IList<IList<string>> degreesWithGrades = new List<IList<string>>();
        
        foreach (string degree in degrees)
        {
            IList<string> degreeWithGrades = [degree];
            degreesWithGrades.Add(degreeWithGrades);
        }

        return degreesWithGrades;
    }

    private string SplitCourseName(string course, bool gradeIsNumber)
    {
        int startSplit = (gradeIsNumber ? 2 : 0) + 7;
        int endSplit = 0;

        for (int i = course.Length - 1; i >= 0; i--)
        {
            if (course[i] == '(')
            {
                endSplit = i;
                break;
            }
        }

        return course.Substring(startSplit, endSplit-startSplit);
    }

    private IList<string> GrabCourseName(IList<string> grades)
    {
        string pattern = @"^(V|O|[0-9])$";
        IList<string> courses = new List<string>();

        foreach (string grade in grades)
        {
            if (grade.Length > 10 && Regex.IsMatch(grade[0].ToString(), pattern))
            {
                if (Regex.IsMatch(grade[0].ToString(), @"^([0-9])$"))
                {
                    courses.Add(SplitCourseName(grade, true));
                }
                else
                {
                    courses.Add(SplitCourseName(grade, false));
                }
            }
        }

        return courses;
    }

    private async Task CollectProgressInfo(HtmlDocument document)
    {
        var degrees = GetSelectedNodesHashSetAsync(document, "//label[@class='font-size-xl lh-20']");
        var courses =  GetSelectedNodesListAsync(document, "//ion-row[@class='sc-ion-label-md md']");
        var subjects =  GetSelectedNodesListAsync(document, "//span[@class='font-li-body font-size- osi-black-87 sc-ion-label-md']");

        Task.WaitAll(degrees, courses, subjects);

        foreach (string s in degrees.Result)
        {
            Console.WriteLine(s);
        }

        foreach (string s in courses.Result)
        {
            Console.WriteLine(s);
        }

        foreach (string s in subjects.Result)
        {
            Console.WriteLine(s);
        }
        
        if (courses.Result.Count == 0 || degrees.Result.Count == 0)
        {
            await LoadProgressionPage();
        }
        else
        {
            IList<IList<string>> degreesList = SetDegrees(degrees.Result);
            IList<string> coursesNames = GrabCourseName(courses.Result);

            int courseInteger = 0;
            for (int i = 0; i < subjects.Result.Count; i += 3)
            {
                int range = Math.Min(3, subjects.Result.Count - i);
                IList<string> data = subjects.Result.GetRange(i, range);

                if (IsFirstYear(data[1]))
                {
                    foreach (string str in data)
                    {
                        degreesList[1].Add(str);
                    }

                    degreesList[1].Add(coursesNames[courseInteger]);
                }
                else
                {
                    foreach (string str in data)
                    {   
                        degreesList[0].Add(str);
                    }
                    degreesList[0].Add(coursesNames[courseInteger]);
                }

                courseInteger++;

            }

            
            foreach (IList<string> degreeWithSubject in degreesList)
            {
                new ProgressInfo(degreeWithSubject, _dbContext);
            }
        }
    }

    
}