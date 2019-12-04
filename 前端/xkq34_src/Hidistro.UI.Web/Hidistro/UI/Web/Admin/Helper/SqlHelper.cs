using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;

namespace Hidistro.UI.Web.Admin.Helper
{
    public class SqlHelper
    {
        #region 连接字符串
        /// <summary>
        /// 连接字符
        /// </summary>
        //private static readonly string connectionString = "server=(local);database=K6_FPCON;uid=sa;pwd=chenqiao;MultipleActiveResultSets=true";
        //private static readonly string connectionString = "server=.;database=demo;uid=sa;pwd=123456;MultipleActiveResultSets=true";
        //private static readonly string connectionString = "server=(local);database=K6_FPCON;uid=sa;pwd=chenqiao;MultipleActiveResultSets=true";
        private static readonly string connectionString = "server=.;database=xkd34;uid=sa;pwd=123456;MultipleActiveResultSets=true";
        //private readonly string connectionString = ConfigurationManager.ConnectionStrings["sqlConnection"].ConnectionString;
        #endregion

        #region 获取连接
        /// <summary>
        /// 获取连接
        /// </summary>
        private SqlConnection _conn;
        public SqlConnection Conn
        {
            get
            {
                if (_conn == null) _conn = new SqlConnection(connectionString);

                if (_conn.State == ConnectionState.Broken)
                {
                    _conn.Close();
                    _conn.Open();
                }
                else if (_conn.State == ConnectionState.Closed)
                {
                    _conn.Open();
                }
                return _conn;
            }
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (Conn == null) return;
            if (Conn.State == ConnectionState.Open || Conn.State == ConnectionState.Broken)
            {
                Conn.Close();
            }
        }
        #endregion

        #region 增、删、改
        /// <summary>
        /// 增、删、改
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="type">执行解析类型 默认 sql</param>
        /// <param name="param">参数</param>
        /// <returns>数据库受影响行数</returns>
        public int ExecuteNonQuery(string sql, CommandType type = CommandType.Text, params SqlParameter[] param)
        {
            try
            {
                //创建执行对象
                SqlCommand cmd = new SqlCommand(sql, Conn);
                //判断是否存在参数
                if (param != null && param.Length > 0)
                {
                    cmd.Parameters.AddRange(param);
                }
                cmd.CommandType = type;//指定解析类型

                return cmd.ExecuteNonQuery();//返回执行后的结果
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }


        }
        #endregion

        #region 查询单个结果
        /// <summary>
        /// 查询单个结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="type">执行解析类型 默认 sql</param>
        /// <param name="param">参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列</returns>
        public int ExecuteScalar(string sql, CommandType type = CommandType.Text, params SqlParameter[] param)
        {
            try
            {
                //创建执行对象
                SqlCommand cmd = new SqlCommand(sql, Conn);
                //判断是否存在参数
                if (param != null && param.Length > 0)
                {
                    cmd.Parameters.AddRange(param);
                }
                cmd.CommandType = type;//指定解析类型

                return Convert.ToInt32(cmd.ExecuteScalar());//返回执行后的结果
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region 异步操作的任务查询
        /// <summary>
        /// 异步操作的任务查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="type">执行解析类型 默认 sql</param>
        /// <param name="param">参数</param>
        /// <returns>Task<SqlDataReader></returns>
        //public Task<SqlDataReader> ExecuteReaderAsync(string sql, CommandType type = CommandType.Text, params SqlParameter[] param)
        //{

        //    SqlCommand cmd = new SqlCommand(sql, Conn);
        //    if (param != null && param.Length > 0)
        //    {
        //        cmd.Parameters.AddRange(param);
        //    }
        //    cmd.CommandType = type;

        //    return cmd.ExecuteReaderAsync();

        //}
        #endregion

        #region 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="type">执行解析类型 默认 sql</param>
        /// <param name="param">参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string sql, CommandType type = CommandType.Text, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(sql, Conn);
            try
            {
                if (param != null && param.Length > 0)
                {
                    cmd.Parameters.AddRange(param);
                }
                cmd.CommandType = type;

                return cmd.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 查询(断开式连接)
        /// <summary>
        /// 查询(断开式连接)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="type">执行解析类型 默认 sql</param>
        /// <param name="param">参数</param>
        /// <returns>DataTable</returns>
        public DataTable GetTable(string sql, CommandType type = CommandType.Text, params SqlParameter[] param)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(sql, Conn);
                if (param != null && param.Length > 0)
                {
                    sda.SelectCommand.Parameters.AddRange(param);
                }
                sda.SelectCommand.CommandType = type;
                sda.Fill(ds);//填充数据到ds
                return ds.Tables[0];
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion
    }
}
