using System;
using System.Collections.Generic;
using System.Text;

namespace Yabe.Mailer
{
    public class MailDeliveryException : Exception
    {
        public MailDeliveryException() 
            : base()
        {
        }

        public MailDeliveryException(string message) 
            : base(message)
        {
        }

        public MailDeliveryException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }   
    }
}
