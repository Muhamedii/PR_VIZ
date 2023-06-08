using System.Collections.Generic;
using System;
using WebviewAppShared.Data.Models;
using System.Linq;

namespace WebviewAppShared.Data.Services
{
    public class FifoService : PageReplacementParameters
    {
        public List<(List<int>, bool isPageFound)> ItemsInFrameState { get; set; } = new();
        public Queue<int> Items { get; set; } = new();

        public void Replace(int item)
        {
            if (Items.Count >= FrameSize)
            {
                Items.Dequeue();
            }
            Items.Enqueue(item);
        }

        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new Queue<int>();
                FrameSize = frameSizeOption.FrameSize;

                int faults = 0;
                int hits = 0;
                foreach (var page in frameSizeOption.Pages)
                {
                    if (!Items.Contains(page))
                    {
                        faults++;
                        Replace(page);
                        ItemsInFrameState.Add((Items.ToList(), isPageFound: false));
                    }
                    else
                    {
                        hits++;
                        ItemsInFrameState.Add((Items.ToList(), isPageFound: true));
                    }
                        
                }
                result.FrameSizePageFailRatioData.Add(new Tuple<int, int>(faults, FrameSize));
                result.RatioSummary.TotalFaults = faults;
                result.RatioSummary.TotalHits = hits;
                result.FrameState = ItemsInFrameState;
            }
            return result;
        }
    }
}
