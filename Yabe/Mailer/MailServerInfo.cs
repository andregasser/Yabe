namespace Yabe.Mailer
{
    public class MailServerInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EnableSsl { get; set; }
    }
}
