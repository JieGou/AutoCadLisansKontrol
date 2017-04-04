using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ExceptionLib;
using CheckLicense.Log;
using CheckLicense.Model;
using ProtectionConnLib_4.DataAccessModel;
using AutoCadLisansKontrol.DAL;

namespace CheckLicense.MssqlLogger
{
    public class MssqlLogger : ILog
    {
        string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["CheckLicenseDbLog"].ConnectionString;
        /// <summary>
        /// Tamamlanan her data aktarım işleminin hepsine birden verilen isim process.
        /// </summary>
        /// <param name="log"></param>
        public void InitiliazeProcess(LogData log)//string appName, string id, DateTime startTime, string ip, string host, string method)
        {
            using (var dbaccess = new MssqlDbAccess(connstring))
            {
                dbaccess.ExecuteNonQuery("DBO.PROC_INITILIAZE_ORTEC_LOG", new List<SqlParameter>
                {
                    new SqlParameter("@P_ID",log.Id),
                    new SqlParameter("@P_STARTTIME",log.StartTime),
                    new SqlParameter("@P_METHOD",log.Method),
                    new SqlParameter("@P_APPNAME",log.AppName)
                });
            }
        }

        public void UpdateProcess(LogData log)//string id, int statu,DateTime endDate)
        {
            using (var dbaccess = new MssqlDbAccess(connstring))
            {
                dbaccess.ExecuteNonQuery("DBO.PROC_UPDATE_ORTEC_LOG", new List<SqlParameter>
                {
                    new SqlParameter("@P_ID", log.Id),
                    new SqlParameter("@P_STATU", log.State),
                    new SqlParameter("@P_ENDTIME", log.EndTime)
                });
            }
        }
        public void UpdateItemOfProcess(LogData log)//string id, string response,int statu,int levelid,DateTime endDate)
        {

            using (var dbaccess = new MssqlDbAccess(connstring))
            {
                dbaccess.ExecuteNonQuery("DBO.PROC_UPDATE_ORTEC_LOG_DT", new List<SqlParameter>
                {
                    new SqlParameter("@P_ID", log.Id),
                    new SqlParameter("@P_RESPONSE", log.ResXml.ReplaceEncodedHtmlCharacter()),
                    new SqlParameter("@P_STATU", log.State),
                    new SqlParameter("@P_LEVELID", log.LevelId),
                    new SqlParameter("@P_ENDTIME", log.EndTime)
                });
            }
        }
        /// <summary>
        /// Açılmış olan bir işlemin herbir adımına verilen isim ItemOfProcess
        /// </summary> 
        public decimal InitiliazeItemOfProcess(LogData log)//string request, string id,string method,DateTime startTime)
        {
            using (var dbaccess = new MssqlDbAccess(connstring))
            {
                var outputparam = new SqlParameter("@P_LEVELID", log.LevelId);
                outputparam.Direction = ParameterDirection.Output;
                dbaccess.ExecuteNonQuery("DBO.PROC_INITILIAZE_ORTEC_LOG_DT", new List<SqlParameter>
                {
                    new SqlParameter("@P_ID", new Guid(log.Id)),
                    new SqlParameter("@P_METHOD", log.Method),
                    new SqlParameter("@P_STARTTIME", log.StartTime),
                    new SqlParameter("@P_REQUEST", log.ReqXml.ReplaceEncodedHtmlCharacter()),
                    outputparam
                });
                return Convert.ToInt32(outputparam.Value);
            }

        }

    }

}