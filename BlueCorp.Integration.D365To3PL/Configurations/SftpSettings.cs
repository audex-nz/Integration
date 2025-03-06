namespace BlueCorp.Integration.D365To3PL.Configurations
{
    public class SftpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string IncomingDirectory { get; set; }
        public string PrivateKeySecretName { get; set; }
    }
}
