using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Dapper_ORM
{
    public interface IDapper
    {
        Task<IEnumerable<T>> GetAllAsync<T>(string query, DynamicParameters parameters);
        Task ExecuteProcedure(string procedureName, DynamicParameters parameters);
        Task Delete(string query, DynamicParameters parameters);
        Task ExecuteProcedure(string procedureName);
        Task<T> GetFirstOrDefaultAsync<T>(string query, DynamicParameters parameters);
        Task<T> ExecuteProcedureFirstOrDefault<T>(string procedureName, DynamicParameters parameters);
        Task<IEnumerable<T>> ExecuteProcedure<T>(string procedureName, DynamicParameters parameters);
        Task<SqlMapper.GridReader> ExecuteProcedureMultiple(string procedureName, DynamicParameters parameters);
    }
}
