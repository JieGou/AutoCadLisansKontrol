using CheckLicense.Log;
using CheckLicense.MssqlLogger;
using ProtectionConnLib_4.DataAccessModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL
{

    public class DataAccess
    {
        AUTOCADLICENSEEntities1 dbaccess = new AUTOCADLICENSEEntities1();
        MssqlDbAccess mssqlappaccess = new MssqlDbAccess(System.Configuration.ConfigurationManager.ConnectionStrings["AutocadLicenseDatabase"].ConnectionString);

        public void UpsertFirm(FirmEntity firm)
        {
            try
            {
                var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<FirmEntity>();

                if (item == null)
                {
                    firm.InsertDate = DateTime.Now;
                    dbaccess.Firm.Add(firm);
                }
                else
                {
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
                    validationerror = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
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
        public FirmEntity GetFirm(int? firmId)
        {
            var item = dbaccess.Firm.Where(x => x.Id == firmId).FirstOrDefault<FirmEntity>();
            return item;
        }
        public List<FirmEntity> ListFirm()
        {

            return dbaccess.Firm.ToList();
        }

        public void DeleteFirm(int firmid)
        {
            try
            {

                mssqlappaccess.ExecuteNonQuery("SP_DELETE_FIRM", new List<System.Data.SqlClient.SqlParameter>() {

                    new System.Data.SqlClient.SqlParameter("@FIRMID",firmid)
                });

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateFirm(FirmEntity firm)
        {
            try
            {
                var item = dbaccess.Firm.Where(x => x.Id == firm.Id).FirstOrDefault<FirmEntity>();
                item.Name = firm.Name;
                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int UpsertComputer(ComputerEntity c)
        {
            try
            {
                var item = dbaccess.Computer.Where(x => x.Name == c.Name && x.FirmId == c.FirmId).FirstOrDefault<ComputerEntity>();
                if (item == null)
                {
                    item = new ComputerEntity { Id = c.Id, Ip = c.Ip, IsComputer = c.IsComputer, IsRootMachine = c.IsRootMachine, IsVisible = c.IsVisible, Name = c.Name, PyshicalAddress = c.PyshicalAddress, FirmId = c.FirmId, Type = c.Type, InsertDate = DateTime.Now };
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
                return item.Id;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string validationerror = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    validationerror = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
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

        public OperationEntity GetOperation(int opr)
        {
            var item = dbaccess.Operation.Where(x => x.Id == opr).FirstOrDefault<OperationEntity>();
            return item;
        }

        public List<ComputerEntity> ListComputer()
        {
            return dbaccess.Computer.ToList();
        }
        public List<ComputerEntity> ListComputer(int? firmId)
        {
            return dbaccess.Computer.Where(x => x.FirmId == firmId).ToList();
        }



        public void DeleteComputer(int id)
        {
            try
            {
                var item = dbaccess.Computer.SingleOrDefault(x => x.Id == id);

                if (item != null)
                    dbaccess.Computer.Remove(item);
                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void UpdateComputer(ComputerEntity comp)
        {
            var item = dbaccess.Computer.Where(x => x.Id == comp.Id).FirstOrDefault<ComputerEntity>();
            item.Ip = comp.Ip;
            item.IsComputer = comp.IsComputer;
            item.IsRootMachine = comp.IsRootMachine;
            item.Name = comp.Name;
            item.PyshicalAddress = comp.PyshicalAddress;
            item.IsVisible = comp.IsVisible;
            dbaccess.SaveChanges();
        }

        public List<ControlPoint> GetControlPoint()
        {
            try
            {
                return dbaccess.ControlPoint.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpsertOperation(OperationEntity opr)
        {
            try
            {
                var item = dbaccess.Operation.Where(x => x.Id == opr.Id).FirstOrDefault<OperationEntity>();
                if (item == null)
                {
                    dbaccess.Operation.Add(opr);

                    //var list = dbaccess.Computer.Where(x => x.FirmId == opr.FirmId).ToList();

                    //if (list != null)
                    //{
                    //    var checklicense = list.ConvertAll(x => new CheckLicenseEntity { FirmId = opr.FirmId, ComputerId = x.Id, OperationId = opr.Id });

                    //    foreach (var chck in checklicense)
                    //    {
                    //        dbaccess.CheckLicense.Add(chck);
                    //    }
                    //    dbaccess.SaveChanges();
                    //}

                }
                else
                {
                    item.Id = opr.Id;
                    item.Name = opr.Name;
                }
                dbaccess.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string validationerror = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    validationerror = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
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
        public List<OperationEntity> ListOperation()
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
        public List<OperationEntity> ListOperation(int firmid)
        {
            return dbaccess.Operation.Where(x => x.FirmId == firmid).ToList();
        }
        public void DeleteOperation(OperationEntity opr)
        {

            mssqlappaccess.ExecuteNonQuery("SP_DELETE_OPERATION", new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@OPRID", opr.Id) });
        }



        public void UpdateOperation(OperationEntity opr)
        {
            var item = dbaccess.Operation.Where(x => x.Id == opr.Id).FirstOrDefault<OperationEntity>();
            item.Id = opr.Id;
            item.Name = opr.Name;
            dbaccess.SaveChanges();
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
        public void UpsertCheckLicense(CheckLicenseEntity oprdetail)
        {
            try
            {
                var item = dbaccess.CheckLicense.Where(x => x.Id == oprdetail.Id).FirstOrDefault<CheckLicenseEntity>();
                if (item == null)
                    dbaccess.CheckLicense.Add(oprdetail);
                else
                {
                    item.Output = oprdetail.Output;
                    item.CheckDate = oprdetail.CheckDate;
                    item.UpdateDate = DateTime.Now;
                }
                dbaccess.SaveChanges();



            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string validationerror = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    validationerror = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
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

        public List<CheckLicenseEntity> ListCheckLicense(int id)
        {
            return dbaccess.CheckLicense.Where(x => x.OperationId == id).ToList();
        }

        public void DeleteCheckLicense(CheckLicenseEntity oprdetail)
        {
            dbaccess.CheckLicense.Remove(oprdetail);
            dbaccess.SaveChanges();
        }

        public void UpdateCheckLicense(CheckLicenseEntity oprdetail)
        {
            var item = dbaccess.CheckLicense.Where(x => x.Id == oprdetail.Id).FirstOrDefault<CheckLicenseEntity>();
            item.Output = oprdetail.Output;
            item.CheckDate = oprdetail.CheckDate;
            item.UpdateDate = DateTime.Now;

            dbaccess.SaveChanges();
        }

        public void DeleteAllComputerBaseFormid(int firmId)
        {
            try
            {
                mssqlappaccess.ExecuteNonQuery("SP_DELETE_COMPUTER", new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@FIRMID",firmId)
                });
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
        public void DeleteAllLicenseBaseOperationid(int oprId)
        {
            try
            {
                mssqlappaccess.ExecuteNonQuery("SP_DELETE_LICENSE", new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@OPRID",oprId)
                });
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
    }
}
