using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AutoRingUAB
{
    public interface IEsapiService
    {
        Task<StructSet[]> GetStructureSetsAsync();
        Task<Struct[]> GetStructureIdsAsync(string structureSet, string keyword);
        Task<string> GetEditableRingNameAsync(string structureSetId, string ringId);
        Task AddRingAsync(string structureSetId, ObservableCollection<string> ptvIds, string ringInnerId, string ringMiddleId, string ringOuterId);
    }
}
