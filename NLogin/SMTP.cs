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

        public void Datos_Mensaje(string pFrom, string pTo, string pMessage, string pSubject)
        {
            From = pFrom;
            To = pTo;
            Message = pMessage;
            Subject = pSubject;
            Host = "64.79.170.155";
            Puerto = 25;
        }

        public void Datos_Mensaje(string pFrom, string pTo, string pMessage, string pSubject, string pHost, int pPuerto)
        {
            From = pFrom;
            To = pTo;
            Message = pMessage;
            Subject = pSubject;
            Host = pHost;
            Puerto = pPuerto;
        }

        public int Enviar_Mail()
        {
            Email = new System.Net.Mail.MailMessage(From, To, Subject, Message);
            System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient();
            Email.Priority = MailPriority.Normal;
            Email.IsBodyHtml = false;

            smtpMail.EnableSsl = false;
            smtpMail.Port = Puerto;
            smtpMail.Host = Host;
          //  smtpMail.Credentials = new System.Net.NetworkCredential("mediweb@dacasys.com", "D4c4sys20161!");
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

        public void Datos_Mensaje(string pTo, string pMensaje, string pAsunto)
        {
            To = pTo;
            Message = pMensaje;
            Subject = pAsunto;
            Host = "64.79.170.155";
            Puerto = 25;
            From = "mwediweb@dacasys.com";
        }

        #endregion
    }
}