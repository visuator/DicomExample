namespace DicomExample.Services
{
    public interface IStudyService
    {
        Task Save(Stream stream, CancellationToken token = default);
    }
}
