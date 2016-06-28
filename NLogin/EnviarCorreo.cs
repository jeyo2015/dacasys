namespace NLogin
{
    using System;
    using System.Threading;
    using System.Data;
    public class EnviarCorreo
    {
        Thread hilo;
        DataTable gDTEmpresas;

        #region Metodos Publicos

        public void Iniciar(DataTable Empresas)
        {
            gDTEmpresas = Empresas;
            hilo = new Thread(VerificarLicencias);
            hilo.Start();
        }

        public void VerificarLicencias()
        {
            foreach (DataRow IDEmpresa in gDTEmpresas.Rows)
            {
                var vLicencia = ABMEmpresa.ObtenerLicenciaClinica((int)IDEmpresa[0]).Split('-');
                var datelc = Convert.ToDateTime(vLicencia[1].Trim());
                var diff = datelc.Subtract(DateTime.Now.AddHours(ABMEmpresa.DirefenciaHora()));
                var dias = diff.Days + 1;
                //if(diff.Hours >)
                if (dias <= 3 && dias > 0)///falta3 dias para vencer
                {
                    Enviar_Correo((int)IDEmpresa[0], dias);
                }
                else
                {
                    if (dias <= 0 && dias > -3) /// controla 3 dias despued de vecido
                    {
                        EnviarCorreoVencido((int)IDEmpresa[0], Math.Abs(dias) + 1);
                    }
                    else
                        if (dias < -2)///Se vencio los 3 dias mas
                        {
                            ABMEmpresa.EliminarConsultorio((int)IDEmpresa[0], "00000");
                        }
                }
            }
        }

        #endregion

        #region Metodos Privados

        private static void EnviarCorreoVencido(int idEmpresa, int diasRestantes)
        {
            var vDTEmpresa = ABMEmpresa.ObtenerEmpresa(idEmpresa);
            if (vDTEmpresa.Rows.Count <= 0) return;
            var vSMTP = new SMTP();
            var vMensaje = vDTEmpresa.Rows[0][2].ToString() + " buenos dias, \n" +
                              "Se le informa que su licencia Odontoweb se vencio hace " + Convert.ToString(diasRestantes) + " dias," +
                              " \npor tal motivo algunas funciones se encuentran desactivadas." +
                              "\nPasados los 3 dias su acceso sera completamente denegado, \npor favor ampliar su licencia " +
                              "para evitar inconvenientes." +
                              "\nSaludos, \nOdontoweb" +
                              "\nPara mayor informacion contáctenos : soporte@dacasys.com ";
            vSMTP.Datos_Mensaje(vDTEmpresa.Rows[0][13].ToString(),
                vMensaje, "Licencia - Odontoweb");

            vSMTP.Enviar_Mail();
        }

        private static void Enviar_Correo(int idEmpresa, int diasRestantes)
        {
            var vDTEmpresa = ABMEmpresa.ObtenerEmpresa(idEmpresa);
            if (vDTEmpresa.Rows.Count <= 0) return;
            var vSMTP = new SMTP();
            var vMensaje = vDTEmpresa.Rows[0][2].ToString() + " buenos dias, \n" +
                              "Se le informa que en " + Convert.ToString(diasRestantes) + " dias vence su lincencia" +
                              " Odontoweb, \npor favor ampliar la misma para evitar los inconvenientes." +
                              "\nSaludos, \nOdontoweb" +
                              "\nPara mayor informacion contáctenos : soporte@dacasys.com ";
            vSMTP.Datos_Mensaje(vDTEmpresa.Rows[0][13].ToString(),
                vMensaje, "Licencia - Odontoweb");

            vSMTP.Enviar_Mail();
        }

        #endregion
    }
}