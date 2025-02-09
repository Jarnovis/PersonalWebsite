using WebApi.StudyInfo;

namespace WebApi.Database;

public class SelectFromDatabase : IDisposable
{
    private readonly DatabaseContext _databaseContext;
    private bool _disposed = false;

    public SelectFromDatabase(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    /// <summary>
    /// Selects the first detected row from the database where the given columnvalue (string) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>T</returns>
    public T GetRowFromTable<T>(string columnName, string columnValue) where T : class
    {
        T rowData = DynamicDatabaseTool.SelectExistingRow<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    /// <summary>
    /// Selects the first detected row from the database where the given columnvalue (int) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>T</returns>
    public T GetRowFromTable<T>(string columnName, int columnValue) where T : class
    {
        T rowData = DynamicDatabaseTool.SelectExistingRow<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    /// <summary>
    /// Selects the first detected row from the database where the given columnvalue (class) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>T</returns>
    public T GetRowFromTable<T>(string columnName, T columnValue) where T : class
    {
        T rowData = DynamicDatabaseTool.SelectExistingRow<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    /// <summary>
    /// Selects each detected row from the database where the given columnvalue (string) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>IList<typeparamref name="T"/>></returns>
    public IList<T> GetRowsFromTable<T>(string columnName, string columnValue) where T : class
    {
        IList<T> rowData = DynamicDatabaseTool.SelectExistingRows<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    /// <summary>
    /// Selects each detected row from the database where the given columnvalue (int) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>IList<typeparamref name="T"/>></returns>
    public IList<T> GetRowsFromTable<T>(string columnName, int columnValue) where T : class
    {
        IList<T> rowData = DynamicDatabaseTool.SelectExistingRows<T>(columnName, columnValue, _databaseContext);

        return rowData;
    }

    /// <summary>
    /// Selects each detected row from the database where the given columnvalue (class) matches the columnvalue in the database from the given column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <param name="columnValue"></param>
    /// <returns>IList<typeparamref name="T"/>></returns>
    public IList<T> GetRowsFromTable<T>(string columnName, T columnValue) where T : class
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

    internal object GetRowsFromTable<T>(string v, Degree degree)
    {
        throw new NotImplementedException();
    }

    ~SelectFromDatabase()
    {
        Dispose(false);
    }
}