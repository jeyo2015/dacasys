namespace NLogin
{
    using System;
    using System.Threading;
    using System.Data;
    public class EnviarCorreo
    {
        Thread hilo;
        DataTable gDTEmpresas;
        ABMEmpresa gABMEmpresa;
        public void Iniciar(DataTable Empresas)
        {
            gDTEmpresas = Empresas;


            hilo = new Thread(Verificar_Licencias);
            hilo.Start();


            /// hilo.Join();
        }

        public void Verificar_Licencias()
        {

            ABMEmpresa em = new ABMEmpresa();
            DataTable vDTLicencia = new DataTable();
            foreach (DataRow IDEmpresa in gDTEmpresas.Rows)
            {
                String[] vLicencia = em.Get_LicenciaString((int)IDEmpresa[0]).Split('-');
                DateTime datelc = Convert.ToDateTime(vLicencia[1].Trim());
                TimeSpan diff = datelc.Subtract(DateTime.Now.AddHours(em.Get_DirefenciaHora()));
                int dias = diff.Days + 1;
                //if(diff.Hours >)
                if (dias <= 3 && dias > 0)///falta3 dias para vencer
                {
                    Enviar_Correo((int)IDEmpresa[0], dias);
                }
                else
                {
                    if (dias <= 0 && dias > -3) /// controla 3 dias despued de vecido
                    {
                        Enviar_Correo_Vencido((int)IDEmpresa[0], Math.Abs(dias) + 1);
                    }
                    else
                        if (dias < -2)///Se vencio los 3 dias mas
                        {
                            gABMEmpresa = new ABMEmpresa();
                            gABMEmpresa.Eliminar((int)IDEmpresa[0], "00000");
                        }

                }
            }



        }

        private void Enviar_Correo_Vencido(int pIDEmpresa, int pDiasrestantes)
        {
            gABMEmpresa = new ABMEmpresa();
            DataTable vDTEmpresa = gABMEmpresa.Get_Empresap(pIDEmpresa);

            if (vDTEmpresa.Rows.Count > 0)
            {
                SMTP vSMTP = new SMTP();
                String vMensaje = vDTEmpresa.Rows[0][2].ToString() + " buenos dias, \n" +
                                     "Se le informa que su licencia Mediweb se vencio hace " + Convert.ToString(pDiasrestantes) + " dias," +
                                     " \npor tal motivo algunas funciones se encuentran desactivadas." +
                                     "\nPasados los 3 dias su acceso sera completamente denegado, \npor favor ampliar su licencia " +
                                     "para evitar inconvenientes." +
                                     "\nSaludos, \nMediweb" +
                                     "\nPara mayor informacion contáctenos : soporte@dacasys.com ";
                vSMTP.Datos_Mensaje(vDTEmpresa.Rows[0][13].ToString(),
                                      vMensaje, "Licencia - Mediweb");

                vSMTP.Enviar_Mail();
            }
        }

        private void Enviar_Correo(int pIDEmpresa, int pDiasrestantes)
        {
            gABMEmpresa = new ABMEmpresa();
            DataTable vDTEmpresa = gABMEmpresa.Get_Empresap(pIDEmpresa);

            if (vDTEmpresa.Rows.Count > 0)
            {
                SMTP vSMTP = new SMTP();
                String vMensaje = vDTEmpresa.Rows[0][2].ToString() + " buenos dias, \n" +
                                     "Se le informa que en " + Convert.ToString(pDiasrestantes) + " dias vence su lincencia" +
                                     " Mediweb, \npor favor ampliar la misma para evitar los inconvenientes." +
                                     "\nSaludos, \nMediweb" +
                                     "\nPara mayor informacion contáctenos : soporte@dacasys.com ";
                vSMTP.Datos_Mensaje(vDTEmpresa.Rows[0][13].ToString(),
                                      vMensaje, "Licencia - Mediweb");

                vSMTP.Enviar_Mail();
            }
        }
    }
}