using LicenseControllerWs.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL.Service
{
    public class CheckLicenseService : BaseService
    {
        public void Upsert(CheckLicenseDTO oprdetail)
        {
            var model = Converter.Convert<CheckLicense, CheckLicenseDTO>(oprdetail);
            try
            {
                var item = dbaccess.CheckLicense.Where(x => (x.Name == model.Name || x.Ip==model.Ip) && x.OperationId==model.OperationId).FirstOrDefault<CheckLicense>();
                if (item == null)
                    dbaccess.CheckLicense.Add(model);
                else
                {
                    item.Output = model.Output;
                    item.CheckDate = model.CheckDate;
                    item.UpdateDate = DateTime.Now;
                    item.State = model.State;
                    item.Installed = model.Installed;
                    item.Uninstalled = model.Uninstalled;
                    item.InstallDate = model.InstallDate;
                    item.UnInstallDate = model.UnInstallDate;
                    item.IsFound = model.IsFound;
                    item.LogId = model.LogId;
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

        public List<CheckLicenseDTO> List(int id)
        {
            var data = dbaccess.CheckLicense.Where(x => x.OperationId == id).ToList();
            return Converter.Convert<CheckLicenseDTO, CheckLicense>(data);
        }
        public void Delete(CheckLicenseDTO oprdetail)
        {
            var model = Converter.Convert<CheckLicense, CheckLicenseDTO>(oprdetail);
            dbaccess.CheckLicense.Remove(model);
            dbaccess.SaveChanges();
        }

        public void Update(CheckLicenseDTO oprdetail)
        {
            var model = Converter.Convert<CheckLicense, CheckLicenseDTO>(oprdetail);
            var item = dbaccess.CheckLicense.Where(x => x.Id == model.Id).FirstOrDefault<CheckLicense>();
            item.Output = model.Output;
            item.CheckDate = model.CheckDate;
            item.UpdateDate = DateTime.Now;

            dbaccess.SaveChanges();
        }

        public void DeleteAllBaseOperationid(int oprId)
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
