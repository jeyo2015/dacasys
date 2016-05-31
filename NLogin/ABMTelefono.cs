namespace NLogin
{
    using System;
    using System.Linq;
    using Datos;
    using Herramientas;

    public class ABMTelefono
    {
        #region VariablesGlogales

        readonly DataContext dataContext = new DataContext();

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
                    dataContext.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
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
                    dataContext.Telefono.InsertOnSubmit(newTelefono);
                    dataContext.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
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
                dataContext.Telefono.InsertOnSubmit(newTelefono);
                dataContext.TefonosClinica.InsertOnSubmit(new TefonosClinica()
                {
                    IDTelefono = newIDTelefono,
                    IDClinica = pIDClinica
                });
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
            }
        }

        private int GetNextIDTelefono()
        {
            var telefonos = (from t in dataContext.Telefono
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
            var sql = (from e in dataContext.Telefono
                       where e.ID == IDTelefono

                       select e).FirstOrDefault();

            if (sql != null)
            {
                sql.Nombre = pnombre;
                sql.Telefono1 = pTelefono;
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Pone a un Telefono como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="pID">Es el ID del Telefono a eliminar</param>
        public void Eliminar(string pTelefono)
        {
            var sql = from e in dataContext.Telefono
                      where e.Telefono1 == pTelefono
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().Estado = false;
                dataContext.SubmitChanges();
            }
        }

        #endregion
    }
}