namespace WebApi.Database;

public class SelectFromDatabase : IDisposable
{
    private readonly DatabaseContext _databaseContext;
    private bool _disposed = false;

    public SelectFromDatabase(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    public T GetRowFromTable<T>(string columnName, string columnValue) where T : class
    {
        T rowData = DynamicDatabaseTool.SelectExistingRow<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    public IList<T> GetRowsFromTable<T>(string columnName, string columnValue) where T : class
    {
        IList<T> rowData = DynamicDatabaseTool.SelectExistingRows<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                (_databaseContext as IDisposable)?.Dispose();
            }

            _disposed = true;
        }
    }

    ~SelectFromDatabase()
    {
        Dispose(false);
    }
}