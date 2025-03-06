namespace BlueCorp.Integration.D365To3PL.Services.Interfaces
{
    public interface ISftpService
    {
        Task UploadFileAsync(byte[] fileData, string fileName);
    }
}
