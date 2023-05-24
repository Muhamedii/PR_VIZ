using System;
using System.Collections.Generic;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class RprService : PageReplacementParameters
    {
        public List<int> Items { get; set; }
        private Random Randomizer;
        public (bool hasHit, bool hasFault) Replace(int page)
        {
            bool hasHit = false;
            bool hasFault = false;

            if (Items.Contains(page))
            {
                hasHit = true;
            }
            else
            {
                hasFault = true;
                if (Items.Count >= FrameSize)
                {
                    int randomIndex = Randomizer.Next(FrameSize);
                    Items[randomIndex] = page;
                }
                else
                {
                    Items.Add(page);
                }
            }
            return (hasHit, hasFault);
        }
        public Result GetResult(RequestPayload inputData)
        {
            Randomizer = new();
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new List<int>();
                FrameSize = frameSizeOption.FrameSize;

                int faults = 0;
                int hits = 0;
                foreach (var page in frameSizeOption.Pages)
                {
                    var (hasHit, hasFault) = Replace(page);

                    if (hasHit)
                        hits++;
                    if (hasFault)
                        faults++;
                }
                result.FrameSizePageFailRatioData.Add(new Tuple<int, int>(faults, FrameSize));
                result.RatioSummary.TotalFaults = faults;
                result.RatioSummary.TotalHits = hits;
            }
            return result;
        }
    }
}
