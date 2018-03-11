using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebAPI
{
    public class SQLServerHelper
    {

        public static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection"].ToString();

        #region 执行查询，返回DataTable对象-----------------------
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTableBySql(string strSQL)
        {
            return GetTableBySql(strSQL, null);
        }
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="pas">参数数组</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTableBySql(string strSQL, DbParameter[] pas)
        {
            return GetTable(strSQL, pas, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTableByProc(string procName)
        {
            return GetTableByProc(procName, null);
        }
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="pas">参数数组</param>
        /// <returns>DataTable对象</returns>
        public static DataTable GetTableByProc(string procName, DbParameter[] pas)
        {
            return GetTable(procName, pas, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="strSQL">sql语句或存储过程名</param>
        /// <param name="pas">参数数组</param>
        /// <param name="cmdtype">Command类型</param>
        /// <returns>DataTable对象</returns>
        private static DataTable GetTable(string strSQL, DbParameter[] pas, CommandType cmdtype)
        {
            DataTable dt = new DataTable(); ;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                da.SelectCommand.CommandType = cmdtype;
                if (pas != null)
                {
                    da.SelectCommand.Parameters.AddRange(pas);
                }
                da.Fill(dt);
            }
            return dt;
        }
        #endregion
        #region 执行查询，返回DataSet对象-------------------------
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>DataSet对象</returns>
        public static DataSet GetDataSetBySql(string strSQL)
        {
            return GetDataSetBySql(strSQL, null);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="pas">参数数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet GetDataSetBySql(string strSQL, DbParameter[] pas)
        {
            return GetDataSet(strSQL, pas, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>DataSet对象</returns>
        public static DataSet GetDataSetByProc(string procName)
        {
            return GetDataSetByProc(procName, null);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="pas">参数数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet GetDataSetByProc(string procName, DbParameter[] pas)
        {
            return GetDataSet(procName, pas, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="strSQL">sql语句或存储过程名</param>
        /// <param name="pas">参数数组</param>
        /// <param name="cmdtype">Command类型</param>
        /// <returns>DataSet对象</returns>
        private static DataSet GetDataSet(string strSQL, DbParameter[] pas, CommandType cmdtype)
        {
            DataSet dt = new DataSet(); ;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                da.SelectCommand.CommandType = cmdtype;
                if (pas != null)
                {
                    da.SelectCommand.Parameters.AddRange(pas);
                }
                da.Fill(dt);
            }
            return dt;
        }
        #endregion
        #region 执行非查询存储过程和SQL语句-----------------------------
        /// <summary>
        /// 执行SQL脚本,返回受影响的行数
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>受影响行数</returns>
        public static int ExcuteSql(string strSQL)
        {
            return ExcuteSql(strSQL, null);
        }
        /// <summary>
        /// 执行SQL脚本,返回受影响的行数
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="paras"></param>
        /// <returns>受影响的行数</returns>
        public static int ExcuteSql(string strSQL, DbParameter[] paras)
        {
            return ExecuteNonQuery(strSQL, paras, CommandType.Text);
        }

        /// <summary>
        /// 执行存储过程,返回受影响的行数
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <returns>受影响行数</returns>
        public static int ExcuteProc(string ProcName)
        {
            return ExcuteProc(ProcName, null);
        }
        /// <summary>
        /// 执行存储过程,返回受影响的行数
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="pars">参数数组</param>
        /// <returns>受影响行数</returns>
        public static int ExcuteProc(string ProcName, DbParameter[] pars)
        {
            return ExecuteNonQuery(ProcName, pars, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行SQL脚本,返回受影响的行数
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句或存储过程</param>
        /// <param name="paras">参数数组</param>
        /// <param name="cmdType">Command类型</param>
        /// <returns>返回影响行数</returns>
        private static int ExecuteNonQuery(string strSQL, DbParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.CommandType = cmdType;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                i = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return i;
        }
        /// <summary>
        /// 批量执行SQL语句，并使用事务控制
        /// 增、删、改
        /// </summary>
        /// <param name="strSQL">批量要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>受影响行数</returns>
        public static int ExcuteSql(SqlTransaction tran, string strSQL, DbParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            SqlCommand cmd = new SqlCommand(strSQL, tran.Connection, tran);
            cmd.CommandType = cmdType;
            try
            {
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                i = 0;
                tran.Rollback();
                throw;
            }
            finally
            {
            }
            return i;
        }
        #endregion
        #region 执行查询返回首行首列---------------------------------
        /// <summary>
        /// 执行sql查询返回首行首列
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>首行首列</returns>
        public static object GetObjectBySql(string strSQL)
        {
            return GetObjectBySql(strSQL, null);
        }
        /// <summary>
        /// 执行sql查询返回首行首列
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns>首行首列</returns>
        public static object GetObjectBySql(string strSQL, DbParameter[] paras)
        {
            return GetObject(strSQL, paras, CommandType.Text);
        }
        /// <summary>
        /// 执行存储过程查询返回首行首列
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>首行首列</returns>
        public static object GetObjectByProc(string procName)
        {
            return GetObjectByProc(procName, null);
        }
        /// <summary>
        /// 执行存储过程查询返回首行首列
        /// </summary>
        /// <param name="strSQL">存储过程名称</param>
        /// <param name="paras">参数数组</param>
        /// <returns>首行首列</returns>
        public static object GetObjectByProc(string procName, DbParameter[] paras)
        {
            return GetObject(procName, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行SQL语句，返回第一行，第一列
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>返回影响行数</returns>
        private static object GetObject(string strSQL, DbParameter[] paras, CommandType cmdType)
        {
            object i = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.CommandType = cmdType;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                i = cmd.ExecuteScalar();
                conn.Close();
            }
            return i;
        }
        /// <summary>
        /// 执行SQL语句，返回第一行，第一列
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句或存储过程名</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>返回影响行数</returns>
        public static object GetObject(SqlTransaction tran, string strSQL, DbParameter[] paras, CommandType cmdType)
        {
            object result = ""; ;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(strSQL, tran.Connection, tran);
                    cmd.CommandType = cmdType;
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    conn.Open();
                    result = cmd.ExecuteScalar();
                    conn.Close();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
            return result;
        }
        #endregion
        #region 查询获取DataReader------------------------------------
        /// <summary>
        /// 调用不带参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderByProc(string procName)
        {
            return GetReaderByProc(procName, null);
        }
        /// <summary>
        /// 调用带有参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderByProc(string procName, DbParameter[] paras)
        {
            return GetReader(procName, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 根据sql语句返回DataReader对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderBySql(string strSQL)
        {
            return GetReaderBySql(strSQL, null);
        }
        /// <summary>
        /// 根据sql语句和参数返回DataReader对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public static SqlDataReader GetReaderBySql(string strSQL, DbParameter[] paras)
        {
            return GetReader(strSQL, paras, CommandType.Text);
        }
        /// <summary>
        /// 查询SQL语句获取DataReader
        /// </summary>
        /// <param name="strSQL">查询的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>查询到的DataReader（关闭该对象的时候，自动关闭连接）</returns>
        public static SqlDataReader GetReader(string strSQL, DbParameter[] paras, CommandType cmdtype)
        {
            SqlDataReader sqldr = null;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(strSQL, conn);
            cmd.CommandType = cmdtype;
            if (paras != null)
            {
                cmd.Parameters.AddRange(paras);
            }
            conn.Open();
            //CommandBehavior.CloseConnection的作用是如果关联的DataReader对象关闭，则连接自动关闭
            sqldr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return sqldr;
        }
        #endregion
        #region 批量插入数据---------------------------------------------
        /// <summary>
        /// 往数据库中批量插入数据
        /// </summary>
        /// <param name="sourceDt">数据源表</param>
        /// <param name="targetTable">服务器上目标表</param>
        public static void BulkToDB(DataTable sourceDt, string targetTable)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);   //用其它源的数据有效批量加载sql server表中
            bulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称
            bulkCopy.BatchSize = sourceDt.Rows.Count;   //每一批次中的行数
            try
            {
                conn.Open();
                if (sourceDt != null && sourceDt.Rows.Count != 0)
                    bulkCopy.WriteToServer(sourceDt);   //将提供的数据源中的所有行复制到目标表中
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }
        #endregion
        /// <summary>
        /// 开始数据库事务,返回一个事务用来进行批量操作
        /// </summary>
        /// <returns>事务</returns>
        public static SqlTransaction BeginTran()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            return tran;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="tran">事务</param>
        public static void CommitTran(SqlTransaction tran)
        {
            tran.Commit();
            if (tran.Connection != null)
            {
                tran.Connection.Dispose();
                tran.Connection.Close();
            }
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="tran"></param>
        public static void RollBack(SqlTransaction tran)
        {
            tran.Rollback();
        }

    }
}