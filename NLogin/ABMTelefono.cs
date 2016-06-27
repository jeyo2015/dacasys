namespace NLogin
{
    using System;
    using System.Linq;
    using Datos;
    using Herramientas;

    public class ABMTelefono
    {
        #region VariablesGlogales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta un nuevo número telefónico al sistema
        /// </summary>
        /// <param name="telefonoDto">IDEmpresa de la Empresa a la que pertenece el número telefónico</param>
        public static void InsertarConsultorio(TelefonoDto telefonoDto)
        {
            try
            {
                if (telefonoDto.IDClinica > 0)
                {
                    dataContext.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
                    {
                        IDTelefono = telefonoDto.ID,
                        IDConsultorio = telefonoDto.IDConsultorio
                    });
                }
                else
                {
                    var newTelefono = new Telefono { Nombre = telefonoDto.Nombre, Telefono1 = telefonoDto.Telefono };
                    dataContext.Telefono.InsertOnSubmit(newTelefono);
                    var newIDTelefono = ObtenerCodigo();
                    dataContext.TelefonoConsultorio.InsertOnSubmit(new TelefonoConsultorio()
                    {
                        IDTelefono = newIDTelefono,
                        IDConsultorio = telefonoDto.IDConsultorio
                    });
                }
            }
            catch (Exception) { }
        }

        public static void InsertarTelefonosDeLaClinica(int idClinica, string telefono, string nombre)
        {
            var newTelefono = new Telefono {Nombre = nombre, Telefono1 = telefono, Estado = true};
            try
            {
                dataContext.Telefono.InsertOnSubmit(newTelefono);
                dataContext.SubmitChanges();
                var newIDTelefono = ObtenerCodigo();
                dataContext.TefonosClinica.InsertOnSubmit(new TefonosClinica()
                {
                    IDTelefono = newIDTelefono,
                    IDClinica = idClinica
                });
                dataContext.SubmitChanges();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Modifica un telefono
        /// </summary>
        /// <param name="idEmpresa">ID de la empresa</param>
        /// <param name="telefono">Numero de telefono</param>
        /// <param name="nombre">Numero de telefono</param>
        /// <param name="idTelefono">id de telefono</param>
        public static void Modificar(int idEmpresa, string telefono, string nombre, int idTelefono)
        {
            var sql = (from e in dataContext.Telefono
                       where e.ID == idTelefono
                       select e).FirstOrDefault();

            if (sql == null) return;
            sql.Nombre = nombre;
            sql.Telefono1 = telefono;
            dataContext.SubmitChanges();
        }

        /// <summary>
        /// Pone a un Telefono como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="telefono">Es el ID del Telefono a eliminar</param>
        public static void Eliminar(string telefono)
        {
            var sql = from e in dataContext.Telefono
                      where e.Telefono1 == telefono
                      select e;

            if (!sql.Any()) return;
            sql.First().Estado = false;
            dataContext.SubmitChanges();
        }

        #endregion

        #region Metodos Private

        private static int ObtenerCodigo()
        {
            var telefonos = (from t in dataContext.Telefono
                             select t);
            return telefonos.Max(x => x.ID) + 1;
        }

        #endregion
    }
}