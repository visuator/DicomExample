namespace DicomSaver.Services
{
    public interface IDicomStorage
    {
        Task SaveFromJson(string json, CancellationToken token = default);
    }
}
