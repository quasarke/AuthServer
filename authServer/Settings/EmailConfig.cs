namespace AuthServer.Settings
{
    public class EmailConfig
    {
        public string smtpHost { get; set; }
        public string smtpUser { get; set; }
        public string smtpPassword { get; set; }
        public string messageFrom { get; set;}
        public string messageFromDisplay { get; set; }
        public string VerifyEmailTemplate { get; set; }
        public string UserNameTemplate { get; set; }
        public string PasswordResetTemplate { get; set; }
        public string VerifyEmailWhyUrl { get; set; }
        public string VerifyEmailUrl { get; set; }
        public string PasswordResetUrl { get; set; }
    }
}

