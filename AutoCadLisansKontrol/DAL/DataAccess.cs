using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL
{

    public class DataAccess
    {
        AUTOCADLICENSEEntities1 dbaccess = new AUTOCADLICENSEEntities1();

        public void UpsertFirm(Firm firm)
        {
            var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<Firm>();

            if (item == null)
                dbaccess.Firm.Add(firm);
            else
                item.Name = firm.Name;

            dbaccess.SaveChanges();
        }
        public Firm GetFirm(int? firmId) {
            var item = dbaccess.Firm.Where(x => x.Id == firmId).FirstOrDefault<Firm>();
            return item;
        }
        public List<Firm> ListFirm()
        {
            return dbaccess.Firm.ToList();
        }

        public void DeleteFirm(Firm firm)
        {
            dbaccess.Firm.Remove(firm);
            dbaccess.SaveChanges();
        }

        public void UpdateFirm(Firm firm)
        {
            var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<Firm>();
            item.Name = firm.Name;
            dbaccess.SaveChanges();
        }

        public void UpsertComputer(Computer comp)
        {
            var item = dbaccess.Computer.Where(x => x.Id == comp.Id).FirstOrDefault<Computer>();
            if (item == null)
            {
                item = new Computer { Id = comp.Id, Ip = comp.Ip, IsComputer = comp.IsComputer, IsRootMachine = comp.IsRootMachine, IsVisible = comp.IsVisible, Name = comp.Name, PyshicalAddress = comp.PyshicalAddress, FirmId = comp.FirmId, Type = comp.Type, InsertDate = comp.InsertDate } ;
                dbaccess.Computer.Add(item);
            }
            else
            {
                item.Ip = comp.Ip;
                item.IsComputer = comp.IsComputer;
                item.IsRootMachine = comp.IsRootMachine;
                item.Name = comp.Name;
                item.PyshicalAddress = comp.PyshicalAddress;
                item.IsVisible = comp.IsVisible;
            }
            dbaccess.SaveChanges();
        }

        public Operation GetOperation(int opr)
        {
            var item = dbaccess.Operation.Where(x => x.Id == opr).FirstOrDefault<Operation>();
            return item;
        }

        public List<Computer> ListComputer()
        {
            return dbaccess.Computer.ToList();
        }
        public List<Computer> ListComputer(int? firmId)
        {
            return dbaccess.Computer.Where(x=>x.FirmId==firmId).ToList();
        }
        public void DeleteComputer(Computer comp)
        {
            dbaccess.Computer.Remove(comp);
            dbaccess.SaveChanges();
        }
        public void UpdateComputer(Computer comp)
        {
            var item = dbaccess.Computer.Where(x => x.Id == comp.Id).FirstOrDefault<Computer>();
            item.Ip = comp.Ip;
            item.IsComputer = comp.IsComputer;
            item.IsRootMachine = comp.IsRootMachine;
            item.Name = comp.Name;
            item.PyshicalAddress = comp.PyshicalAddress;
            item.IsVisible = comp.IsVisible;
            dbaccess.SaveChanges();
        }

        public void UpsertOperation(Operation opr)
        {
            var item = dbaccess.Operation.Where(x => x.Id == opr.Id).FirstOrDefault<Operation>();
            if (item == null)
                dbaccess.Operation.Add(opr);
            else
            {
                item.Id = opr.Id;
                item.Name = opr.Name;
            }
            dbaccess.SaveChanges();
        }
        public List<Operation> ListOperation()
        {
            try
            {
                return dbaccess.Operation.ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Operation> ListOperation(int firmid)
        {
            return dbaccess.Operation.Where(x=>x.FirmId== firmid).ToList();
        }
        public void DeleteOperation(Operation opr)
        {
            dbaccess.Operation.Remove(opr);
            dbaccess.SaveChanges();
        }

       

        public void UpdateOperation(Operation opr)
        {
            var item = dbaccess.Operation.Where(x => x.Id == opr.Id).FirstOrDefault<Operation>();
            item.Id = opr.Id;
            item.Name = opr.Name;
            dbaccess.SaveChanges();
        }

        public void UpsertOperationDetail(CheckLicense oprdetail)
        {
            var item = dbaccess.CheckLicense.Where(x => x.Id == oprdetail.Id).FirstOrDefault<CheckLicense>();
            if (item == null)
                dbaccess.CheckLicense.Add(oprdetail);
            else
            {
                item.IsUnlicensed = oprdetail.IsUnlicensed;
                item.Output = oprdetail.Output;
                item.CheckDate = oprdetail.CheckDate;
                item.UpdateDate = DateTime.Now;
            }
            dbaccess.SaveChanges();
        }

        public List<CheckLicense> ListOprDetail()
        {
            return dbaccess.CheckLicense.ToList();
        }

        public void DeleteOprDetail(CheckLicense oprdetail)
        {
            dbaccess.CheckLicense.Remove(oprdetail);
            dbaccess.SaveChanges();
        }

        public void UpdateOprDetail(CheckLicense oprdetail)
        {
            var item = dbaccess.CheckLicense.Where(x => x.Id == oprdetail.Id).FirstOrDefault<CheckLicense>();
            item.IsUnlicensed = oprdetail.IsUnlicensed;
            item.Output = oprdetail.Output;
            item.CheckDate = oprdetail.CheckDate;
            item.UpdateDate = DateTime.Now;

            dbaccess.SaveChanges();
        }
    }
}
