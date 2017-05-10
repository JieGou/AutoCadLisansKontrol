using AutoCadLisansKontrol.DAL;
using AutoCadLisansKontrol.DAL.Service;
using CheckLicense.Log;
using LicenseControllerWs.DAL;
using LicenseControllerWs.ParameterInspector;
using ProtectionConnLib_4.DataAccessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace LicenseControllerWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        BaseService baseservice = new BaseService();
        CheckLicenseService checklicenseservice = new CheckLicenseService();
        ComputerService computerService = new ComputerService();
        FirmService firmService = new FirmService();
        OperationService operationService = new OperationService();


        readonly ILog _logger = LoggerFactory.CreateLogger(DatabaseType.MsSql);

        public UsersDTO Login(string username, string password)
        {
            return baseservice.Login(username, password);
        }
        public void FirmUpsert(FirmDTO firm)
        {
            firmService.Upsert(firm);
        }

        public FirmDTO FirmGet(int? firmId)
        {
            return firmService.Get(firmId);
        }

        public List<FirmDTO> FirmList(int userid)
        {
            var firm = new List<FirmDTO>();
            try
            {
                firm = firmService.List(userid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return firm;
        }
        public void FirmDelete(int firmid)
        {
            firmService.Delete(firmid);
        }
        public void FirmUpdate(FirmDTO firm)
        {
            firmService.Update(firm);
        }
        public int ComputerUpsert(ComputerDTO comp)
        {
            return computerService.Upsert(comp);
        }
        public OperationDTO OperationGet(int opr)
        {
            var operation = operationService.Get(opr);
            return operation;
        }
        public List<ComputerDTO> ComputerListAll()
        {
            return computerService.List();
        }
        public List<ComputerDTO> ComputerList(int? firmId)
        {
            return computerService.List(firmId);
        }
        public void ComputerDelete(int Id)
        {
            computerService.Delete(Id);
        }
        public void ComputerUpdate(ComputerDTO comp)
        {
            computerService.Update(comp);
        }
        public void OperationUpsert(OperationDTO opr)
        {
            operationService.Upsert(opr);
        }

        public List<OperationDTO> OperationListAll()
        {
            return operationService.List();
        }
        public List<OperationDTO> OperationList(int firmid)
        {
            return operationService.List(firmid);
        }

        public void OperationDelete(OperationDTO opr)
        {
            operationService.Delete(opr);
        }

        public void OperationUpdate(OperationDTO opr)
        {
            operationService.Update(opr);
        }


        public void CheckLicenseUpsert(CheckLicenseDTO chck)
        {
            checklicenseservice.Upsert(chck);
        }

        public List<CheckLicenseDTO> CheckLicenseList(int id)
        {
            return checklicenseservice.List(id);
        }
        public void CheckLicenseDelete(CheckLicenseDTO chck)
        {
            checklicenseservice.Delete(chck);
        }

        public void CheckLicenseUpdate(CheckLicenseDTO chck)
        {
            checklicenseservice.Update(chck);
        }

        public void ComputerDeleteAllBaseFormid(int firmId)
        {
            computerService.DeleteAllBaseFormid(firmId);
        }

        public void CheckLicenseDeleteAllBaseOperationid(int oprId)
        {
            checklicenseservice.DeleteAllBaseOperationid(oprId);
        }
        public List<ControlPointDTO> GetControlPoint()
        {
            return baseservice.GetControlPoint();
        }

        public List<SoftwareDTO> GetAllApplication()
        {
            var apps = baseservice.GetSoftware().Where(x => x.IsEnable == true).ToList();

            return apps;
        }
        public SoftwareDTO GetApplication(Nullable<int> id)
        {
            var apps = baseservice.GetSoftware().Where(x => x.Id == id).FirstOrDefault();

            return apps;
        }
        public List<FE_ControlListDTO> GetFEControlList(int appid)
        {

            return baseservice.GetFEControlList(appid);
        }
        public void LogToDb(List<LogData> logs)
        {
            baseservice.LogToDb(logs);
        }
    }
}
