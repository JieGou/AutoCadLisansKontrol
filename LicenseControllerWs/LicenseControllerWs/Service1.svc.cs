using AutoCadLisansKontrol.DAL;
using CheckLicense.Log;
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
        DataAccess dbaccess = new DataAccess();
        readonly ILog _logger = LoggerFactory.CreateLogger(DatabaseType.MsSql);
        public void UpsertFirm(Firm firm)
        {
            dbaccess.UpsertFirm(new FirmEntity { Address = firm.Address, Contact = firm.Contact, Id = firm.Id, InsertDate = firm.InsertDate, Name = firm.Name, PhoneNo = firm.PhoneNo });
        }

        public Firm GetFirm(int? firmId)
        {
            var firm = dbaccess.GetFirm(firmId);
            return new Firm { Address = firm.Address, Contact = firm.Contact, Id = firm.Id, InsertDate = firm.InsertDate, Name = firm.Name, PhoneNo = firm.PhoneNo };
        }

        public List<Firm> ListFirm()
        {
            return dbaccess.ListFirm().ConvertAll(x => new Firm() { Address = x.Address, Contact = x.Contact, Id = x.Id, InsertDate = x.InsertDate, Name = x.Name, PhoneNo = x.PhoneNo });
        }
        public void DeleteFirm(int firmid)
        {
            dbaccess.DeleteFirm(firmid);
        }
        public void UpdateFirm(Firm firm)
        {
            dbaccess.UpdateFirm((new FirmEntity { Address = firm.Address, Contact = firm.Contact, Id = firm.Id, InsertDate = firm.InsertDate, Name = firm.Name, PhoneNo = firm.PhoneNo }));
        }
        public int UpsertComputer(Computer comp)
        {
            return dbaccess.UpsertComputer(new ComputerEntity { FirmId = comp.FirmId, Id = comp.Id, Name = comp.Name, InsertDate = comp.InsertDate, Ip = comp.Ip, IsComputer = comp.IsComputer, IsRootMachine = comp.IsRootMachine, IsVisible = comp.IsVisible, PyshicalAddress = comp.PyshicalAddress, Type = comp.Type });
        }
        public Operation GetOperation(int opr)
        {
            var operation = dbaccess.GetOperation(opr);
            return new Operation { Name = operation.Name, FirmId = operation.FirmId, Id = operation.Id };
        }
        public List<Computer> ListAllComputer()
        {
            return dbaccess.ListComputer().ConvertAll(x => new Computer { Id = x.Id, FirmId = x.FirmId, Name = x.Name, InsertDate = x.InsertDate, Ip = x.Ip, IsComputer = x.IsComputer, IsRootMachine = x.IsRootMachine, IsVisible = x.IsVisible, PyshicalAddress = x.PyshicalAddress, Type = x.Type });
        }
        public List<Computer> ListComputer(int? firmId)
        {
            return dbaccess.ListComputer(firmId).ConvertAll(x => new Computer { Id = x.Id, FirmId = x.FirmId, Name = x.Name, InsertDate = x.InsertDate, Ip = x.Ip, IsComputer = x.IsComputer, IsRootMachine = x.IsRootMachine, IsVisible = x.IsVisible, PyshicalAddress = x.PyshicalAddress, Type = x.Type });
        }
        public void DeleteComputer(int Id)
        {
            dbaccess.DeleteComputer(Id);
        }
        public void UpdateComputer(Computer comp)
        {
            dbaccess.UpdateComputer(new ComputerEntity { FirmId = comp.FirmId, Id = comp.Id, Name = comp.Name, InsertDate = comp.InsertDate, Ip = comp.Ip, IsComputer = comp.IsComputer, IsRootMachine = comp.IsRootMachine, IsVisible = comp.IsVisible, PyshicalAddress = comp.PyshicalAddress, Type = comp.Type });
        }
        public void UpsertOperation(Operation opr)
        {
            dbaccess.UpsertOperation(new OperationEntity { FirmId = opr.FirmId, Id = opr.Id, Name = opr.Name });
        }

        public List<Operation> ListAllOperation()
        {
            return dbaccess.ListOperation().ConvertAll(x => new Operation { FirmId = x.FirmId, Id = x.Id, Name = x.Name });
        }
        public List<Operation> ListOperation(int firmid)
        {
            return dbaccess.ListOperation(firmid).ConvertAll(x => new Operation { FirmId = x.FirmId, Id = x.Id, Name = x.Name, ComputerCount = ListComputer(firmid).Count });
        }

        public void DeleteOperation(Operation opr)
        {
            dbaccess.DeleteOperation(new OperationEntity { Name = opr.Name, Id = opr.Id, FirmId = opr.FirmId });
        }

        public void UpdateOperation(Operation opr)
        {
            dbaccess.UpdateOperation(new OperationEntity { Name = opr.Name, Id = opr.Id, FirmId = opr.FirmId });
        }


        public void UpsertCheckLicense(CheckLicense chck)
        {
            dbaccess.UpsertCheckLicense(new CheckLicenseEntity { Id = chck.Id, CheckDate = chck.CheckDate, ComputerId = chck.ComputerId, OperationId = chck.OperationId, Output = chck.Output, UpdateDate = chck.UpdateDate, FirmId = chck.FirmId, Name = chck.Name, Ip = chck.Ip, State = chck.State, InstallDate = chck.InstallDate, Installed = chck.Installed, Uninstalled = chck.Uninstalled, UnInstallDate = chck.UnInstallDate, IsFound = chck.IsFound, AppId = chck.AppId });
        }
        public void LogToDb(List<LogData> logs)
        {
            dbaccess.LogToDb(logs);
        }
        public List<CheckLicense> ListCheckLicense(int id)
        {
            return dbaccess.ListCheckLicense(id).ConvertAll(chck => new CheckLicense { Id = chck.Id, CheckDate = chck.CheckDate, ComputerId = chck.ComputerId, OperationId = chck.OperationId, Output = chck.Output, UpdateDate = chck.UpdateDate, FirmId = chck.FirmId, Name = chck.Name, Ip = chck.Ip, State = chck.State, InstallDate = chck.InstallDate, Installed = chck.Installed, Uninstalled = chck.Uninstalled, UnInstallDate = chck.UnInstallDate, IsFound = chck.IsFound, AppId = chck.AppId });
        }
        public void DeleteCheckLicense(CheckLicense chck)
        {
            dbaccess.DeleteCheckLicense(new CheckLicenseEntity { Id = chck.Id, CheckDate = chck.CheckDate, ComputerId = chck.ComputerId, OperationId = chck.OperationId, Output = chck.Output, UpdateDate = chck.UpdateDate, FirmId = chck.FirmId, Name = chck.Name, Ip = chck.Ip, State = chck.State, InstallDate = chck.InstallDate, Installed = chck.Installed, Uninstalled = chck.Uninstalled, UnInstallDate = chck.UnInstallDate, IsFound = chck.IsFound, AppId = chck.AppId });
        }

        public void UpdateCheckLicense(CheckLicense chck)
        {
            dbaccess.UpdateCheckLicense(new CheckLicenseEntity { Id = chck.Id, CheckDate = chck.CheckDate, ComputerId = chck.ComputerId, OperationId = chck.OperationId, Output = chck.Output, UpdateDate = chck.UpdateDate, FirmId = chck.FirmId, Name = chck.Name, Ip = chck.Ip, State = chck.State, InstallDate = chck.InstallDate, Installed = chck.Installed, Uninstalled = chck.Uninstalled, UnInstallDate = chck.UnInstallDate, IsFound = chck.IsFound, AppId = chck.AppId });
        }

        public void DeleteAllComputerBaseFormid(int firmId)
        {
            dbaccess.DeleteAllComputerBaseFormid(firmId);
        }

        public void DeleteAllLicenseBaseOperationid(int oprId)
        {
            dbaccess.DeleteAllLicenseBaseOperationid(oprId);
        }
        public List<ControlPoint> GetControlPoint()
        {
            return dbaccess.GetControlPoint();
        }

        public List<Software> GetAllApplication()
        {
            var apps = dbaccess.GetSoftware().ConvertAll(x => new Software { AppName = x.AppName, Id = x.Id });

            return apps;
        }
        public Software GetApplication(Nullable<int> id)
        {
            var apps = dbaccess.GetSoftware().Where(x => x.Id == id).Select(x => new Software { AppName = x.AppName, Id = x.Id }).FirstOrDefault();

            return apps;
        }
        public List<FE_ControlList> GetFEControlList(int appid)
        {

            return dbaccess.GetFEControlList(appid).ConvertAll(x => new FE_ControlList { Id = x.Id, AppId = x.AppId, Drive = x.Drive, Path = x.Path, Type = x.Type });
        }
    }
}
