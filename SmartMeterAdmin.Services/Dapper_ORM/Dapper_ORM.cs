using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Dapper_ORM
{
    public class Dapper_ORM : IDapper
    {
        private readonly IConfiguration _config;
        private readonly string Connectionstring = "DevConnection";

        public Dapper_ORM(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string query, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await connection.QueryAsync<T>(query, parameters);
        }
        public async Task Delete(string query, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            await connection.ExecuteAsync(query, parameters);
        }
        public async Task ExecuteProcedure(string procedureName, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task ExecuteProcedure(string procedureName)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            await connection.ExecuteAsync(procedureName, commandType: CommandType.StoredProcedure);
        }

        public async Task<T> GetFirstOrDefaultAsync<T>(string query, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }

        public async Task<T> ExecuteProcedureFirstOrDefault<T>(string procedureName, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);

        }
        public async Task<IEnumerable<T>> ExecuteProcedure<T>(string procedureName, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<SqlMapper.GridReader> ExecuteProcedureMultiple(string procedureName, DynamicParameters parameters)
        {
            var connection = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await connection.QueryMultipleAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
