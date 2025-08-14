using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BM.DAL
{
    public class SqlTrxHelper : IDisposable
    {
        public SqlConnection SqlConn;
        public SqlCommand SqlCmd;
        public SqlTransaction SqlTrx;

        public SqlTrxHelper()
        {
            SqlConn = new SqlConnection(CommonDAL.GetConnectionString());
            SqlConn.Open();
            SqlTrx = SqlConn.BeginTransaction();
            SqlCmd = SqlConn.CreateCommand();
            SqlCmd.Transaction = SqlTrx;
            SqlCmd.CommandTimeout = 120;//120秒
            SqlCmd.CommandType = CommandType.Text;
        }

        public void SetParameters(Dictionary<string, object> dictParams)
        {
            SqlCmd.Parameters.Clear();
            if (dictParams != null)
            {
                foreach (SqlParameter parameter in dictParams.Select(kvp => new SqlParameter("@" + kvp.Key.ToLower(), kvp.Value)).ToArray())
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
                        parameter.Value = DBNull.Value;
                    SqlCmd.Parameters.Add(parameter);
                }
            }
        }

        public void SetParameters(SqlParameter[] sqlParams)
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

        public void DisposeSqlTrx()
        {
            SqlCmd.Dispose();
            SqlTrx.Dispose();
            SqlConn.Close();
            SqlConn.Dispose();
        }

        void IDisposable.Dispose()
        {
            SqlCmd.Dispose();
            SqlTrx.Dispose();
            SqlConn.Close();
            SqlConn.Dispose();
        }
    }
}
