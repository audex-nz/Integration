using BlueCorp.Integration.D365To3PL.Models;

namespace BlueCorp.Integration.D365To3PL.Services.Interfaces
{
    public interface IDataProcessingService
    {
        Task ProcessD365EventAsync(DispatchEvent dispatchEvent);
    }
}
