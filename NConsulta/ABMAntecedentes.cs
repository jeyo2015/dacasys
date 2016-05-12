using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEventos;
using DataTableConverter;
using System.Data;
using NLogin;
using DConsulta;

namespace NConsulta
{
   public class ABMAntecedentes
    {
        #region VariableGlobales
        DConsultaDataContext gDc = new DConsultaDataContext();
       
        // ControlBitacora gCb = new ControlBitacora();
        // ControlLogErrores gLe = new ControlLogErrores();

        #endregion

        #region ABM_Antecedentes

       /// <summary>
       /// Inserta un Atecedente a un paciente
       /// </summary>
       /// <param name="pIDpaciente">ID del paciente</param>
       /// <param name="pAntecedente">Descripcion del antecedente</param>
        public void Insertar(int pIDpaciente,string pAntecedente)
        {
            Antecedentes vAntecedente = new Antecedentes();
            vAntecedente.id_paciente = pIDpaciente;
            vAntecedente.descripcion = pAntecedente;
           

            gDc.Antecedentes.InsertOnSubmit(vAntecedente);
            gDc.SubmitChanges();
           

        }

       /// <summary>
       /// Permite elimnar un antecedente
       /// </summary>
       /// <param name="pIDPaciente">Id del paciente</param>
       /// <param name="pIDAntecedente">ID del antecedente</param>
        public void Eliminar(int pIDPaciente, int pIDAntecedente)
        {
            var sql = from c in gDc.Antecedentes
                      where c.id_paciente == pIDPaciente && c.id_antecedente == pIDAntecedente
                      select c;
            
            if (sql.Count() > 0)
            {
                Antecedentes vDA = new Antecedentes();
                vDA.id_antecedente = pIDAntecedente;
                vDA.id_paciente = pIDPaciente;
                vDA.descripcion = sql.First().descripcion;
                gDc.Antecedentes.DeleteOnSubmit(vDA);

                gDc.SubmitChanges();

            }


        }

       /// <summary>
       /// Modifica un antecedente
       /// </summary>
       /// <param name="pIDpaciente">ID del paciente</param>
       /// <param name="pAntecedente">Descripcion del antecedente</param>
       /// <param name="pIDAntecedente">ID del antecedente</param>
        public void Modificar(int pIDpaciente, string pAntecedente, int pIDAntecedente)
        {

            var sql = from c in gDc.Antecedentes
                      where c.id_paciente == pIDpaciente && c.id_antecedente == pIDAntecedente
                      select c;

            if (sql.Count() > 0)
            {
                sql.First().descripcion = pAntecedente;
                
                gDc.SubmitChanges();
            }

            

        }


        private IEnumerable<Antecedentes> Get_Antecedentes(int pIDPaciente)
        {
            return from p in gDc.Antecedentes
                   where p.id_paciente == pIDPaciente
                   select p;
        }

       /// <summary>
       /// Devuelve los antedecentes de un paciente
       /// </summary>
       /// <param name="pIDPaciente">ID del paciente</param>
       /// <returns>Retorna un DataTable con los antecedentes</returns>
        public DataTable Get_Pacientesp(int pIDPaciente)
        {
            return Converter<Antecedentes>.Convert(Get_Antecedentes(pIDPaciente).ToList());
        }

        #endregion

    }
       

}
