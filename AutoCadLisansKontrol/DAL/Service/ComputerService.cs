using LicenseControllerWs.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL.Service
{
    public class ComputerService : BaseService
    {
        public int Upsert(ComputerDTO cmp)
        {
            try
            {
                var model= Converter.Convert<ComputerEntity,ComputerDTO>(cmp);

                var item = dbaccess.Computer.Where(x => x.Name == model.Name && x.FirmId == model.FirmId).FirstOrDefault<ComputerEntity>();
                if (item == null)
                {
                    dbaccess.Computer.Add(model);
                }
                else
                {
                    item.Ip = model.Ip;
                    item.IsComputer = model.IsComputer;
                    item.IsRootMachine = model.IsRootMachine;
                    item.Name = model.Name;
                    item.PyshicalAddress = model.PyshicalAddress;
                    item.IsVisible = model.IsVisible;
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
        public List<ComputerDTO> List()
        {
            var comps = dbaccess.Computer.ToList();
            return Converter.Convert<ComputerDTO, ComputerEntity>(comps);
        }
        public List<ComputerDTO> List(int? firmId)
        {
            var comps = dbaccess.Computer.Where(x => x.FirmId == firmId).ToList();
            return Converter.Convert<ComputerDTO, ComputerEntity>(comps);
        }

        public void Delete(int id)
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
        public void Update(ComputerDTO comp)
        {
            var model = Converter.Convert<ComputerEntity, ComputerDTO>(comp);
            var item = dbaccess.Computer.Where(x => x.Id == model.Id).FirstOrDefault<ComputerEntity>();
            item.Ip = model.Ip;
            item.IsComputer = model.IsComputer;
            item.IsRootMachine = model.IsRootMachine;
            item.Name = model.Name;
            item.PyshicalAddress = model.PyshicalAddress;
            item.IsVisible = model.IsVisible;
            dbaccess.SaveChanges();
        }
        public void DeleteAllBaseFormid(int firmId)
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
    }
}
