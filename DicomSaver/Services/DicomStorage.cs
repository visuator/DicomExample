using DicomSaver.Entities;

using System.Text.Json;

namespace DicomSaver.Services
{
    public class DicomStorage : IDicomStorage
    {
        private readonly DicomDbContext _dbContext;
        private readonly ILogger<DicomStorage> _logger;

        public DicomStorage(DicomDbContext dbContext, ILogger<DicomStorage> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SaveFromJson(string json, CancellationToken token = default)
        {
            var dicom = new DicomSnapshot() { Snapshot = JsonDocument.Parse(json) };
            await _dbContext.Snapshots.AddAsync(dicom, token);
            await _dbContext.SaveChangesAsync(token);
            _logger.LogInformation("Saved snapshot: {0}", dicom.Id);
        }
    }
}
