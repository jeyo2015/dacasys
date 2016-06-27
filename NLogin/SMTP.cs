namespace NLogin
{
    using System;
    using System.Net.Mail;
    using NEventos;

    public class SMTP
    {
        string From;
        string To;
        string Message;
        string Subject;
        string Host;
        int Puerto;//puerto = 587(local) - puerto= 25(servidor)
        MailMessage Email;

        #region Metodos Publicos

        public void Datos_Mensaje(string from, string para, string mensaje, string asunto)
        {
            From = from;
            To = para;
            Message = mensaje;
            Subject = asunto;
            Host = "64.79.170.155";
            Puerto = 25;
        }

        public void Datos_Mensaje(string from, string para, string mensaje, string asunto, string pHost, int pPuerto)
        {
            From = from;
            To = para;
            Message = mensaje;
            Subject = asunto;
            Host = pHost;
            Puerto = pPuerto;
        }

        public int Enviar_Mail()
        {
            Email = new System.Net.Mail.MailMessage(From, To, Subject, Message);
            var smtpMail = new System.Net.Mail.SmtpClient();
            Email.Priority = MailPriority.Normal;
            Email.IsBodyHtml = false;

            smtpMail.EnableSsl = false;
            smtpMail.Port = Puerto;
            smtpMail.Host = Host;
            smtpMail.Credentials = new System.Net.NetworkCredential("odontoweb@dacasys.com", "Dacasys123");
            try
            {
                smtpMail.Send(Email);
                ControlBitacora.Insertar("Se envio correctamente correo", To);
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo :" + ex.Data);
                ControlLogErrores.Insertar("NLongin", "SMTP", "enviarmail", ex);
                return 0;
            }
        }

        public void Datos_Mensaje(string para, string mensaje, string asunto)
        {
            To = para;
            Message = mensaje;
            Subject = asunto;
            Host = "64.79.170.155";
            Puerto = 25;
            From = "odontoweb@dacasys.com";
        }

        #endregion
    }
}