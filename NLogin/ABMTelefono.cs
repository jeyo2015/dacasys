using System;
using System.Linq;
using DLogin;
using Herramientas;
namespace NLogin
{
    public class ABMTelefono
    {
        #region VariablesGlogales
        DLoginLinqDataContext gDc = new DLoginLinqDataContext();
        #endregion

        #region ABM_Telefono

        /// <summary>
        /// Inserta un nuevo número telefónico al sistema
        /// </summary>
        /// <param name="pIDEmpresa">IDEmpresa de la Empresa a la que pertenece el número telefónico</param>
        /// <param name="Telefono">Número Telefónico</param>
        public void InsertarConsultorio(TelefonoDto ptelefono)
        {
            try
            {
                if (ptelefono.IDClinica > 0)
                {
                    gDc.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
                    {
                        IDTelefono = ptelefono.ID,
                        IDConsultorio = ptelefono.IDConsultorio
                    });
                }
                else
                {
                    int newIDTelefono = GetNextIDTelefono();
                    var newTelefono = new Telefono();
                    newTelefono.ID = newIDTelefono;
                    newTelefono.Nombre = ptelefono.Nombre;
                    newTelefono.Telefono1 = ptelefono.Telefono;
                    gDc.Telefono.InsertOnSubmit(newTelefono);
                    gDc.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
                    {
                        IDTelefono = newIDTelefono,
                        IDConsultorio = ptelefono.IDConsultorio
                    });
                }
            }
            catch (Exception ex)
            {


            }


        }


        public void InsertarTelefonosDeLaClinica(int pIDClinica, string pTelefono, string pNombre)
        {
            int newIDTelefono = GetNextIDTelefono();
            var newTelefono = new Telefono();
            newTelefono.ID = newIDTelefono;
            newTelefono.Nombre = pNombre;
            newTelefono.Telefono1 = pTelefono;
            newTelefono.Estado = true;
            try
            {
                gDc.Telefono.InsertOnSubmit(newTelefono);
                gDc.TefonosClinica.InsertOnSubmit(new TefonosClinica()
                {
                    IDTelefono = newIDTelefono,
                    IDClinica = pIDClinica
                });
                gDc.SubmitChanges();
            }
            catch (Exception ex)
            {


            }


        }

        private int GetNextIDTelefono()
        {
            var telefonos = (from t in gDc.Telefono
                             select t);
            return telefonos == null ? 1 : telefonos.Max(x => x.ID) + 1;
        }
        /// <summary>
        /// Modifica un telefono
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pTelefono">Numero de telefono</param>
        public void Modificar(int pIDEmpresa, string pTelefono, string pnombre, int IDTelefono)
        {
            var sql = (from e in gDc.Telefono
                       where e.ID == IDTelefono

                       select e).FirstOrDefault();

            if (sql != null)
            {
                sql.Nombre = pnombre;
                sql.Telefono1 = pTelefono;
                gDc.SubmitChanges();
            }
        }

        /// <summary>
        /// Pone a un Telefono como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="pID">Es el ID del Telefono a eliminar</param>
        public void Eliminar(string pTelefono)
        {
            var sql = from e in gDc.Telefono
                      where e.Telefono1 == pTelefono
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().Estado = false;
                gDc.SubmitChanges();
            }
        }
        #endregion
    }
}
