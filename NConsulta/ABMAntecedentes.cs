namespace NConsulta
{
    using System.Collections.Generic;
    using System.Linq;
    using DataTableConverter;
    using System.Data;
    using Datos;

    public class ABMAntecedentes
    {
        #region VariableGlobales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta un Atecedente a un paciente
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pAntecedente">Descripcion del antecedente</param>
        public static void Insertar(int pIDpaciente, string pAntecedente)
        {
            Antecedentes vAntecedente = new Antecedentes();
            vAntecedente.id_paciente = pIDpaciente;
            vAntecedente.descripcion = pAntecedente;


            dataContext.Antecedentes.InsertOnSubmit(vAntecedente);
            dataContext.SubmitChanges();


        }

        /// <summary>
        /// Permite elimnar un antecedente
        /// </summary>
        /// <param name="pIDPaciente">Id del paciente</param>
        /// <param name="pIDAntecedente">ID del antecedente</param>
        public static void Eliminar(int pIDPaciente, int pIDAntecedente)
        {
            var sql = from c in dataContext.Antecedentes
                      where c.id_paciente == pIDPaciente && c.id_antecedente == pIDAntecedente
                      select c;

            if (!sql.Any()) return;
            Antecedentes vDA = new Antecedentes();
            vDA.id_antecedente = pIDAntecedente;
            vDA.id_paciente = pIDPaciente;
            vDA.descripcion = sql.First().descripcion;
            dataContext.Antecedentes.DeleteOnSubmit(vDA);
            dataContext.SubmitChanges();
        }

        /// <summary>
        /// Modifica un antecedente
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pAntecedente">Descripcion del antecedente</param>
        /// <param name="pIDAntecedente">ID del antecedente</param>
        public static void Modificar(int pIDpaciente, string pAntecedente, int pIDAntecedente)
        {

            var sql = from c in dataContext.Antecedentes
                      where c.id_paciente == pIDpaciente && c.id_antecedente == pIDAntecedente
                      select c;

            if (!sql.Any()) return;
            sql.First().descripcion = pAntecedente;
            dataContext.SubmitChanges();
        }

        /// <summary>
        /// Devuelve los antedecentes de un paciente
        /// </summary>
        /// <param name="pIDPaciente">ID del paciente</param>
        /// <returns>Retorna un DataTable con los antecedentes</returns>
        public static DataTable Get_Pacientesp(int pIDPaciente)
        {
            return Converter<Antecedentes>.Convert(Get_Antecedentes(pIDPaciente).ToList());
        }

        #endregion

        #region Metodos Privados

        private static IEnumerable<Antecedentes> Get_Antecedentes(int pIDPaciente)
        {
            return from p in dataContext.Antecedentes
                   where p.id_paciente == pIDPaciente
                   select p;
        }

        #endregion
    }
}