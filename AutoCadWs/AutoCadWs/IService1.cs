using AutoCadLisansKontrol.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AutoCadWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
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
        void UpsertComputer(Computer c);
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
        List<CheckLicense> ListCheckLicense();

        [OperationContract]
        void DeleteCheckLicense(CheckLicense oprdetail);
        [OperationContract]
        void UpdateCheckLicense(CheckLicense oprdetail);
        [OperationContract]
        void DeleteAllComputerBaseFormid(int firmId);



        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "AutoCadWs.ContractType".
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
