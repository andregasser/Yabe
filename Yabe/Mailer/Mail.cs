// TODO: Implement Mail Priority
// TODO: Implement ReplyTo
// TODO: Implement Headers
// TODO: Implement Sender (??) Difference to .From property?
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Yabe.Mailer
{
    /// <summary>
    /// Class which implements basic mail functionality.
    /// </summary>
    public class Mail : IMail
    {
        System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
        AlternateView plaintextView = null;
        AlternateView htmlView = null;
        private string host = "";
        private int port = 25;
        private string username = "";
        private string password = "";
        private int timeout = 30000;  // 30 seconds
        private bool ssl = false;
        private string subject = "";
        private Yabe.Mailer.MailPriority priority = Yabe.Mailer.MailPriority.Normal;

        #region Properties
        /// <summary>
        /// SMTP mail server.
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// SMTP mail server port.
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Timeout for SMTP connection in seconds.
        /// </summary>
        public int Timeout
        {
            get { return timeout / 1000; }
            set { timeout = value * 1000; }
        }

        /// <summary>
        /// SMTP username required for mail server connection.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// SMTP password required for mail server connection.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Specifies, if SSL should be used or not.
        /// </summary>
        public bool EnableSsl
        {
            get { return ssl; }
            set { ssl = value; }
        }

        /// <summary>
        /// Subject of the email message to be sent.
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public Yabe.Mailer.MailPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
        #endregion

        /// <summary>
        /// Mail constructor.
        /// </summary>
        #region Mail
        public Mail()
        {
        }
        #endregion

        /// <summary>
        /// Specify sender email address and full name.
        /// </summary>
        /// <param name="emailAddress">string</param>
        /// <param name="displayName">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if one of the parameters is empty.
        /// </exception>
        #region SetFrom
       public void SetFrom(string emailAddress, string displayName)
        {
            if (emailAddress == "")
            {
                throw new ArgumentOutOfRangeException("emailAddress parameter cannot be empty.");
            }

            if (displayName == "")
            {
                throw new ArgumentOutOfRangeException("displayName parameter cannot be empty.");
            }

            MailAddress mailAddress = new MailAddress(emailAddress, displayName);
            mailMessage.From = mailAddress;
        }
        #endregion

        /// <summary>
        /// Add a recipient to the current mail message.
        /// </summary>
        /// <param name="MailRecipientType">MailRecipientType</param>
        /// <param name="emailAddress">string</param>
        /// <param name="displayName">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if one of the parameters is empty.
        /// </exception>
        #region AddRecipient
        public void AddRecipient(MailRecipientType MailRecipientType, string emailAddress, string displayName)
        {
            if (MailRecipientType != MailRecipientType.To && MailRecipientType != MailRecipientType.Cc &&
                MailRecipientType != MailRecipientType.Bcc)
            {
                throw new ArgumentOutOfRangeException("MailRecipientType parameter is invalid.");
            }

            if (emailAddress == "")
            {
                throw new ArgumentOutOfRangeException("emailAddress parameter cannot be empty.");
            }

            if (displayName == "")
            {
                throw new ArgumentOutOfRangeException("displayName parameter cannot be empty.");
            }

            MailAddress mailAddress = new MailAddress(emailAddress, displayName);

            switch (MailRecipientType)
            {
                case MailRecipientType.To:
                    mailMessage.To.Add(mailAddress);
                    break;
                case MailRecipientType.Cc:
                    mailMessage.CC.Add(mailAddress);
                    break;
                case MailRecipientType.Bcc:
                    mailMessage.Bcc.Add(mailAddress);
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Remove a recipient from the current mail message.
        /// </summary>
        /// <param name="MailRecipientType">MailRecipientType</param>
        /// <param name="emailAddress">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if one of the parameters is empty.
        /// </exception>
        #region RemoveRecipient
        public void RemoveRecipient(MailRecipientType MailRecipientType, string emailAddress)
        {
            if (MailRecipientType != MailRecipientType.To && MailRecipientType != MailRecipientType.Cc &&
                MailRecipientType != MailRecipientType.Bcc)
            {
                throw new ArgumentOutOfRangeException("MailRecipientType parameter is invalid.");
            }

            if (emailAddress == "")
            {
                throw new ArgumentOutOfRangeException("emailAddress parameter cannot be empty.");
            }
            
            List<int> markers = new List<int>();

            switch (MailRecipientType)
            {
                case MailRecipientType.To:
                    // Check for addresses to remove
                    for (int i = 0; i < mailMessage.To.Count; i++)
                    {
                        if (mailMessage.To[i].Address == emailAddress)
                        {
                            markers.Add(i);
                        }
                    }

                    // Remove addresses
                    for (int i = 0; i < markers.Count; i++)
                    {
                        mailMessage.To.RemoveAt(i);
                    }
                    break;
                case MailRecipientType.Cc:
                    // Check for addresses to remove
                    for (int i = 0; i < mailMessage.CC.Count; i++)
                    {
                        if (mailMessage.CC[i].Address == emailAddress)
                        {
                            markers.Add(i);
                        }
                    }

                    // Remove addresses
                    for (int i = 0; i < markers.Count; i++)
                    {
                        mailMessage.CC.RemoveAt(i);
                    }
                    break;
                case MailRecipientType.Bcc:
                    // Check for addresses to remove
                    for (int i = 0; i < mailMessage.Bcc.Count; i++)
                    {
                        if (mailMessage.Bcc[i].Address == emailAddress)
                        {
                            markers.Add(i);
                        }
                    }

                    // Remove addresses
                    for (int i = 0; i < markers.Count; i++)
                    {
                        mailMessage.Bcc.RemoveAt(i);
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Remove all recipients from the current mail message.
        /// </summary>
        /// <param name="MailRecipientType">MailRecipientType</param>
        /// <param name="emailAddress">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if the parameter is empty.
        /// </exception>
        #region RemoveAllRecipients
        public void RemoveAllRecipients(MailRecipientType MailRecipientType)
        {
            if (MailRecipientType != MailRecipientType.To && MailRecipientType != MailRecipientType.Cc &&
                MailRecipientType != MailRecipientType.Bcc)
            {
                throw new ArgumentOutOfRangeException("MailRecipientType parameter is invalid.");
            }
            
            switch (MailRecipientType)
            {
                case MailRecipientType.To:
                    mailMessage.To.Clear();
                    break;
                case MailRecipientType.Cc:
                    mailMessage.CC.Clear();
                    break;
                case MailRecipientType.Bcc:
                    mailMessage.Bcc.Clear();
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Returns the number of recipients
        /// </summary>
        /// <param name="MailRecipientType">MailRecipientType</param>
        /// <returns>int</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if the parameter is empty.
        /// </exception>
        #region GetRecipientCount
        public int GetRecipientCount(MailRecipientType MailRecipientType)
        {
            if (MailRecipientType != MailRecipientType.To && MailRecipientType != MailRecipientType.Cc &&
                MailRecipientType != MailRecipientType.Bcc)
            {
                throw new ArgumentOutOfRangeException("MailRecipientType parameter is invalid.");
            }
            
            switch (MailRecipientType)
            {
                case MailRecipientType.To: return mailMessage.To.Count;
                case MailRecipientType.Cc: return mailMessage.CC.Count;
                case MailRecipientType.Bcc: return mailMessage.Bcc.Count;
                default: return 0;
            }
        }
        #endregion

        /// <summary>
        /// Get list of recipients.
        /// </summary>
        /// <param name="MailRecipientType">MailRecipientType</param>
        /// <returns>List<Recipient></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if the parameter is empty.
        /// </exception>
        #region GetRecipients
        public List<MailRecipient> GetRecipients(MailRecipientType MailRecipientType)
        {
            if (MailRecipientType != MailRecipientType.To && MailRecipientType != MailRecipientType.Cc &&
                MailRecipientType != MailRecipientType.Bcc)
            {
                throw new ArgumentOutOfRangeException("MailRecipientType parameter is invalid.");
            }
            
            List<MailRecipient> recipientsList = new List<MailRecipient>();

            switch (MailRecipientType)
            {
                case MailRecipientType.To:
                    for (int i = 0; i < mailMessage.To.Count; i++)
                    {
                        recipientsList.Add(new MailRecipient(mailMessage.To[i].Address, mailMessage.To[i].DisplayName));
                    }
                    break;
                case MailRecipientType.Cc:
                    for (int i = 0; i < mailMessage.CC.Count; i++)
                    {
                        recipientsList.Add(new MailRecipient(mailMessage.CC[i].Address, mailMessage.CC[i].DisplayName));
                    }
                    break;
                case MailRecipientType.Bcc:
                    for (int i = 0; i < mailMessage.Bcc.Count; i++)
                    {
                        recipientsList.Add(new MailRecipient(mailMessage.Bcc[i].Address, mailMessage.Bcc[i].DisplayName));
                    }
                    break;
            }

            return recipientsList;
        }
        #endregion

        /*
        public override void AddHeader(string headerName, string headerValue)
        {
            throw new NotImplementedException();
        }

        public override void RemoveHeader(string headerName)
        {
            throw new NotImplementedException();
        }

        public override List<Dictionary<string, string>> GetHeaders()
        {
            throw new NotImplementedException();
        }

        public override void SetContentType()
        {
            throw new NotImplementedException();
        }

        public override void SetBodyPlainText(string content)
        {
            throw new NotImplementedException();
        }

        public override void SetBodyHTML(string content)
        {
            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Set plaintext pody of current mail message.
        /// </summary>
        /// <param name="plaintextBody">string</param>
        #region SetPlaintextBody
        public void SetPlaintextBody(string plaintextBody)
        {
            plaintextView = (AlternateView)AlternateView.CreateAlternateViewFromString(
                plaintextBody,
                null,
                "text/plain"
            );
        }
        #endregion

        /// <summary>
        /// Set html body of current mail message.
        /// </summary>
        /// <param name="htmlBody">string</param>
        #region SetHtmlBody
        public void SetHtmlBody(string htmlBody)
        {
            htmlView = (AlternateView)AlternateView.CreateAlternateViewFromString(
                htmlBody,
                null,
                "text/html"
            );
        }
        #endregion

        /// <summary>
        /// Adds an embedded resource like an image or file
        /// to the current mail message. This resource can be used in 
        /// conjunction with html bodies.
        /// See http://www.systemnetmail.com for more details.
        /// </summary>
        /// <param name="filename">string</param>
        /// <param name="contentId">string</param>
        /// <exception cref="System.InvalidOperationException">
        /// This exception is thrown, if no html body has been defined before.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if one of the parameters is empty. 
        /// </exception>
        #region AddEmbeddedResource
        public void AddEmbeddedResource(string filename, string contentId)
        {
            if (filename == "")
            {
                throw new ArgumentOutOfRangeException("filename parameter cannot be empty.");
            }

            if (contentId == "")
            {
                throw new ArgumentOutOfRangeException("contentId parameter cannot be empty.");
            }
            
            if (htmlView != null)
            {
                LinkedResource res = new LinkedResource(filename);
                res.ContentId = contentId;
                htmlView.LinkedResources.Add(res);
            }
            else
            {
                throw new InvalidOperationException("Html body is not defined. Please define html body first.");
            }
        }
        #endregion

        /// <summary>
        /// Add attachment to attachment collection.
        /// </summary>
        /// <param name="filename">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if the parameter is empty.
        /// </exception>
        #region AddAttachment
        public void AddAttachment(string filename)
        {
            if (filename == "")
            {
                throw new ArgumentOutOfRangeException("filename parameter cannot be empty.");
            }

            Attachment attData = new Attachment(filename, MediaTypeNames.Application.Octet);
            mailMessage.Attachments.Add(attData);
        }
        #endregion

        /// <summary>
        /// Remove attachment from attachment collection.
        /// </summary>
        /// <param name="filename">string</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// This exception is thrown, if the parameter is empty.
        /// </exception>
        #region RemoveAttachment
        public void RemoveAttachment(string filename)
        {
            if (filename == "")
            {
                throw new ArgumentOutOfRangeException("filename parameter cannot be empty");
            }
            
            List<int> markers = new List<int>();
            
            // Check for attachments to remove
            for (int i = 0; i < mailMessage.Attachments.Count; i++)
            {
                if (mailMessage.Attachments[i].Name == filename)
                {
                    markers.Add(i);
                }
            }

            // Remove attachments
            for (int i = 0; i < markers.Count; i++)
            {
                mailMessage.Attachments.RemoveAt(i);
            }
        }
        #endregion

        /// <summary>
        /// Returns the number of attached files.
        /// </summary>
        /// <returns>int</returns>
        #region GetAttachmentCount
        public int GetAttachmentCount()
        {
            return mailMessage.Attachments.Count;
        }
        #endregion

        /// <summary>
        /// Send email message.
        /// </summary>
        /// <exception cref="Ispin.Irma.Core.Tools.MailDeliveryException">
        /// This exception is thrown, if the message could not be delivered to all
        /// recipients. Check the inner exception for details.
        /// </exception>
        /// <exception cref="Ispin.Irma.Core.Tools.MailSmtpException">
        /// This exception is thrown, if there was a problem sending the email.
        /// Check the inner exception for details.
        /// </exception>
        /// <exception cref="System.Exception">
        /// This exception is throw, if an unknown error occurred.
        /// </exception>
        #region Send
        public void Send()
        {
            // Assemble email
            mailMessage.Subject = subject;

            switch (priority)
            {
                case MailPriority.Normal:
                    mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
                    break;
                case MailPriority.Low:
                    mailMessage.Priority = System.Net.Mail.MailPriority.Low;
                    break;
                case MailPriority.High:
                    mailMessage.Priority = System.Net.Mail.MailPriority.High;
                    break;
            }

            if (plaintextView != null)
            {
                mailMessage.AlternateViews.Add(plaintextView);
            }
            if (htmlView != null)
            {
                mailMessage.AlternateViews.Add(htmlView);
            }

            // Send Email
            SmtpClient client = new SmtpClient();
            client.Host = this.host;
            client.Port = this.port;
            client.Timeout = this.timeout;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(this.username, this.password);
            if (this.ssl)
            {
                client.EnableSsl = true;
            }
            
            // Send email
            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                throw new MailDeliveryException("The message has not been delivered to all recipients.", ex);
            }
            catch (SmtpException ex)
            {
                throw new MailSmtpException("There was an error sending the email.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
