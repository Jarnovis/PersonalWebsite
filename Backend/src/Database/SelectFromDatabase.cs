namespace WebApi.Database;

public class SelectFromDatabase
{
    private readonly DatabaseContext _databaseContext;

    public SelectFromDatabase(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    public IList<T> GetRowFromTable<T>(string columnName, string columnValue) where T : class
    {
        IList<T> rowData = DynamicDatabaseTool.SelectExistingRow<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }
}