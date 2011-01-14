using System.Collections.Generic;

namespace Yabe.Mailer
{
    public interface IMail
    {
        /* Mail server settings */
        string Host { get; set; }
        int Port { get; set; }
        int Timeout { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool EnableSsl { get; set; }
        string Subject { get; set; }
        Yabe.Mailer.MailPriority Priority { get; set; }
        
        /* Sender */
        void SetFrom(string emailAddress, string displayName);
        
        /* Recipients */
        void AddRecipient(MailRecipientType recipientType, string emailAddress, string displayName);
        void RemoveRecipient(MailRecipientType recipientType, string emailAddress);
        void RemoveAllRecipients(MailRecipientType recipientType);
        int GetRecipientCount(MailRecipientType recipientType);
        List<MailRecipient> GetRecipients(MailRecipientType recipientType);
        
        /* Header */
        //public abstract void AddHeader(string headerName, string headerValue);
        //public abstract void RemoveHeader(string headerName);
        //public abstract Dictionary<string, string> GetHeaders();

        /* Content */
        void SetPlaintextBody(string plaintextBody);
        void SetHtmlBody(string htmlBody);
        void AddEmbeddedResource(string filename, string contentId);

        /* Attachments */
        void AddAttachment(string filename);
        void RemoveAttachment(string filename);
        int GetAttachmentCount();
        
        /* Additional settings */
        
        /* Submission */
        void Send();
    }
}
