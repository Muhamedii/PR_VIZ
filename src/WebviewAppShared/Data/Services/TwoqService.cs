using System;
using System.Collections.Generic;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class TwoqService : PageReplacementParameters
    {
        private LinkedList<int> Items { get; set; }
        private Dictionary<int, LinkedListNode<int>> PageAcceses { get; set; }
        private int ComplementaryFrameSize = 1;
        private int _pageFaults = 0;
        private int _pageHits = 0;


        public (bool hasHit, bool hasFault) Replace(int page)
        {
            bool hasHit = false;
            bool hasFault = false;

            if (PageAcceses.ContainsKey(page))
            {
                var node = PageAcceses[page];
                Items.Remove(node);
                Items.AddFirst(node);
                hasHit = true;
            }
            else
            {
                if (Items.Count >= FrameSize)
                {
                    var last = Items.Last;
                    PageAcceses.Remove(last.Value);
                    Items.RemoveLast();
                    hasFault = true;
                }
                Items.AddFirst(page);
                PageAcceses[page] = Items.First;
            }

            return (hasHit, hasFault);
        }

        public int Get()
        {
            if (Items.Count == 0)
                return default;

            var node = Items.Last;
            if (ComplementaryFrameSize > 0 && Items.Count > ComplementaryFrameSize)
            {
                while (node != null && --ComplementaryFrameSize > 0)
                    node = node.Previous;
                if (node == null)
                    node = Items.Last;
            }
            PageAcceses.Remove(node.Value);
            Items.Remove(node);
            return node.Value;
        }

        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new();
                PageAcceses = new();
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
