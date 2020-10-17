using System.Threading.Tasks;

namespace ForexMiner.Heimdallr.Instruments.Worker.Services.History
{
    public interface IHistoryService
    {
        public Task CheckAndFillToday();

        public Task CheckAndFillUntilYesterday();
    }
}
