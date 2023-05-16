using System.Linq;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class PageReplacementService
    {
        private readonly LifoService _lifoService;

        public PageReplacementService()
        {
            _lifoService = new();
        }

        public Result GetGeneratedResult(RequestPayload requestPayload)
        {
            var selectedAlgorithm = requestPayload.Payload.FirstOrDefault().AlgorithmType;
            return selectedAlgorithm switch
            {
                AlgorithmType.LIFO => _lifoService.GetResult(requestPayload),
                _ => null,
            };
        }
    }
}
