using System.Linq;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class PageReplacementService
    {
        private readonly LifoService _lifoService;
        private readonly FifoService _fifoService;
        private readonly LruService _lruService;
        private readonly CprService _cprService;
        private readonly NruService _nruService;
        private readonly RprService _rprService;
        private readonly LrukService _lrukService;

        public PageReplacementService()
        {
            _lifoService = new();
            _fifoService = new();
            _lruService = new();
            _cprService = new();
            _nruService = new();
            _rprService = new();
            _lrukService = new();
        }

        public Result GetGeneratedResult(RequestPayload requestPayload)
        {
            var selectedAlgorithm = requestPayload.Payload.FirstOrDefault().AlgorithmType;
            return selectedAlgorithm switch
            {
                AlgorithmType.LIFO => _lifoService.GetResult(requestPayload),
                AlgorithmType.FIFO => _fifoService.GetResult(requestPayload),
                AlgorithmType.LRU => _lruService.GetResult(requestPayload),
                AlgorithmType.CPR => _cprService.GetResult(requestPayload),
                AlgorithmType.NRU => _nruService.GetResult(requestPayload),
                AlgorithmType.RPR => _rprService.GetResult(requestPayload),
                AlgorithmType.LRUK => _lrukService.GetResult(requestPayload),
                _ => null,
            };
        }
    }
}
