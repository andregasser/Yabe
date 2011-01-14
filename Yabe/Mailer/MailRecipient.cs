namespace Yabe.Mailer
{
    public class MailRecipient
    {
        public string MailAddress { get; set; }
        public string DisplayName { get; set; }

        public MailRecipient(string mailAddress, string displayName)
        {
            this.MailAddress = mailAddress;
            this.DisplayName = displayName;
        }
    }
}
