using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace BM.DAL
{
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public class CommonDAL
    {
        /// <summary>
        /// 设置数据库连接串，指定数据库名称
        /// </summary>
        public static string GetConnectionString(string pDataBase)
        {
            string DbHost = ConfigurationManager.AppSettings["DataSource"].ToString();
            string DbUid = ConfigurationManager.AppSettings["UserId"].ToString();
            string DbPwd = ConfigurationManager.AppSettings["Password"].ToString();
            string DBConnOther = ConfigurationManager.AppSettings["ConnOther"].ToString();

            return string.Format("Data Source={0};user id={1};password={2};initial catalog={3};{4}", DbHost, DbUid, DbPwd, pDataBase, DBConnOther);
        }

        /// <summary>
        /// 设置数据库连接串
        /// </summary>
        public static string GetConnectionString()
        {
            /// <summary>
            /// 数据库连接字符串
            /// </summary>
            string connectionString = ConfigurationManager.ConnectionStrings["BM_DBContext"].ConnectionString;

            return connectionString;
        }

        /// <summary>
        /// 返回DataTable类型的数据列表
        /// </summary>
        public static DataTable GetdtList(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 返回事务中的DataTable类型的数据列表
        /// </summary>
        public static DataTable GetdtList(string SQLString, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, SQLString, cmdParms);
            using (SqlDataAdapter da = new SqlDataAdapter(Trx.SqlCmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                return dt;
            }
        }

        /// <summary>
        /// ExecuteNonQuery 执行一条无返回值的 SQL 语句
        /// </summary>
        public static int ExecuteNonQuery(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlConnection lSqlConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand lSqlCommand = new SqlCommand())
                {
                    PrepareCommand(lSqlCommand, lSqlConnection, null, pcSQL, cmdParms);
                    int liResult = lSqlCommand.ExecuteNonQuery();
                    lSqlCommand.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                    return liResult;
                }
            }
        }

        /// <summary>
        /// ExecuteNonQuery 在事务中执行一条 SQL 语句
        /// </summary>
        public static int ExecuteNonQuery(string pcSQL, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, pcSQL, cmdParms);
            int liResult = Trx.SqlCmd.ExecuteNonQuery();
            Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
            return liResult;
        }

        /// <summary>
        /// 在事务中执行一条或多条 SQL 语句
        /// </summary>
        public static int ExecuteTransaction(string pcSQL)
        {
            return ExecuteTransaction(pcSQL, null);
        }

        /// <summary>
        /// 在事务中执行一条或多条 SQL 语句
        /// </summary>
        public static int ExecuteTransaction(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlTrxHelper Trx = new SqlTrxHelper())
            {
                int liResult = ExecuteNonQuery(pcSQL, Trx, cmdParms);
                Trx.SqlTrx.Commit();
                return liResult;
            }
        }

        #region 使用SqlBulkCopy将DataTable中的数据批量插入数据库中
        /// <summary> 
        /// 注意：DataTable中的列需要与数据库表中的列完全一致。
        /// </summary> 
        /// <param name="">数据库名</param>
        /// <param name="strTableName">数据库中对应的表名</param> 
        /// <param name="dtData">数据集</param> 
        public static int SqlBulkCopyInsert(string strTableName, DataTable dtData)
        {
            using (SqlBulkCopy sqlRevdBulkCopy = new SqlBulkCopy(GetConnectionString()))//引用SqlBulkCopy 
            {
                sqlRevdBulkCopy.DestinationTableName = strTableName;//数据库中对应的表名 
                sqlRevdBulkCopy.NotifyAfter = dtData.Rows.Count;//有几行数据 
                sqlRevdBulkCopy.WriteToServer(dtData);//数据导入数据库 
                sqlRevdBulkCopy.Close();//关闭连接 
                return dtData.Rows.Count;
            }
        }
        #endregion

        /// <summary>
        /// 读取一个整数值, 若是无值 DBNull 则返回 0
        /// </summary>
        public static int ReadInteger(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlConnection lSqlConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand lSqlCommand = new SqlCommand())
                {
                    PrepareCommand(lSqlCommand, lSqlConnection, null, pcSQL, cmdParms);
                    object ob = lSqlCommand.ExecuteScalar();
                    lSqlCommand.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                    return ob is DBNull ? 0 : Convert.ToInt32(ob);
                }
            }
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        public static int ReadInteger(string pcSQL, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, pcSQL, cmdParms);
            object ob = Trx.SqlCmd.ExecuteScalar();
            Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
            return ob is DBNull ? 0 : Convert.ToInt32(ob);
        }

        /// <summary>
        /// 读取一个浮点型值, 若是无值 DBNull 则返回 0
        /// </summary>
        public static double ReadDouble(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlConnection lSqlConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand lSqlCommand = new SqlCommand())
                {
                    PrepareCommand(lSqlCommand, lSqlConnection, null, pcSQL, cmdParms);
                    object ob = lSqlCommand.ExecuteScalar();
                    lSqlCommand.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                    return ob is DBNull ? 0 : Convert.ToDouble(ob);
                }
            }
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        public static double ReadDouble(string pcSQL, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, pcSQL, cmdParms);
            object ob = Trx.SqlCmd.ExecuteScalar();
            Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
            return ob is DBNull ? 0 : Convert.ToDouble(ob);
        }

        /// <summary>
        /// 读取一个字符串, 若为空 DBNull 返回 ""
        /// </summary>
        public static string ReadString(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlConnection lSqlConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand lSqlCommand = new SqlCommand())
                {
                    PrepareCommand(lSqlCommand, lSqlConnection, null, pcSQL, cmdParms);
                    object ob = lSqlCommand.ExecuteScalar();
                    lSqlCommand.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                    if (ob is DBNull) return "";
                    else return Convert.ToString(ob);
                }
            }
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.
        /// 读取一个字符串, 若为空 DBNull 返回 ""
        /// </summary>
        public static string ReadString(string pcSQL, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, pcSQL, cmdParms);
            object ob = Trx.SqlCmd.ExecuteScalar();
            Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
            if (ob is DBNull) return "";
            else return Convert.ToString(ob);
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.
        /// 读取一个字符串, 若为空 DBNull 返回 "", 若无记录，返回"CIMS_NULL"
        /// </summary>
        public static string ReadStringNull(string pcSQL, SqlTrxHelper Trx, params SqlParameter[] cmdParms)
        {
            PrepareCommand(Trx, pcSQL, cmdParms);
            object ob = Trx.SqlCmd.ExecuteScalar();
            Trx.SqlCmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
            if (ob == null) return "CIMS_NULL";
            else if (ob is DBNull) return "";
            else return Convert.ToString(ob);
        }

        /// <summary>
        /// 读取一个字符串, 若为空 DBNull 返回 "", 若无记录，返回"CIMS_NULL"
        /// </summary>
        public static string ReadStringNull(string pcSQL, params SqlParameter[] cmdParms)
        {
            using (SqlConnection lSqlConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand lSqlCommand = new SqlCommand())
                {
                    PrepareCommand(lSqlCommand, lSqlConnection, null, pcSQL, cmdParms);
                    object ob = lSqlCommand.ExecuteScalar();
                    lSqlCommand.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                    if (ob == null) return "CIMS_NULL";
                    else if (ob is DBNull) return "";
                    else return Convert.ToString(ob);
                }
            }
        }

        private static void PrepareCommand(SqlTrxHelper Trx, string cmdText, params SqlParameter[] cmdParms)
        {
            Trx.SqlCmd.CommandText = cmdText;
            Trx.SqlCmd.Parameters.Clear();
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    Trx.SqlCmd.Parameters.Add(parameter);
                }
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, params SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandTimeout = 120;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            cmd.Parameters.Clear();
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 字典转化为实体类
        /// </summary>
        public static T DictionaryToMap<T>(Dictionary<string, object> pDictionary) where T : new()
        {
            PropertyInfo[] pros = typeof(T).GetProperties();
            T obj = new T();
            foreach (PropertyInfo pro in pros)
            {
                if (pDictionary.ContainsKey(pro.Name.ToLower()))
                {
                    object pDictionaryName = pro.Name.ToLower();
                    object pDictionaryValue = pDictionary[pro.Name.ToLower()];

                    if (!string.IsNullOrEmpty(pDictionaryValue.ToString()))
                    {
                        switch (pro.PropertyType.Name)
                        {
                            case "Byte[]":
                                pro.SetValue(obj, (byte[])pDictionaryValue, null);
                                break;

                            default:
                                try
                                {
                                    pro.SetValue(obj, pro.PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { pDictionaryValue.ToString() }), null);
                                }
                                catch (Exception)
                                {
                                    pro.SetValue(obj, Convert.ToString(pDictionaryValue).Trim(), null);
                                }
                                break;
                        }
                    }
                }
            }
            return obj;
        }

        public static List<T> GetetList<T>(string SQLString, params SqlParameter[] cmdParms) where T : new()
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, con, null, SQLString, cmdParms);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        cmd.Parameters.Clear();     //framework机制限制两个SqlParameterCollection指向同一个对象
                        List<T> objs = new List<T>();
                        PropertyInfo[] pros = typeof(T).GetProperties();
                        while (sdr.Read())
                        {
                            T obj = new T();
                            for (int i = 0; i < sdr.FieldCount; i++)
                            {
                                foreach (PropertyInfo pro in pros)
                                {
                                    if (sdr.GetName(i).ToUpper() == pro.Name.ToUpper() && sdr[pro.Name].GetType() != typeof(DBNull) && sdr[pro.Name] != DBNull.Value)
                                    {
                                        switch (pro.PropertyType.Name)
                                        {
                                            case "Byte[]":
                                                pro.SetValue(obj, (byte[])sdr[pro.Name], null);
                                                break;
                                            default:
                                                try
                                                {
                                                    pro.SetValue(obj, pro.PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { sdr[pro.Name].ToString() }), null);
                                                }
                                                catch (Exception)
                                                {
                                                    pro.SetValue(obj, Convert.ToString(sdr[pro.Name]).Trim(), null);
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            objs.Add(obj);
                        }
                        return objs;
                    }
                }
            }
        }

        private static void SetParameters(SqlCommand SqlCmd, SqlParameter[] sqlParams)
        {
            SqlCmd.Parameters.Clear();
            if (sqlParams != null)
            {
                foreach (SqlParameter parameter in sqlParams)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
                        parameter.Value = DBNull.Value;
                    SqlCmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 读取数据库当前日期
        /// </summary>
        public static string ReadCurrDate()
        {
            return ReadString("Select CONVERT(varchar(100), GETDATE(), 21)");
        }


        #region 存储过程操作

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
        public static DataTable RunProcedureDataTable(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
        public static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
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
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
        public static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion
    }
}
