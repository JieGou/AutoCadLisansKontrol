using LicenseControllerWs.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.DAL.Service
{
    public class OperationService : BaseService
    {
        public OperationDTO Get(int opr)
        {

            var item = dbaccess.Operation.Where(x => x.Id == opr).FirstOrDefault<OperationEntity>();
            return Converter.Convert<OperationDTO, OperationEntity>(item);
        }
        public void Upsert(OperationDTO opr)
        {
            try
            {
                var model = Converter.Convert<OperationEntity, OperationDTO>(opr);
                var item = dbaccess.Operation.Where(x => x.Id == model.Id).FirstOrDefault<OperationEntity>();
                if (item == null)
                {
                    dbaccess.Operation.Add(model);
                }
                else
                {
                    item.FirmId = model.FirmId;
                    item.Id = model.Id;
                    item.Name = model.Name;
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
        public List<OperationDTO> List()
        {
            try
            {
                var model = dbaccess.Operation.ToList();
                return Converter.Convert<OperationDTO, OperationEntity>(model);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<OperationDTO> List(int firmid)
        {
            var model = dbaccess.Operation.Where(x => x.FirmId == firmid).ToList();
            return Converter.Convert<OperationDTO, OperationEntity>(model);
        }
        public void Delete(OperationDTO opr)
        {
            mssqlappaccess.ExecuteNonQuery("SP_DELETE_OPERATION", new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@OPRID", opr.Id) });
        }



        public void Update(OperationDTO opr)
        {
            var model = Converter.Convert<OperationEntity, OperationDTO>(opr);
            var item = dbaccess.Operation.Where(x => x.Id == model.Id).FirstOrDefault<OperationEntity>();

            item.Id = model.Id;
            item.Name = model.Name;
            dbaccess.SaveChanges();
        }


    }
}
