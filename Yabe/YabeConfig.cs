using System;
using System.Collections.Generic;
using System.Text;

namespace Yabe
{
    class YabeConfig
    {
        /* Host settings */
        public string hostName { get; set; }

        /* SMTP settings */
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }

        /* Mail message settings */
        public string successMailSenderName { get; set; }
        public string successMailSenderEmail { get; set; }
        public string successMailSubject { get; set; }
        public string successMailBody { get; set; }
        public Dictionary<string, string> successMailRecipients { get; set; }
        public string errorMailSenderName { get; set; }
        public string errorMailSenderEmail { get; set; }
        public string errorMailSubject { get; set; }
        public string errorMailBody { get; set; }
        public Dictionary<string, string> errorMailRecipients { get; set; }

        /* Actions */
        public List<YabeAction> actions { get; set; }
    }
}
