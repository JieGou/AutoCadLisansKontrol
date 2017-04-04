using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtectionConnLib_4.DataAccessModel;

namespace CheckLicense.Log
{
    public class LoggerFactory
    {
        public static ILog CreateLogger(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new MssqlLogger.MssqlLogger();
                case DatabaseType.Oracle:
                    return new OracleLogger.OracleLogger();
            }
            return null;
        }
    }
}