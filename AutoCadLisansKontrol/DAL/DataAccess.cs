using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
            try
            {
                var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<Firm>();

                if (item == null) {
                    firm.InsertDate = DateTime.Now;
                    dbaccess.Firm.Add(firm);
                }
                else { 
                    item.Name = firm.Name;
                    item.Contact = firm.Contact;
                    item.Address = firm.Address;
                    item.PhoneNo = firm.PhoneNo;
                }
                dbaccess.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string validationerror = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    validationerror = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \""+ eve.Entry.State + "\" has the following validation errors:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        validationerror += "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"";
                    }
                }

                throw new Exception(validationerror);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {

                    if (ex.InnerException.InnerException != null)
                    {
                        throw new Exception(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        throw new Exception(ex.InnerException.Message);
                    }
                }
                else
                    throw new Exception(ex.Message);
            }

        }
        public Firm GetFirm(int? firmId)
        {
            var item = dbaccess.Firm.Where(x => x.Id == firmId).FirstOrDefault<Firm>();
            return item;
        }
        public List<Firm> ListFirm()
        {
            return dbaccess.Firm.ToList();
        }

        public void DeleteFirm(int firmid)
        {
            try
            {


                var item = dbaccess.Firm.SingleOrDefault(x => x.Id == firmid);
                var computers = item.Computer.ToList().GetRange(0, item.Computer.Count);
                var operations = item.Operation.ToList().GetRange(0, item.Operation.Count);
                if (item != null)
                {
                    if (item.Computer.Count > 0)
                    {
                        foreach (var comp in computers)
                        {
                            dbaccess.Computer.Remove(comp);
                            dbaccess.SaveChanges();
                        }
                    }

                    if (item.Operation.Count > 0)
                    {
                        foreach (var opr in operations)
                        {
                            dbaccess.Operation.Remove(opr);
                            dbaccess.SaveChanges();
                        }
                    }

                    dbaccess.Entry(item).State = EntityState.Deleted;
                    dbaccess.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateFirm(Firm firm)
        {
            try
            {
                var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<Firm>();
                item.Name = firm.Name;
                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void UpsertComputer(Computer c)
        {

            try
            {
                var item = dbaccess.Computer.Where(x => x.Id == c.Id).FirstOrDefault<Computer>();
                if (item == null)
                {
                    item = new Computer { Id = c.Id, Ip = c.Ip, IsComputer = c.IsComputer, IsRootMachine = c.IsRootMachine, IsVisible = c.IsVisible, Name = c.Name, PyshicalAddress = c.PyshicalAddress, FirmId = c.FirmId, Type = c.Type, InsertDate = c.InsertDate };
                    dbaccess.Computer.Add(item);
                }
                else
                {
                    item.Ip = c.Ip;
                    item.IsComputer = c.IsComputer;
                    item.IsRootMachine = c.IsRootMachine;
                    item.Name = c.Name;
                    item.PyshicalAddress = c.PyshicalAddress;
                    item.IsVisible = c.IsVisible;
                }


                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
            return dbaccess.Computer.Where(x => x.FirmId == firmId).ToList();
        }
        public void DeleteComputer(string Ip)
        {
            try
            {
                var item = dbaccess.Computer.SingleOrDefault(x => x.Ip.Contains(Ip));

                if (item != null)
                    dbaccess.Computer.Remove(item);
                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
          
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
            return dbaccess.Operation.Where(x => x.FirmId == firmid).ToList();
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

        public void UpsertCheckLicense(CheckLicense oprdetail)
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

        public void DeleteCheckLicense(CheckLicense oprdetail)
        {
            dbaccess.CheckLicense.Remove(oprdetail);
            dbaccess.SaveChanges();
        }

        public void UpdateCheckLicense(CheckLicense oprdetail)
        {
            var item = dbaccess.CheckLicense.Where(x => x.Id == oprdetail.Id).FirstOrDefault<CheckLicense>();
            item.IsUnlicensed = oprdetail.IsUnlicensed;
            item.Output = oprdetail.Output;
            item.CheckDate = oprdetail.CheckDate;
            item.UpdateDate = DateTime.Now;

            dbaccess.SaveChanges();
        }

        public void DeleteAllComputerBaseFormid(int firmId)
        {
            var comps = dbaccess.Computer.Where(x => x.FirmId == firmId).ToList();

            foreach (var item in comps)
            {
                dbaccess.Computer.Remove(item);
            }
            dbaccess.SaveChanges();
        }
    }
}
