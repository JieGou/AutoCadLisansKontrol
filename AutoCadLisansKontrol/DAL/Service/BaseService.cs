using CheckLicense.Log;
using LicenseControllerWs.DAL;
using ProtectionConnLib_4.DataAccessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL.Service
{
    public class BaseService
    {
        protected AUTOCADLICENSEEntities1 dbaccess = new AUTOCADLICENSEEntities1();
        protected MssqlDbAccess mssqlappaccess = new MssqlDbAccess(System.Configuration.ConfigurationManager.ConnectionStrings["AutocadLicenseDatabase"].ConnectionString);
        public UsersDTO Login(string username, string password)
        {
            var user = dbaccess.Users.Where(x => x.UserName == username && x.Password == password).FirstOrDefault();
            return Converter.Convert<UsersDTO, Users>(user);
        }
        public List<ControlPointDTO> GetControlPoint()
        {
            try
            {
                var controlpoint = dbaccess.ControlPoint.ToList();
                return Converter.Convert<ControlPointDTO, ControlPoint>(controlpoint);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SoftwareDTO> GetSoftware()
        {

            var softlist = dbaccess.Software.ToList();
            return Converter.Convert<SoftwareDTO, SoftwareEntity>(softlist);
        }
        public List<FE_ControlListDTO> GetFEControlList(int appid)
        {
            var controllist = dbaccess.FE_ControlList.Where(x => x.AppId == appid).ToList();
            return Converter.Convert<FE_ControlListDTO, FE_ControlListEntity>(controllist);
        }
        public void LogToDb(List<LogData> items)
        {
            ILog logger = LoggerFactory.CreateLogger(DatabaseType.MsSql);
            foreach (var item in items)
            {

                switch (item.LogDataType)
                {
                    case LogDataType.InitiliazeProcess:
                        logger.InitiliazeProcess(item);
                        break;

                    case LogDataType.InitiliazeItemOfProcess:

                        logger.InitiliazeItemOfProcess(item);

                        break;

                    case LogDataType.UpdateItemOfProcess:

                        logger.UpdateItemOfProcess(item);

                        break;

                    case LogDataType.UpdateProcess:

                        logger.UpdateProcess(item);

                        break;
                    default:
                        break;
                }
            }

        }
    }
}
