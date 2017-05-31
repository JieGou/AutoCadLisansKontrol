using AutoCadLisansKontrol.DAL;
using CheckLicense.Log;
using LicenseControllerWs.DAL;
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
        UsersDTO Login(string username, string password);
        [OperationContract]
        void FirmUpsert(FirmDTO firm);
        [OperationContract]
        FirmDTO FirmGet(int? firmId);
        [OperationContract]
        List<FirmDTO> FirmList(int userid);
        [OperationContract]
        void FirmDelete(int firmid);
        [OperationContract]
        void FirmUpdate(FirmDTO firm);
        [OperationContract]
        string ComputersUpsert(List<ComputerDTO> c);
        [OperationContract]
        int ComputerUpsert(ComputerDTO c);
        [OperationContract]
        OperationDTO OperationGet(int opr);
        [OperationContract]
        List<ComputerDTO> ComputerListAll();
        [OperationContract]
        List<ComputerDTO> ComputerList(int? firmId);
        [OperationContract]
        void ComputerDelete(int Id);
        [OperationContract]
        void ComputerUpdate(ComputerDTO comp);
        [OperationContract]
        void OperationUpsert(OperationDTO opr);
        [OperationContract]
        List<OperationDTO> OperationListAll();
        [OperationContract]
        List<OperationDTO> OperationList(int firmid);
        [OperationContract]
        void OperationDelete(OperationDTO opr);
        [OperationContract]
        void OperationUpdate(OperationDTO opr);
        [OperationContract]
        void CheckLicenseUpsert(CheckLicenseDTO oprdetail);
        [OperationContract]
        void LogToDb(List<LogData> logs);
        [OperationContract]
        List<CheckLicenseDTO> CheckLicenseList(int id);

        [OperationContract]
        void CheckLicenseDelete(CheckLicenseDTO oprdetail);
        [OperationContract]
        void CheckLicenseUpdate(CheckLicenseDTO oprdetail);
        [OperationContract]
        void ComputerDeleteAllBaseFormid(int firmId);
        [OperationContract]
        void CheckLicenseDeleteAllBaseOperationid(int oprId);
        [OperationContract]
        List<ControlPointDTO> GetControlPoint();
        [OperationContract]
        List<SoftwareDTO> GetAllApplication();
        [OperationContract]
        List<FE_ControlListDTO> GetFEControlList(int appid);
        [OperationContract]
        SoftwareDTO GetApplication(Nullable<int> id);
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
