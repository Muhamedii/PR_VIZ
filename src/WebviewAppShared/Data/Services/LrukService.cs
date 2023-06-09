using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class LrukService : PageReplacementParameters
    {
        public List<(List<int>, bool isPageFound)> ItemsInFrameState { get; set; } = new();
        private List<int> Items { get; set; }
        private Dictionary<int, int> PageAccesses { get; set; }

        private (bool hasHit, bool hasFault) Replace(int page)
        {
            bool hasHit = false;
            bool hasFault = false;
            if (PageAccesses.ContainsKey(page))
            {
                hasHit = true;
                PageAccesses[page]++;
                Items.Remove(page);
                Items.Add(page);
                ItemsInFrameState.Add((Items.ToList(), isPageFound: true));
            }
            else
            {
                hasFault = true;
                PageAccesses[page] = 1;

                if (Items.Count >= FrameSize)
                {
                    var leastAccessedPage = GetTheLeastAccessedPage();
                    PageAccesses.Remove(leastAccessedPage);
                    Items.Remove(leastAccessedPage);
                    ItemsInFrameState.Add((Items.ToList(), isPageFound: false));
                }
                Items.Add(page);
            }

            return (hasHit, hasFault);
        }
        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new List<int>();
                FrameSize = frameSizeOption.FrameSize;
                PageAccesses = new();

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
                result.FrameState = ItemsInFrameState;
            }
            return result;
        }
        private int GetTheLeastAccessedPage()
        {
           return Items.OrderBy(page => PageAccesses.GetValueOrDefault(page))
                 .ThenByDescending(Items.IndexOf)
                 .First();
        }
    }

}
