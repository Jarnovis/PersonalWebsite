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

    [HttpGet("ReadFromOsiris")]
    public async Task<IActionResult> ReadFromOsiris()
    {
        CollectStudyInfoProgression progression = new CollectStudyInfoProgression(_dbContext);

        progression.LoadProgressionPage();

        return StatusCode(200, new { message = "Ok" } );
    }
}