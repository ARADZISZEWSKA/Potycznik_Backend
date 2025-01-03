using Potycznik_Backend.Models;

namespace Potycznik_Backend.DTo
{
    public class EndInventoryRequestDto
    {
        public List<InventoryRecordRequest> InventoryRecords { get; set; }
        public List<int> ProductsToDelete { get; set; }
    }
}
