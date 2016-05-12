using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAgenda;
using DataTableConverter;
using System.Data;
namespace NAgenda
{
   public class ABMDia
    {
       #region VariablesGlogales
       DAgendaDataContext gDc = new DAgendaDataContext();
       #endregion

        #region MetodosPublicos
       /// <summary>
       /// Este metodo devolvera el codigo del dia, segun el nombre corto
       /// </summary>
       /// <param name="pnombrecorto">Es el nombre corto del dia</param>
       /// <returns> Retorna el codigo del dia, retorna -1 en caso de que no exista el dia</returns>
       public int Get_codigo_corto(string pnombrecorto)
       {
           
           var sql = from d in gDc.Dia
                     where d.nombre_corto == pnombrecorto
                     select d;
           if (sql.Count() > 0) {
               return sql.First().iddia;
           }
           return -1;
   
       }

       /// <summary>
       /// Este metodo devuelve el id del dia segun su descripcion
       /// </summary>
       /// <param name="pdescripcion">Es el nombre completo del dia</param>
       /// <returns>Retorna id de dia, retorna -1 en caso de que exista el dia</returns>
       public int Get_codigo(string pdescripcion)
       {
           var sql = from d in gDc.Dia
                     where d.descripcion== pdescripcion
                     select d;
           if (sql.Count() > 0)
           {
               return sql.First().iddia;
           }
           return -1;

       }

       /// <summary>
       /// Devuelve el nombre corto del dia segun el ID dia
       /// </summary>
       /// <param name="pID">Es el id del dia</param>
       /// <returns>El nombre corto del dia</returns>
       public string Get_nombre_corto(int pID)
       {
           var sql = from d in gDc.Dia
                     where d.iddia == pID
                     select d;
           if (sql.Count() > 0)
           {
               return sql.First().nombre_corto;
           }
           return "none";

       }
       
       /// <summary>
       /// Devuelve la descripcion del dia segun el ID dia
       /// </summary>
       /// <param name="pID">Es el ID del dia</param>
       /// <returns>Descripcion de dia segun el id</returns>
       public string Get_descripcion(int pID)
       {
           var sql = from d in gDc.Dia
                     where d.iddia == pID
                     select d;
           if (sql.Count() > 0)
           {
               return sql.First().descripcion;
           }
           return "none";

       }

       private IEnumerable<Dia> Get_Dias() {
           return from d in gDc.Dia select d;
       }

       /// <summary>
       /// Retorna la tabla Dias
       /// </summary>
       /// <returns> DataTable </returns>
       public DataTable Get_Diasp() {
           return Converter<Dia>.Convert(Get_Dias().ToList());
       }

        #endregion

    }
}
