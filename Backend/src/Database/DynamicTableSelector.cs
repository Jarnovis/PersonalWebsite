using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using WebApi.StudyInfo;

namespace WebApi.Database;

public class DynamicDatabaseTool
{
    private static Expression<Func<T, bool>> CreatePredicate<T>(string columnName, object columnValue)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, columnName);
        if (columnValue != null && columnValue.GetType() != property.Type)
        {
            try
            {
                columnValue = Convert.ChangeType(columnValue, property.Type);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Cannot convert {columnValue.GetType()} to {property.Type}");
            }
        }
        var constant = Expression.Constant(columnValue, property.Type);
        var equality = Expression.Equal(property, constant);
        var predicate = Expression.Lambda<Func<T, bool>>(equality, parameter);

        return predicate;
    }
    public static T SelectExistingRow<T>(string columnName, object columnValue, DatabaseContext dbContext) where T : class
    {
        var predicate = CreatePredicate<T>(columnName, columnValue);
        var existingItem = dbContext.Set<T>().FirstOrDefault(predicate);

        return existingItem;
    }

    public static List<T> SelectExistingRows<T>(string identityColumn, object identityValue, DatabaseContext dbContext) where T : class
    {
        var predicate = CreatePredicate<T>(identityColumn, identityValue);
        var existingItems = dbContext.Set<T>().Where(predicate).ToList();

        return existingItems;
    }

    public static List<T> SelectWholeTable<T>(T table, DatabaseContext dbContext) where T : class
    {
        return dbContext.Set<T>().ToList();
    }

    public static T AddOrUpdate<T>(T data, string columnName, object columnValue, DatabaseContext dbContext) where T : class
    {
        var existingItem = SelectExistingRow<T>(columnName, columnValue, dbContext);

        if (existingItem == null)
        {
            dbContext.Set<T>().Add(data);
            dbContext.SaveChanges();
            
            return data;
        }
        else
        {
            if (data.GetType() == new Degree().GetType())
            {
                Console.WriteLine("Degree");
            }
            UpdateValue(data, existingItem, dbContext);
            return existingItem;
        }
    }

    /// <summary>
    /// Checks if the incomming values are different from the values in the database.
    /// When the incomming values are different, the database will be updated with the rigth values.
    /// The data will also be automaticly be submitted towards the database with <see cref="DynamicDatabaseTool.UpdateDatabase{T}(Dictionary{string, object}, T, DatabaseContext)"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="detected"></param>
    /// <param name="database"></param>
    /// <param name="dbContext"></param>
    public static void UpdateValue<T>(T detected, T database, DatabaseContext dbContext) where T : class
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
            }
        }

        if (changedValues.Count > 0)
        {
            UpdateDatabase(changedValues, database, dbContext);
        }
    }

    public static void UpdateDatabase<T>(Dictionary<string, object> updates, T row, DatabaseContext dbContext) where T : class
    {
        foreach (var entry in updates)
        {
            var prop = typeof(T).GetProperty(entry.Key);

            prop?.SetValue(row, entry.Value);
        }

        dbContext.Attach(row);
        dbContext.Entry(row).State = EntityState.Modified;
        dbContext.SaveChanges();
    }
}