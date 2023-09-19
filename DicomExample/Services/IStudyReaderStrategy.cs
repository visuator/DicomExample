using FellowOakDicom;

namespace DicomExample.Services
{
    public interface IStudyReaderStrategy
    {
        Dictionary<string, string> GetValues(DicomDataset dataset);
    }
}
