using ServerDeployment.Domains.Utility;
using ServerDeployment.infrastructure.Contracts.IDataAccessor;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Transactions;

namespace ServerDeployment.infrastructure.DataAccessor;

public class DataAccess : IDataAccess
{
    #region "Global declaration"

    private readonly string _dbConnectionString;

    public DataAccess(string conString)
    {
        _dbConnectionString = conString;
    }

    #endregion


    #region "Insert"

    public string Insert<T>(T obj, string tableName) where T : new()
    {
        string result = "0";
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = GetInsertSqlCommand(obj, tableName))
                {
                    cmd.Connection = cn;

                    UpdateParametersWithObjectValues(cmd, obj);
                    object id = cmd.ExecuteScalar();
                    result = id?.ToString() ?? "0";
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }

    public async Task<string> InsertAsync<T>(T obj, string tableName) where T : new()
    {
        string result = "0";
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();

                using (SqlCommand cmd = GetInsertSqlCommand(obj, tableName))
                {
                    cmd.Connection = cn;

                    UpdateParametersWithObjectValues(cmd, obj);
                    object id = await cmd.ExecuteScalarAsync();
                    result = id?.ToString() ?? "0";
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }

    public async Task<int> InsertRangeAsync<T>(ICollection<T> lst, string tableName) where T : new()
    {
        int inserted = 0;

        if (lst.Count == 0)
        {
            return inserted;
        }

        using (SqlConnection cn = new SqlConnection(_dbConnectionString))
        {
            await cn.OpenAsync();

            using (SqlCommand cmd = GetInsertSqlCommand(lst.First(), tableName))
            {
                cmd.Connection = cn;

                foreach (T obj in lst)
                {
                    try
                    {
                        cmd.Parameters.Clear();
                        UpdateParametersWithObjectValues(cmd, obj);

                        await cmd.ExecuteNonQueryAsync();
                        inserted++;
                    }
                    catch (Exception)
                    {
                        // Handle any insertion failure for this row (you can log or take other actions here)
                    }
                }
            }

            await cn.CloseAsync();
        }

        return inserted;
    }

    public async Task<int> ExecuteSqlAsync(string sqlSchema)
    {
        int result = 0;

        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlSchema;
                    result = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }

    private static SqlCommand GetInsertSqlCommand<T>(T obj, string tableName) where T : new()
    {
        SqlCommand command = new SqlCommand();

        Type type = obj.GetType();

        if (AppUtility.HasNoStr(tableName))
        {
            tableName = type.Name;
        }


        PropertyInfo primaryKeyProp = GetPrimaryKeyProperty(type);

        string sql = $"INSERT INTO dbo.{tableName} (";
        string valuesSql = "VALUES (";

        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (prop.Name == primaryKeyProp.Name) continue;

            if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
            {
                string paramName = $"@{prop.Name}";
                SqlParameter parameter = new SqlParameter(paramName, DBNull.Value);
                command.Parameters.Add(parameter);

                sql += $"{prop.Name}, ";
                valuesSql += $"{paramName}, ";
            }
        }

        sql = sql.TrimEnd(' ', ',') + ")";


        //Define OUTPUT primaryKey's inserted value
        if (!string.IsNullOrEmpty(primaryKeyProp.Name))
        {
            sql += " OUTPUT Inserted." + primaryKeyProp.Name;
        }

        valuesSql = valuesSql.TrimEnd(' ', ',') + ")";

        sql += " " + valuesSql;

        command.CommandText = sql;

        return command;
    }

    private static PropertyInfo GetPrimaryKeyProperty(Type type)
    {
        foreach (PropertyInfo prop in type.GetProperties())
        {
            var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
            if (attribute != null)
            {
                return prop;
            }
        }

        return null;
    }

    #endregion

    #region "Update"

    public async Task<int> UpdateAsync<T>(T obj, string tableName) where T : new()
    {
        int result = 0;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();

                using (SqlCommand cmd = GetUpdateSqlCommand(obj, tableName))
                {
                    cmd.Connection = cn;
                    result = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return result;
    }

    public async Task<int> UpdateRangeAsync<T>(ICollection<T> lst, string tableName = "") where T : new()
    {
        int updated = 0;

        if (lst.Count == 0)
        {
            return updated;
        }

        using (SqlConnection cn = new SqlConnection(_dbConnectionString))
        {
            await cn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandType = CommandType.Text;

                foreach (T obj in lst)
                {
                    try
                    {
                        SqlCommand updateCommand = GetUpdateSqlCommand(obj, tableName);
                        updateCommand.Connection = cn;

                        var isExecuted = await updateCommand.ExecuteNonQueryAsync();
                        updated++;
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception (log or take other actions)
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            await cn.CloseAsync();
        }

        return updated;
    }

    private static SqlCommand GetUpdateSqlCommand<T>(T obj, string tableName = null) where T : new()
    {
        SqlCommand command = new SqlCommand();
        ICollection<SqlParameter> parameters = new List<SqlParameter>();
        // Create type of T
        Type type = obj.GetType();
        if (AppUtility.HasNoStr(tableName))
        {
            tableName = type.Name;
        }


        string primaryKey = null;

        type = obj.GetType();

        // Get primary key column and value.
        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (string.IsNullOrEmpty(primaryKey) && !prop.GetMethod.IsVirtual)
            {
                var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
                if (attribute != null)
                {
                    primaryKey = prop.Name;
                    object propValue = prop.GetValue(obj, null);
                    SqlParameter primaryKeyParam = new SqlParameter($"@{primaryKey}", propValue ?? DBNull.Value);
                    parameters.Add(primaryKeyParam);
                    break;
                }
            }
        }

        // Create the UPDATE statement.
        string sql = $"UPDATE {tableName} SET ";

        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (prop.Name != primaryKey && !prop.GetMethod.IsVirtual &&
                !Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
            {
                if (prop.PropertyType == typeof(DateTime))
                {
                    string paramName = $"@{prop.Name}";
                    var propVal = prop.GetValue(obj, null);
                    if (propVal != null)
                    {
                        DateTime dateTime = Convert.ToDateTime(propVal);
                        if (dateTime <= DateTimeConstant.EMPTY_DATE)
                        {
                            propVal = DateTimeConstant.EMPTY_DATE;
                        }
                    }

                    object propValue = propVal != null ? Convert.ToDateTime(propVal) : null;
                    SqlParameter parameter = new SqlParameter(paramName, propValue ?? DBNull.Value);
                    parameters.Add(parameter);

                    sql += $"{prop.Name} = {paramName}, ";
                }
                else
                {
                    string paramName = $"@{prop.Name}";
                    object propValue = prop.GetValue(obj, null);
                    SqlParameter parameter = new SqlParameter(paramName, propValue ?? DBNull.Value);
                    parameters.Add(parameter);

                    sql += $"{prop.Name} = {paramName}, ";
                }
            }
        }

        // Remove the trailing comma and space.
        sql = sql.TrimEnd(' ', ',');

        // Add the WHERE clause with the primary key.
        sql += $" WHERE {primaryKey} = @{primaryKey}";

        // Assign the SQL and parameters to the SqlCommand.
        command.CommandText = sql;
        command.Parameters.AddRange(parameters.ToArray());

        return command;
    }

    private static void UpdateParametersWithObjectValues<T>(SqlCommand cmd, T obj) where T : new()
    {
        PropertyInfo primaryKeyProp = GetPrimaryKeyProperty(obj.GetType());
        foreach (SqlParameter parameter in cmd.Parameters)
        {
            string parameterName = parameter.ParameterName.Substring(1);
            PropertyInfo prop = typeof(T).GetProperty(parameterName);

            if (prop != null)
            {
                if (prop.Name == primaryKeyProp.Name) continue;

                object propValue = prop.GetValue(obj, null);
                parameter.Value = propValue ?? DBNull.Value;
            }
        }
    }

    #endregion

    #region "Delete"

    public async Task<int> DeleteAsync<T>(T obj) where T : new()
    {
        int result = 0;

        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;

                    var sqlAndParams = GetDeleteSql(obj);
                    cmd.CommandText = sqlAndParams.Sql;

                    foreach (var parameter in sqlAndParams.Parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    result = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }

    public async Task<int> DeleteRangeAsync<T>(ICollection<T> lst) where T : new()
    {
        int deleted = 0;

        if (lst.Count == 0)
        {
            return deleted;
        }

        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;

                    var sqlAndParams = ConvertToParameterizedDeleteQuery<T>();
                    cmd.CommandText = sqlAndParams.Sql;

                    foreach (var parameter in sqlAndParams.Parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    foreach (T obj in lst)
                    {
                        try
                        {
                            SetParameterValues(cmd.Parameters, obj);

                            await cmd.ExecuteNonQueryAsync();
                            deleted++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return deleted;
    }

    private (string Sql, SqlParameter[] Parameters) GetDeleteSql<T>(T obj) where T : new()
    {
        string tableName = typeof(T).Name;
        string primaryKey = null;
        PropertyInfo primaryKeyProperty = null;

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
            if (attribute != null)
            {
                primaryKey = prop.Name;
                primaryKeyProperty = prop;
                break;
            }
        }

        if (primaryKey == null)
        {
            throw new InvalidOperationException("No primary key found for the entity.");
        }

        string sql = $"DELETE FROM {tableName} WHERE {primaryKey} = @{primaryKey}";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter($"@{primaryKey}", primaryKeyProperty.GetValue(obj) ?? DBNull.Value)
        };

        return (sql, parameters);
    }

    private (string Sql, SqlParameter[] Parameters) ConvertToParameterizedDeleteQuery<T>() where T : new()
    {
        string tableName = typeof(T).Name;
        string primaryKey = null;
        PropertyInfo primaryKeyProperty = null;

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
            if (attribute != null)
            {
                primaryKey = prop.Name;
                primaryKeyProperty = prop;
                break;
            }
        }

        if (primaryKey == null)
        {
            throw new InvalidOperationException("No primary key found for the entity.");
        }

        string sql = $"DELETE FROM {tableName} WHERE {primaryKey} = @{primaryKey}";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter($"@{primaryKey}", DBNull.Value)
        };

        return (sql, parameters);
    }

    private static void SetParameterValues<T>(SqlParameterCollection parameters, T obj) where T : new()
    {
        foreach (SqlParameter parameter in parameters)
        {
            string paramName = parameter.ParameterName.TrimStart('@');
            PropertyInfo prop = typeof(T).GetProperty(paramName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop != null)
            {
                object value = prop.GetValue(obj) ?? DBNull.Value;
                parameter.Value = value;
            }
        }
    }

    #endregion

    #region "Gets"

    public async Task<T?> GetAsync<T>(string sqlQuery) where T : new()
    {
        T? obj = default;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow r = dt.Rows[0];
                        obj = AppUtility.CreateObjectFromRow<T>(r);
                    }

                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return obj;
    }

    public async Task<T?> GetByIdAsync<T>(long id) where T : new()
    {
        T? obj = default;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = GetSelectSql<T>(id);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow r = dt.Rows[0];
                        obj = AppUtility.CreateObjectFromRow<T>(r);
                    }

                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return obj;
    }

    public T GetById<T>(int id) where T : new()
    {
        T obj = default;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = GetSelectSql<T>(id);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow r = dt.Rows[0];
                        obj = AppUtility.CreateObjectFromRow<T>(r);
                    }

                    cmd.Dispose();
                }

                cn.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return obj;
    }
    public ICollection<T> GetAll<T>(string sqlQuery) where T : new()
    {
        ICollection<T> lst = new List<T>();
        try
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    cmd.Dispose();
                }

                cn.Close();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    lst.Add(AppUtility.CreateObjectFromRow<T>(r));
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return lst;
    }

    public async Task<ICollection<T>> GetAllAsync<T>(string sqlQuery) where T : new()
    {
        ICollection<T> lst = new List<T>();
        try
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    lst.Add(AppUtility.CreateObjectFromRow<T>(r));
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return lst;
    }

    public async Task<int> GetCountAsync<T>(string filter) where T : new()
    {
        T obj = default;
        int numrows = 0;

        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = GetCountSql<T>(filter);
                    cmd.CommandType = CommandType.Text;
                    numrows = (int)cmd.ExecuteScalar();
                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return numrows;
    }

    public async Task<int> GetCountBySqlAsync(string sqlJoins)
    {
        int numrows = 0;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = sqlJoins;
                    cmd.CommandType = CommandType.Text;
                    numrows = (int)cmd.ExecuteScalar();
                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return numrows;
    }

    private static string GetSelectSql<T>(long Id) where T : new()
    {
        Type type = Activator.CreateInstance<T>().GetType();
        string primaryKey = string.Empty;
        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (string.IsNullOrEmpty(primaryKey) && !prop.GetMethod.IsVirtual)
            {
                var attribute = Attribute.GetCustomAttribute(prop, typeof(KeyAttribute)) as KeyAttribute;
                if (attribute != null)
                {
                    primaryKey = prop.Name;
                    break;
                }
            }
        }

        return string.Format("SELECT * FROM {0} WHERE {1} = {2}", type.Name, primaryKey, Id);
    }

    private static string GetCountSql<T>(string filter) where T : new()
    {
        Type type = typeof(T);
        if (string.IsNullOrEmpty(filter))
        {
            filter = " 1=1 ";
        }

        return string.Format("SELECT COUNT(*) FROM {0} WHERE {1} ", type.Name, filter);
    }

    #endregion

    #region Query

    public async Task<bool> ExecuteNonQuery(string sqlQuery)
    {
        bool result = false;
        try
        {
            using (SqlConnection cn = new SqlConnection(_dbConnectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    result = await cmd.ExecuteNonQueryAsync() > 0;
                    await cmd.DisposeAsync();
                }

                await cn.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }

    #endregion


    #region Transaction Scope

    public TransactionScope BeginTransaction()
    {
        using TransactionScope transactionScope = new TransactionScope();
        return transactionScope;
    }

    #endregion

    #region "SQL Injection Validator"

    public static bool IsSafeInput(string input)
    {
        if (input is null)
        {
            return true; // Null input is considered safe
        }

        // Check for common SQL injection patterns in the input string
        string[] sqlInjectionPatterns =
        {
            @"SELECT\s+",
            @"INSERT\s+",
            @"UPDATE\s+",
            @"DELETE\s+",
            @"UNION\s+",
            @"DROP\s+",
            @"ALTER\s+",
            @"EXEC\s+",
            @"TRUNCATE\s+",
            @";|'|""|`|=" // Common characters used in SQL injection
        };

        foreach (string pattern in sqlInjectionPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                return false; // Potential SQL injection detected
            }
        }

        return true; // Input is safe
    }

    public static bool IsSafeInput(object input)
    {
        if (input is string)
        {
            return IsSafeInput((string)input);
        }
        else if (input is null)
        {
            return true; // Null input is considered safe
        }
        else if (input is IDictionary)
        {
            foreach (var entry in (IDictionary)input)
            {
                //TODO:
                //if (!IsSafeInput(entry.Value))
                //{
                //    return false; // Potential SQL injection detected in the object's properties
                //}
            }
        }
        else if (input is IEnumerable)
        {
            foreach (var item in (IEnumerable)input)
            {
                if (!IsSafeInput(item))
                {
                    return false; // Potential SQL injection detected in the object's elements
                }
            }
        }

        return true; // Input is safe
    }

    #endregion
}