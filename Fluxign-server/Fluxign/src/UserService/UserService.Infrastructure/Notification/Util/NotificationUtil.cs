using System.Net.Mail;
using System.Net;

namespace UserService.Infrastructure.Notification.Util
{
    public class NotificationUtil
    {
        public static int SendMail(string email, string msg, string subject, Attachment attachment = null)
        {
            if (email != null && !email.Equals(""))
            {
                try
                {

                    MailMessage obj = new MailMessage();
                    SmtpClient serverobj = new SmtpClient();
                    serverobj.Credentials = new NetworkCredential("notifications@done.ae", "Donedotae@789#");
                    serverobj.Port = 587;
                    serverobj.Host = "smtp.office365.com";
                    serverobj.EnableSsl = true;
                    obj = new MailMessage();
                    obj.From = new MailAddress("notifications@done.ae", "Done.ae", System.Text.Encoding.UTF8);
                    obj.To.Add(email);
                    obj.Priority = MailPriority.High;
                    obj.Subject = subject;
                    string date = DateTime.Now.ToString();
                    string mailto = "mailto:" + email;
                    string bodyMsg = msg;
                    bodyMsg = bodyMsg.Replace("{#MSG#}", msg + "");
                    obj.IsBodyHtml = true;
                    obj.Body = bodyMsg;
                    if (attachment != null)
                    {
                        obj.Attachments.Add(attachment);
                    }
                    serverobj.Send(obj);
                    return 1;
                }
                catch (Exception et)
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
