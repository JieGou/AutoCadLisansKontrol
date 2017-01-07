using AutoCadLisansKontrol.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AutoCadWs
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        DataAccess dbaccess = new DataAccess();
        public void UpsertFirm(Firm firm)
        {
            dbaccess.UpsertFirm(firm);
        }

        public Firm GetFirm(int? firmId)
        {
            return dbaccess.GetFirm(firmId);
        }

        public List<Firm> ListFirm()
        {
            return dbaccess.ListFirm();
        }
        public void DeleteFirm(int firmid)
        {
            dbaccess.DeleteFirm(firmid);
        }
        public void UpdateFirm(Firm firm)
        {
            dbaccess.UpdateFirm(firm);
        }
        public void UpsertComputer(Computer c)
        {
            dbaccess.UpsertComputer(c);
        }
        public Operation GetOperation(int opr)
        {
            return dbaccess.GetOperation(opr);
        }
        public List<Computer> ListAllComputer()
        {
            return dbaccess.ListComputer();
        }
        public List<Computer> ListComputer(int? firmId)
        {
            return dbaccess.ListComputer(firmId);
        }
        public void DeleteComputer(string Ip)
        {
            dbaccess.DeleteComputer(Ip);
        }
        public void UpdateComputer(Computer comp)
        {
            dbaccess.UpdateComputer(comp);
        }
        public void UpsertOperation(Operation opr)
        {
            dbaccess.UpsertOperation(opr);
        }

        public List<Operation> ListAllOperation()
        {
            return dbaccess.ListOperation();
        }
        public List<Operation> ListOperation(int firmid)
        {
            return dbaccess.ListOperation(firmid);
        }

        public void DeleteOperation(Operation opr)
        {
            dbaccess.DeleteOperation(opr);
        }

        public void UpdateOperation(Operation opr)
        {
            dbaccess.UpdateOperation(opr);
        }


        public void UpsertCheckLicense(CheckLicense oprdetail)
        {
            dbaccess.UpsertCheckLicense(oprdetail);
        }

        public List<CheckLicense> ListCheckLicense()
        {

            return dbaccess.ListCheckLicense();
        }
        public void DeleteCheckLicense(CheckLicense oprdetail)
        {
            dbaccess.DeleteCheckLicense(oprdetail);

        }

        public void UpdateCheckLicense(CheckLicense oprdetail)
        {
            dbaccess.UpdateCheckLicense(oprdetail);
        }

        public void DeleteAllComputerBaseFormid(int firmId)
        {

            dbaccess.DeleteAllComputerBaseFormid(firmId);
        }
        }

}
