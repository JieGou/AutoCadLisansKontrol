using LicenseControllerWs.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL.Service
{
    public class FirmService : BaseService
    {
        public void Upsert(FirmDTO firm)
        {
            try
            {
                var model = Converter.Convert<FirmEntity, FirmDTO>(firm);
                var item = dbaccess.Firm.Where(x => x.Id == model.Id).FirstOrDefault<FirmEntity>();

                if (item == null)
                {
                    model.InsertDate = DateTime.Now;
                    dbaccess.Firm.Add(model);
                    dbaccess.Firm_User_RL.Add(new Firm_User_RL { FirmId = firm.Id, UserId = firm.UserId });
                    dbaccess.Firm_User_RL.Add(new Firm_User_RL { FirmId = firm.Id, UserId = 1 });//admin user has to see all firms.....

                }
                else
                {
                    item.Name = model.Name;
                    item.Contact = model.Contact;
                    item.Address = model.Address;
                    item.PhoneNo = model.PhoneNo;
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

        public FirmDTO Get(int? firmId)
        {
            var item = dbaccess.Firm.Where(x => x.Id == firmId).FirstOrDefault<FirmEntity>();
            return Converter.Convert<FirmDTO, FirmEntity>(item);
        }
        public List<FirmDTO> List(int userid)
        {
            var list = mssqlappaccess.QueryList<FirmEntity>("SP_SELECT_FIRM", new List<System.Data.SqlClient.SqlParameter>() {

                    new System.Data.SqlClient.SqlParameter("@USERID",userid)
                });
            return Converter.Convert<FirmDTO, FirmEntity>(list);
        }

        public void Delete(int firmid)
        {
            try
            {
                mssqlappaccess.ExecuteNonQuery("SP_DELETE_FIRM", new List<System.Data.SqlClient.SqlParameter>() {

                    new System.Data.SqlClient.SqlParameter("@FIRMID",firmid)
                });


                var removed = dbaccess.Firm_User_RL.Where(x => x.FirmId == firmid).FirstOrDefault();

                if (removed != null)
                {
                    dbaccess.Firm_User_RL.Remove(removed);

                }
                var adminright = dbaccess.Firm_User_RL.Where(x => x.FirmId == 1).FirstOrDefault();
                if (adminright != null)
                {
                    dbaccess.Firm_User_RL.Remove(adminright);
                }
                dbaccess.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(FirmDTO firm)
        {
            try
            {
                var model = Converter.Convert<FirmEntity, FirmDTO>(firm);
                var item = dbaccess.Firm.Where(x => x.Id == model.Id).FirstOrDefault<FirmEntity>();
                item.Name = model.Name;
                dbaccess.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
