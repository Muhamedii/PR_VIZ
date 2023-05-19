using System.Linq;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class PageReplacementService
    {
        private readonly LifoService _lifoService;
        private readonly FifoService _fifoService;
        private readonly LruService _lruService;

        public PageReplacementService()
        {
            _lifoService = new();
            _fifoService = new();
            _lruService = new();
        }

        public Result GetGeneratedResult(RequestPayload requestPayload)
        {
            var selectedAlgorithm = requestPayload.Payload.FirstOrDefault().AlgorithmType;
            return selectedAlgorithm switch
            {
                AlgorithmType.LIFO => _lifoService.GetResult(requestPayload),
                AlgorithmType.FIFO => _fifoService.GetResult(requestPayload),
                AlgorithmType.LRU => _lruService.GetResult(requestPayload),
                _ => null,
            };
        }
    }
}
