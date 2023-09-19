using FellowOakDicom;

namespace DicomExample.Services
{
    public class BasicStudyReaderStrategy : IStudyReaderStrategy
    {
        // Эта вещь здесь находиться по-хорошему не должна, но она инициализируется всего один раз и GC ее потом не трогает.
        private readonly static List<string> AllowedTagNames = new List<string>()
        {
            "Study Instance UID"
        };
        public Dictionary<string, string> GetValues(DicomDataset dataset)
        {
            // Не просто так я не вызывал ToList у dataset-а. Тегов и прочего > 100 (на example файле) + бинарное представление, как я понял, данных, здесь лучше потоком обрабатывать.
            var result = new Dictionary<string, string>();
            foreach(var i in dataset)
            {
                if (AllowedTagNames.Contains(i.Tag.DictionaryEntry.Name))
                    result.Add(i.Tag.DictionaryEntry.Name, dataset.GetString(i.Tag));
            }
            return result;
        }
    }
}
