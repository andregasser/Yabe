using System;
using System.Collections.Generic;
using System.Text;

namespace Yabe.Mailer
{
    public class MailSmtpException : Exception
    {
        public MailSmtpException()
            : base()
        {
        }

        public MailSmtpException(string message)
            : base(message)
        {
        }

        public MailSmtpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
