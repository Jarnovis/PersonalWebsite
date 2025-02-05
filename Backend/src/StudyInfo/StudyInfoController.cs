using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using WebApi.Database;

namespace WebApi.StudyInfo;

[Route("api/[controller]")]
public class StudyInfoController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    public StudyInfoController(DatabaseContext databaseContext)
    {
        _dbContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    [HttpGet("GetDegreeInfo")]
    public async Task<IActionResult> GetDegreeInfo(string degreeName)
    {

        Degree degree = DynamicDatabaseTool.SelectExistingItem<Degree>("Name", degreeName, _dbContext);

        if (degree != null)
        {
            var subjects = DynamicDatabaseTool.SelectExistingRow<Subject>("Degree", degree, _dbContext);
            return StatusCode(200, new { degree, subjects });
        }
        
        return StatusCode(404, new { message = "The selected degree was not found." });
    }

    [HttpGet("GetSubjectInfo")]
    public async Task<IActionResult> GetSubjectInfo(string subjectName)
    {
        SelectFromDatabase select = new SelectFromDatabase(_dbContext);

        var info = select.GetRowFromTable<Subject>("CourseName", subjectName);

        if (info.Count > 0)
        {
            return StatusCode(200, new { info });
        }
        
        return StatusCode(404, new { message = "The selected degree was not found." });
    }
}