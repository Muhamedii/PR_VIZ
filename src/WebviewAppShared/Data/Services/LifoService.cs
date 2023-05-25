using System;
using System.Collections.Generic;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class LifoService : PageReplacementParameters
    {
        public Stack<int> Items { get; set; } = new();
        public void Add(int item)
        {
            Items.Push(item);
        }

        public void Replace(int item)
        {
            if (Items.Count >= FrameSize)
            {
                Items.Pop();
            }
            Items.Push(item);
        }

        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new Stack<int>();
                FrameSize = frameSizeOption.FrameSize;

                int faults = 0;
                int hits = 0;
                foreach (var page in frameSizeOption.Pages)
                {
                    if (!Items.Contains(page))
                    {
                        faults++;
                        Replace(page);
                    }
                    hits++;
                }
                result.FrameSizePageFailRatioData.Add(new Tuple<int, int>(faults, FrameSize));
                result.RatioSummary.TotalFaults = faults;
                result.RatioSummary.TotalHits = hits;
                result.RatioSummary.PageFaultRatio = (faults * 100) / frameSizeOption.Pages.Count;
            }
            return result;
        }
    }
}
