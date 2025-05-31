using System.Transactions;

namespace ServerDeployment.infrastructure.Contracts.IDataAccessor;

public interface IDataAccess
{
    Task<string> InsertAsync<T>(T obj, string tableName = "") where T : new();
    string Insert<T>(T obj, string tableName = "") where T : new();

    Task<int> InsertRangeAsync<T>(ICollection<T> lst, string tableName = "") where T : new();

    //Not tested yet.
    Task<int> UpdateAsync<T>(T obj, string tableName = null) where T : new();
    Task<int> ExecuteSqlAsync(string sqlSchema);
    Task<int> UpdateRangeAsync<T>(ICollection<T> lst, string type = null) where T : new();
    Task<int> DeleteAsync<T>(T obj) where T : new();

    Task<int> DeleteRangeAsync<T>(ICollection<T> lst) where T : new();

    //End not tested yet.
    //Task<T> GetByIdAsync<T>(int Id) where T : new();
    Task<T?> GetByIdAsync<T>(long id) where T : new();
    T GetById<T>(int id) where T : new();
    Task<T?> GetAsync<T>(string sqlQuery) where T : new();
    Task<ICollection<T>> GetAllAsync<T>(string sqlQuery) where T : new();
    ICollection<T> GetAll<T>(string sqlQuery) where T : new();
    Task<int> GetCountAsync<T>(string filter) where T : new();
    Task<int> GetCountBySqlAsync(string sqlJoins);


    TransactionScope BeginTransaction();
}