using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckLicense.Log
{
    public interface ILog
    {
        decimal InitiliazeItemOfProcess(LogData mssqlLogger);
        void UpdateItemOfProcess(LogData mssqlLogger);
        void InitiliazeProcess(LogData logData);
        void UpdateProcess(LogData logData);
    }
}
