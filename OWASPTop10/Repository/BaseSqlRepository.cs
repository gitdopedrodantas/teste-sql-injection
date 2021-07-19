using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Data;
using System.Linq;

namespace OWASPTop10.Repository
{
    public class BaseSqlRepository
    {
        protected readonly IDatabaseProvider _databaseProvider;

        public BaseSqlRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            databaseProvider.SetStringConnection();
        }

        protected async Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                TryOpenConnection(connection);
                using (var transaction = TryBeginTransaction(connection))
                {
                    try
                    {
                        var content = await connection.ExecuteAsync(
                            sql: sql,
                            param: param,
                            transaction: transaction,
                            commandTimeout: commandTimeout
                            ).ConfigureAwait(false);

                        transaction.Commit();

                        return content;
                    }
                    catch (SqlException e)
                    {
                        throw SqlExceptionRollbackHandler(transaction, e);
                    }
                }
            }
        }

        protected async Task<int> InsertListAsync(string sql, IEnumerable<object> parameters, int? commandTimeout = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                TryOpenConnection(connection);
                using (var transaction = TryBeginTransaction(connection))
                {
                    try
                    {
                        var affectedRows = 0;

                        foreach (var parameter in parameters)
                        {
                            affectedRows += await connection.ExecuteAsync(
                                sql: sql,
                                param: parameter,
                                transaction: transaction,
                                commandTimeout: commandTimeout
                                ).ConfigureAwait(false);
                        }

                        if (affectedRows == parameters.Count())
                        {
                            transaction.Commit();
                            return affectedRows;
                        }

                        throw new Exception();
                    }
                    catch (SqlException e)
                    {
                        throw SqlExceptionRollbackHandler(transaction, e);
                    }
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    return await connection.QueryAsync<T>(
                        sql: sql,
                        param: param,
                        commandTimeout: commandTimeout,
                        commandType: commandType
                        ).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TChild>(string sql, Func<T, TChild, T> MapObjectFunc, object param = null, int? commandTimeout = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    return await connection.QueryAsync<T, TChild, T>(
                        sql: sql,
                        map: (item, child) =>
                        {
                            return MapObjectFunc(item, child);
                        },
                        param: param,
                        commandTimeout: commandTimeout
                        ).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TChild, TSecondChild>(string sql, Func<T, TChild, TSecondChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "ID")
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                return await connection.QueryAsync<T, TChild, TSecondChild, T>(
                    sql: sql,
                    map: (item, child, secondChild) =>
                    {
                        return MapObjectFunc(item, child, secondChild);
                    },
                    param: param,
                    commandTimeout: commandTimeout,
                    splitOn: splitOn
                    ).ConfigureAwait(false);
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TId, TChild>(string sql, Func<T, TId> GetIDFunc, Func<T, TChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "Id")
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    var toBeReturned = new Dictionary<TId, T>();
                    await connection.QueryAsync<T, TChild, T>(
                        sql: sql,
                        map: (item, child) =>
                        {
                            var id = GetIDFunc(item);
                            if (toBeReturned.ContainsKey(id))
                            {
                                item = toBeReturned[id];
                            }
                            else
                            {
                                toBeReturned[id] = item;
                            }
                            return MapObjectFunc(item, child);
                        },
                        param: param,
                        commandTimeout: commandTimeout,
                        splitOn: splitOn
                        ).ConfigureAwait(false);

                    return toBeReturned.Values;
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TId, TChild, TSecondChild>(string sql, Func<T, TId> GetIDFunc, Func<T, TChild, TSecondChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "Id")
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    var toBeReturned = new Dictionary<TId, T>();
                    await connection.QueryAsync<T, TChild, TSecondChild, T>(
                        sql: sql,
                        map: (item, child, secondChild) =>
                        {
                            var id = GetIDFunc(item);
                            if (toBeReturned.ContainsKey(id))
                            {
                                item = toBeReturned[id];
                            }
                            else
                            {
                                toBeReturned[id] = item;
                            }
                            return MapObjectFunc(item, child, secondChild);
                        },
                        param: param,
                        commandTimeout: commandTimeout,
                        splitOn: splitOn
                        ).ConfigureAwait(false);

                    return toBeReturned.Values;
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TId, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild>(string sql, Func<T, TId> GetIDFunc, Func<T, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "Id")
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    var toBeReturned = new Dictionary<TId, T>();
                    await connection.QueryAsync<T, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild, T>(
                        sql: sql,
                        map: (item, child, secondChild, thirdChild, fourthChild, fifthChild) =>
                        {
                            var id = GetIDFunc(item);
                            if (toBeReturned.ContainsKey(id))
                            {
                                item = toBeReturned[id];
                            }
                            else
                            {
                                toBeReturned[id] = item;
                            }
                            return MapObjectFunc(item, child, secondChild, thirdChild, fourthChild, fifthChild);
                        },
                        param: param,
                        commandTimeout: commandTimeout,
                        splitOn: splitOn
                        ).ConfigureAwait(false);

                    return toBeReturned.Values;
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild>(string sql, Func<T, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "Id")

        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    return await connection.QueryAsync<T, TChild, TSecondChild, TThirdChild, TFourthChild, TFifthChild, T>(
                        sql: sql,
                        map: (item, child, secondChild, thirdChild, fourthChild, fifthChild) =>
                        {
                            return MapObjectFunc(item, child, secondChild, thirdChild, fourthChild, fifthChild);
                        },
                        param: param,
                        commandTimeout: commandTimeout,
                        splitOn: splitOn
                        ).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<IEnumerable<T>> QueryAsync<T, TChild, TSecondChild, TThirdChild, TFourthChild>(string sql, Func<T, TChild, TSecondChild, TThirdChild, TFourthChild, T> MapObjectFunc, object param = null, int? commandTimeout = null, string splitOn = "Id")
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    return await connection.QueryAsync<T, TChild, TSecondChild, TThirdChild, TFourthChild, T>(
                        sql: sql,
                        map: (item, child, secondChild, thirdChild, fourthChild) =>
                        {
                            return MapObjectFunc(item, child, secondChild, thirdChild, fourthChild);
                        },
                        param: param,
                        commandTimeout: commandTimeout,
                        splitOn: splitOn
                        ).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                try
                {
                    return await connection.QueryFirstOrDefaultAsync<T>(
                        sql: sql,
                        param: param,
                        commandTimeout: commandTimeout,
                        commandType: commandType
                        ).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null)
        {
            using (var connection = _databaseProvider.GetConnection() as SqlConnection)
            {
                TryOpenConnection(connection);
                using (var transaction = TryBeginTransaction(connection))
                {
                    try
                    {
                        var result = await connection.ExecuteScalarAsync<T>(
                            sql: sql,
                            param: param,
                            transaction: transaction,
                            commandTimeout: commandTimeout
                            ).ConfigureAwait(false);

                        transaction.Commit();

                        return result;
                    }
                    catch (SqlException e)
                    {
                        throw SqlExceptionRollbackHandler(transaction, e);
                    }
                }
            }
        }

        protected Exception SqlExceptionRollbackHandler(IDbTransaction transaction, SqlException exception)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception innerException)
            {
                return new Exception();
            }

            return new Exception(exception.Message);
        }



        protected void TryOpenConnection(IDbConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
            }
        }

        protected IDbTransaction TryBeginTransaction(IDbConnection connection)
        {
            try
            {
                return connection.BeginTransaction();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
