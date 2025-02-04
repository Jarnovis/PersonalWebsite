using System.Text.RegularExpressions;
using HtmlAgilityPack;
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

    private bool IsFirstYear(string grade)
    {
        char[] firstYear = new char[] {'s', 'e', 'm', 'e', 's', 't', 'e', 'r', ' ', '_'};
        int begin = 0;

        for (int place = 1; place < grade.Length; place++)
        {
            for (int loop = 0; loop < (place - begin); loop++)
            {
                if (loop == 0 && (grade[begin] == 's' || grade[begin] == 'S'))
                { /* pass */ }
                else if (loop == firstYear.Length - 1 && (grade[begin + loop] == '1' || grade[begin + loop] == '2'))
                {
                    return true;
                }
                else if (grade[begin + loop] != firstYear[loop])
                {
                    if (loop == 0)
                    {
                        begin ++;
                    }
                    else
                    {
                        begin = begin + loop;
                    }
                    
                    place = begin + 1;
                    break;
                }
            }
        }
        return false;
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
        HashSet<string> degrees = await GetSelectedNodesHashSetAsync(document, "//label[@class='font-size-xl lh-20']");
        List<string> courses = await GetSelectedNodesListAsync(document, "//ion-row[@class='sc-ion-label-md md']");
        List<string> subjects = await GetSelectedNodesListAsync(document, "//span[@class='font-li-body font-size- osi-black-87 sc-ion-label-md']");



        if (courses.Count == 0 || degrees.Count == 0)
        {
            await LoadProgressionPage();
        }
        else
        {
            IList<IList<string>> degreesList = SetDegrees(degrees);
            IList<string> coursesNames = GrabCourseName(courses);

            int courseInteger = 0;
            for (int i = 0; i < subjects.Count; i += 3)
            {
                int range = Math.Min(3, subjects.Count - i);
                IList<string> data = subjects.GetRange(i, range);

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