using System.Collections.Generic;

namespace WebviewAppShared.Data.Models
{
    public class RequestPayload
    {
        public List<PageReplacementParameters> Payload { get; set; } = new();
    }
}
