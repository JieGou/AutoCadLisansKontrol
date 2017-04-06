using AutoCadLisansKontrol.DAL;
using CheckLicense.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LicenseControllerWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        bool Login(string username, string password);
        [OperationContract]
        void UpsertFirm(Firm firm);
        [OperationContract]
        Firm GetFirm(int? firmId);
        [OperationContract]
        List<Firm> ListFirm();
        [OperationContract]
        void DeleteFirm(int firmid);
        [OperationContract]
        void UpdateFirm(Firm firm);
        [OperationContract]
        int UpsertComputer(Computer c);
        [OperationContract]
        Operation GetOperation(int opr);
        [OperationContract]
        List<Computer> ListAllComputer();
        [OperationContract]
        List<Computer> ListComputer(int? firmId);
        [OperationContract]
        void DeleteComputer(int Id);
        [OperationContract]
        void UpdateComputer(Computer comp);
        [OperationContract]
        void UpsertOperation(Operation opr);
        [OperationContract]
        List<Operation> ListAllOperation();
        [OperationContract]
        List<Operation> ListOperation(int firmid);
        [OperationContract]
        void DeleteOperation(Operation opr);
        [OperationContract]
        void UpdateOperation(Operation opr);
        [OperationContract]
        void UpsertCheckLicense(CheckLicense oprdetail);
        [OperationContract]
        void LogToDb(List<LogData> logs);
        [OperationContract]
        List<CheckLicense> ListCheckLicense(int id);

        [OperationContract]
        void DeleteCheckLicense(CheckLicense oprdetail);
        [OperationContract]
        void UpdateCheckLicense(CheckLicense oprdetail);
        [OperationContract]
        void DeleteAllComputerBaseFormid(int firmId);
        [OperationContract]
        void DeleteAllLicenseBaseOperationid(int oprId);
        [OperationContract]
        List<ControlPoint> GetControlPoint();
        [OperationContract]
        List<Software> GetAllApplication();
        [OperationContract]
        List<FE_ControlList> GetFEControlList(int appid);
        [OperationContract]
        Software GetApplication(Nullable<int> id);
        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
