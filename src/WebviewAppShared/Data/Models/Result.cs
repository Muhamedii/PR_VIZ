using System;
using System.Collections.Generic;

namespace WebviewAppShared.Data.Models
{
    public class Result
    {
        /// <summary>
        /// Represents chart data X and Y axis.
        /// Key (X axis): TotalHits per given frame size
        /// Value (Y axis): FrameSize selected
        /// </summary>
        public List<Tuple<int,int>> FrameSizePageFailRatioData { get; set; }
        public PageReplacementResult RatioSummary { get; set; }
    }
}
