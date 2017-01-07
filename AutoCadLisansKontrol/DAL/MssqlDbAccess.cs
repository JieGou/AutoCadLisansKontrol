
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace AutoCadLisansKontrol.DAL
{
    public class MssqlDbAccess : IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static SqlConnection _conn = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _lock = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ConnectionString
        {
            get
            {
                lock (_lock)
                {
                    return (@"data source=195.87.11.40;initial catalog=AUTOCADLICENSE;persist security info=True;user id=sa;password=BIMED2000;MultipleActiveResultSets=True;App=EntityFramework");
                }
            }
        }


        public MssqlDbAccess()
        {
        }

        private SqlConnection CheckConnection()
        {
            lock (_lock)
            {
             
                _conn = new SqlConnection(ConnectionString);
                _conn.Open();
                return _conn;
            }
        }

        private void CloseConnection(SqlConnection conn)
        {
            conn.Close();
        }

        private SqlCommand CreateCommand(SqlConnection conn)
        {
            return conn.CreateCommand();
        }

        public List<T> QueryList<T>(string procName, List<SqlParameter> parameter) where T : new()
        {
            lock (_lock)
            {
                using (var conn = CheckConnection())
                {

                    var comm = CreateCommand(conn);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = string.Format(procName);
                    comm.CommandTimeout = 1000000;
                    foreach (var o in parameter)
                    {
                        comm.Parameters.Add(o);
                    }
                    var list = MapData<T>(comm.ExecuteReader());
                    CloseConnection(conn);
                    return list;
                }
            }
        }
        public T QueryItem<T>(string procName, List<SqlParameter> parameter) where T : new()
        {
            lock (_lock)
            {
                var item = new T();
                using (var conn = CheckConnection())
                {
                    var comm = CreateCommand(conn);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = string.Format(procName);
                    foreach (var o in parameter)
                    {
                        comm.Parameters.AddWithValue(o.ParameterName, o.Value);
                    }
                    var list = MapData<T>(comm.ExecuteReader());
                    if (list.Count > 0) item = list[0];
                    CloseConnection(conn);
                    return item;
                }
            }
        }
        public void ExecuteNonQuery(string procName, List<SqlParameter> parameters)
        {
            lock (_lock)
            {
                using (var conn = CheckConnection())
                {
                    var comm = CreateCommand(conn);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = string.Format(procName);
                    foreach (var o in parameters)
                    {
                        comm.Parameters.AddWithValue(o.ParameterName, o.Value);
                    }
                    comm.ExecuteNonQuery();
                    CloseConnection(conn);
                }
            }
        }
        public object ExecuteScalar(string procName, List<SqlParameter> parameters)
        {
            lock (_lock)
            {
                using (var conn = CheckConnection())
                {
                    var comm = CreateCommand(conn);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = string.Format(procName);
                    var parm = new SqlParameter()
                    {
                        Direction = ParameterDirection.ReturnValue,
                        SqlDbType = SqlDbType.Int
                    };
                    comm.Parameters.Add(parm);
                    foreach (var o in parameters)
                    {
                        comm.Parameters.AddWithValue(o.ParameterName, o.Value);
                    }
                    comm.ExecuteScalar();
                    CloseConnection(conn);
                    return comm.Parameters[0].Value.ToString();
                }
            }
        }

        private static List<T> MapData<T>(IDataReader dr) where T : new()
        {
            var pocoType = typeof(T);
            var entitys = new List<T>();
            var hashtable = new Hashtable();
            var properties = pocoType.GetProperties();
            foreach (var info in properties)
            {
                hashtable[info.Name.ToUpper()] = info;
            }
            while (dr.Read())
            {
                var newObject = new T();
                for (var index = 0; index < dr.FieldCount; index++)
                {
                    var name = dr.GetName(index).ToUpper();
                    var info = (PropertyInfo)hashtable[name];
                    if ((info == null) || !info.CanWrite) continue;
                    var value = dr.GetValue(index);
                    if (dr.GetValue(index) == null || dr.GetValue(index) == DBNull.Value) continue;
                    if (dr.GetValue(index) is long)
                    {
                        value = Convert.ToInt32(dr.GetValue(index));
                    }
                    info.SetValue(newObject, value, null);
                }
                entitys.Add(newObject);
            }
            dr.Close();
            return entitys;
        }
        public DataSet DataSetQuery(string procName, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            lock (_lock)
            {
                using (_conn = CheckConnection())
                {
                    var comm = CreateCommand(_conn);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = string.Format(procName);


                    foreach (var o in parameters)
                    {
                        comm.Parameters.Add(o);
                    }
                    comm.Transaction = _conn.BeginTransaction();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = comm;

                    da.Fill(ds);
                    comm.Transaction.Commit();
                    CloseConnection(_conn);
                }
            }
            return ds;
        }
        public void Dispose()
        {
            _conn.Close();
        }
    }


}
