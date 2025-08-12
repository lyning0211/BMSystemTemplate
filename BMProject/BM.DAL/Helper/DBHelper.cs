using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BM.DAL.Helper
{
    /// <summary>
    /// 数据库访问接口
    /// internal 只有同一命名空间才能访问，防止逻辑层调用此接口
    /// </summary>
    internal class DBHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        internal static string connectionString = ConfigurationManager.ConnectionStrings["BM_DBContext"].ConnectionString;
        #region 构造函数
        public DBHelper()
        {
        }
        #endregion

        #region 执行简单SQL语句
        /// <summary>
        /// 执行SQL语句（增删改），返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        internal static int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// RunSql(SqlCommand,bool) 执行Sql语句
        /// </summary>
        /// <param name="m_SqlCommand">SqlCommand 对象</param>
        /// <returns>影响行数</returns>
        internal static int ExecuteSql(SqlCommand m_SqlCommand)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    m_SqlCommand.Connection = connection;
                    connection.Open();
                    return m_SqlCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        internal static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        internal static object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条语句，返回一个值(第一行第一列)
        /// </summary>
        /// <param name="m_SqlCommand">SqlCommand 对象</param>
        /// <returns>object对象</returns>
        internal static object GetSingle(SqlCommand m_SqlCommand)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                m_SqlCommand.Connection = conn;
                return m_SqlCommand.ExecuteScalar();
            }
            catch (Exception exE)
            {
                conn.Close();
                throw new Exception("执行GetSingleItem错误，请检查Sql语句。\n" + m_SqlCommand.CommandText + System.Environment.NewLine + exE.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        /// <summary>
        /// 根据查询语句获取结果
        /// </summary>
        /// <param name="m_strSqlString">查询语句</param>
        /// <param name="m_strTableName">DataTable名</param>
        /// <returns></returns>
        internal static DataTable GetDataTable(string m_strSqlString, string m_strTableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable(m_strTableName);
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(m_strSqlString, connection);
                    command.Fill(dt);
                    return dt;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 根据查询语句获取结果
        /// </summary>
        /// <param name="m_strSqlString">查询语句</param>
        /// <returns>返回DataTable</returns>
        internal static DataTable GetDataTable(string m_strSqlString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(m_strSqlString, connection);
                    command.Fill(dt);
                    return dt;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 根据查询语句获取结果(带参数查询)
        /// </summary>
        /// <param name="m_SqlCommand">SqlCommand 对象</param>
        /// <param name="m_strTableName">DataTable表名</param>
        /// <returns>DataTable对象</returns>
        internal static DataTable GetDataTable(SqlCommand m_SqlCommand)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    m_SqlCommand.Connection = connection;
                    SqlDataAdapter command = new SqlDataAdapter(m_SqlCommand);
                    command.Fill(dt);
                    return dt;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion       

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        internal static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter
                {
                    SelectCommand = BuildQueryCommand(connection, storedProcName, parameters)
                };
                sqlDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        internal static DataTable RunProcedureDataTable(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dataSet = new DataTable();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程获取单个数据
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataTable</returns>
        public static object RunProcedureSingle(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dt);
                connection.Close();
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0];
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        internal static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        internal static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        internal static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion

        /// <summary>
        /// 读取数据库当前日期
        /// </summary>
        public static string ReadCurrDate()
        {
            return GetSingle("Select CONVERT(varchar(100), GETDATE(), 21)").ToString();
        }
    }
}
