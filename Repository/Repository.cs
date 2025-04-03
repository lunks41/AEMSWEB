using AMESWEB.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AMESWEB.Repository
{
    //Unit of Work Pattern
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IConfiguration _configuration;

        public Repository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _configuration = configuration;
        }

        public async Task<IEnumerable<T>> GetQueryAsync<T>(string spName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var entities = await connection.QueryAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQuerySingleOrDefaultAsync<T>(string SpName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var entities = await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQueryFirstAsync<T>(string spName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var entities = await connection.QueryFirstAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<int> GetRowExecuteAsync(string spName)
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection())
            {
                rowsAffected = await connection.ExecuteAsync(spName);
            }
            return rowsAffected;
        }

        public async Task<bool> GetQuerySingleOrDefaultAsync(string spName)
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection())
            {
                rowsAffected = await connection.QueryFirstOrDefaultAsync<int>(spName);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<T> GetQuerySingleOrDefaultAsyncV1(string SpName)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var entities = await connection.QuerySingleOrDefaultAsync(SpName);
                return entities;
            }
        }

        public async Task<bool> GetExecuteScalarAsync(string SpName, object? Parameters)
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection())
            {
                rowsAffected = await connection.ExecuteScalarAsync<int>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<bool> UpsertExecuteScalarAsync(string QueryString)
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection())
            {
                rowsAffected = await connection.ExecuteScalarAsync<int>(QueryString);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<DataSet> GetExecuteDataSetQuery(string storedProcedureName)
        {
            using IDbConnection connection = CreateConnection();
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetExecuteDataSetStoredProcedure(string storedProcedureName, DynamicParameters? parameters = null)
        {
            using IDbConnection connection = CreateConnection();
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (var paramName in parameters.ParameterNames)
                    {
                        var paramValue = parameters.Get<object>(paramName);
                        sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@" + paramName, paramValue);
                    }
                }
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedureName, DynamicParameters parameters, string outputParameterName)
        {
            using (IDbConnection connection = CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<T>(outputParameterName);
            }
        }

        #region Private Methods

        private IDbConnection CreateConnection()
        {
            IDbConnection db = new SqlConnection(_configuration.GetConnectionString("Default"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            return db;
        }

        #endregion Private Methods
    }
}