using System.Collections.Generic;

namespace WebviewAppShared.Data.Models
{
    public class PageReplacementParameters
    {
        public List<int> Pages { get; set; }
        public int FrameSize { get; set; }
        public AlgorithmType AlgorithmType { get; set; }
    }
}
