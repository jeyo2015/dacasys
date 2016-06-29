namespace NAgenda
{
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using Herramientas;

    public class ABMDia
    {
        #region VariablesGlogales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        public static List<DiaDto> ObtenerDias()
        {
            return (from d in dataContext.Dia
                    orderby d.iddia
                    select new DiaDto()
                    {
                        IDDia = d.iddia,
                        NombreDia = d.descripcion,
                        NombreCorto = d.nombre_corto,
                        IsChecked = false
                    }).ToList();
        }

        #endregion
    }
}