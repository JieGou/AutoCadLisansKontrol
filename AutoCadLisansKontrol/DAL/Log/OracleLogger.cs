using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using ExceptionLib;
using CheckLicense.Log;
using ProtectionConnLib_4.DataAccessModel;

namespace CheckLicense.OracleLogger
{
    public class OracleLogger : ILog
    {

        /// <summary>
        /// Tamamlanan her data aktarım işleminin hepsine birden verilen isim process.
        /// </summary>
        /// <param name="log"></param>
        public void InitiliazeProcess(LogData log)//string appName, string id, DateTime startTime, string ip, string host, string method)
        {
            using (var dbaccess = DatabaseFactory.CreateDbAccess(DatabaseType.Oracle, "EXCEPTIONUSR", AccessEnvironment.Product))
            {
                dbaccess.ExecuteNonQuery("EXCEPTIONUSR.EXP_PKG.PROC_INITILIAZE_ORTEC_LOG", new List<IDataParameter>
                {
                    new OracleParameter("P_ID",log.Id),
                    new OracleParameter("P_STARTTIME",log.StartTime),
                    new OracleParameter("P_METHOD",log.Method),
                    new OracleParameter("P_SOURCE_IP",log.Ip),
                    new OracleParameter("P_HOST",log.Host),
                    new OracleParameter("P_APPNAME",log.AppName)
                });
            }
        }

        public void UpdateProcess(LogData log)//string id, int statu,DateTime endDate)
        {
            using (var dbaccess = DatabaseFactory.CreateDbAccess(DatabaseType.Oracle, "EXCEPTIONUSR", AccessEnvironment.Product))
            {
                dbaccess.ExecuteNonQuery("EXCEPTIONUSR.EXP_PKG.PROC_UPDATE_ORTEC_LOG", new List<IDataParameter>
                {
                    new OracleParameter("P_ID", log.Id),
                    new OracleParameter("P_STATU", log.State),
                    new OracleParameter("P_ENDTIME", log.EndTime),
                });
            }
        }
        public void UpdateItemOfProcess(LogData log)//string id, string response,int statu,int levelid,DateTime endDate)
        {
            //var buffer = EncodeToByte(log.ResXml.ReplaceEncodedHtmlCharacter());
            //var connection = new System.Data.OracleClient.OracleConnection(new OracleDbAccess("EXCEPTIONUSR",AccessEnvironment.Product).ConnectionString);
            //connection.Close();
            //connection.Open();
            //System.Data.OracleClient.OracleTransaction transaction = connection.BeginTransaction();
            //System.Data.OracleClient.OracleCommand command = connection.CreateCommand();
            //command.Transaction = transaction;
            //command.CommandText = "declare xx blob; begin dbms_lob.createtemporary(xx, false, 0); :tempblob := xx; end;";
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("tempblob", OracleType.Blob)).Direction = ParameterDirection.Output;
            //command.ExecuteNonQuery();
            //OracleLob tempLob = (OracleLob)command.Parameters[0].Value;
            //tempLob.BeginBatch(OracleLobOpenMode.ReadWrite);
            //tempLob.Write(buffer, 0, buffer.Length);
            //tempLob.EndBatch();
            //command.Parameters.Clear();
            //command.CommandText = "EXCEPTIONUSR.EXP_PKG.PROC_UPDATE_ORTEC_LOG_DT";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_ID", log.Id));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_RESPONSE", OracleType.Clob)).Value = tempLob;
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_STATU", log.Statu));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_LEVELID", log.LevelId));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_ENDTIME", log.EndTime));
            ////command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_MESSAGE", log.Message));
            //command.ExecuteNonQuery();
            //transaction.Commit();
            //connection.Close();
        }
        /// <summary>
        /// Açılmış olan bir işlemin herbir adımına verilen isim ItemOfProcess
        /// </summary> 
        public decimal InitiliazeItemOfProcess(LogData log)//string request, string id,string method,DateTime startTime)
        {

            //var buffer = EncodeToByte(log.ReqXml.ReplaceEncodedHtmlCharacter());
            //var connection = new System.Data.OracleClient.OracleConnection(new OracleDbAccess("EXCEPTIONUSR", AccessEnvironment.Product).ConnectionString);
            //connection.Close();
            //connection.Open();
            //System.Data.OracleClient.OracleTransaction transaction = connection.BeginTransaction();
            //System.Data.OracleClient.OracleCommand command = connection.CreateCommand();
            //command.Transaction = transaction;
            //command.CommandText = "declare xx blob; begin dbms_lob.createtemporary(xx, false, 0); :tempblob := xx; end;";
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("tempblob", OracleType.Blob)).Direction = ParameterDirection.Output;
            //command.ExecuteNonQuery();
            //OracleLob tempLob = (OracleLob)command.Parameters[0].Value;
            //tempLob.BeginBatch(OracleLobOpenMode.ReadWrite);
            //tempLob.Write(buffer, 0, buffer.Length);
            //tempLob.EndBatch();

            //command.Parameters.Clear();
            //command.CommandText = "EXCEPTIONUSR.EXP_PKG.PROC_INITILIAZE_ORTEC_LOG_DT";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_ID", log.Id));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_METHOD", log.Method));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_STARTTIME", log.StartTime));
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_REQUEST", OracleType.Clob)).Value = tempLob;
            //command.Parameters.Add(new System.Data.OracleClient.OracleParameter("P_LEVELID", OracleType.Int32)).Direction = ParameterDirection.Output;
            //command.ExecuteNonQuery();
            //transaction.Commit();
            //connection.Close();
            //var levelid = (int)command.Parameters["P_LEVELID"].Value;

            return 0;
        }

    }

}