using System;
using System.Collections.Generic;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class CprService : PageReplacementParameters
    {
        public HashSet<int> Items { get; set; }

        public (int faults, int hits) Replace(List<int> referenceString, int frameSize)
        {
            int pageFaults = 0;
            int pageHits = 0;
            int currentIndex = 0;

            foreach (int pageNumber in referenceString)
            {
                if (!Items.Contains(pageNumber))
                {
                    if (Items.Count == frameSize)
                    {
                        while (true)
                        {
                            int page = referenceString[currentIndex];
                            if (!Items.Contains(page))
                            {
                                Items.Remove(page);
                                break;
                            }
                            currentIndex = (currentIndex + 1) % referenceString.Count;
                        }
                    }
                    Items.Add(pageNumber);
                    pageFaults++;
                }
                else
                {
                    pageHits++;
                }
            }

            return (pageFaults, pageHits);
        }

        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new HashSet<int>();
                FrameSize = frameSizeOption.FrameSize;

                var (faults, hits) = Replace(frameSizeOption.Pages, frameSizeOption.FrameSize);

                result.FrameSizePageFailRatioData.Add(new Tuple<int, int>(faults, FrameSize));
                result.RatioSummary.TotalFaults = faults;
                result.RatioSummary.TotalHits = hits;
            }
            return result;
        }
    }
}
