using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Text.Json;

namespace DicomSaver.Entities
{
    public class DicomSnapshot : BaseEntity, IEntityTypeConfiguration<DicomSnapshot> //, ISingleKeyEntity etc. Я делал так, удобно если нужно, например, опрокидывать только айдишник, они же могут быть и составные)
    {
        public Guid Id { get; set; }
        public JsonDocument Snapshot { get; set; }

        public void Configure(EntityTypeBuilder<DicomSnapshot> builder)
        {
            // Вообще момент спорный, мб есть способы смотреть на BaseEntity. Можно замутить рефлексией.
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
