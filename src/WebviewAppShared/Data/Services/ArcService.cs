using System;
using System.Collections.Generic;
using WebviewAppShared.Data.Models;

namespace WebviewAppShared.Data.Services
{
    public class ArcService : PageReplacementParameters
    {
        private Dictionary<int, int> Items;
        private LinkedList<int> FrequentlyAccessedItemsListOne;
        private LinkedList<int> FrequentlyAccessedItemsListTwo;
        private LinkedList<int> InfrequentlyAccessedItemsListOne;
        private LinkedList<int> InfrequentylAccessedItemsListTwo;

        public (bool hasHit, bool hasFault) Replace(int key)
        {
            bool hasHit = false;
            bool hasFault = false;

            if (Items.ContainsKey(key))
            {
                MoveToTop(ref FrequentlyAccessedItemsListTwo, key);
                hasHit = true;
            }
            else
            {
                var list = (InfrequentlyAccessedItemsListOne.Contains(key) || InfrequentylAccessedItemsListTwo.Contains(key)) ? InfrequentlyAccessedItemsListOne : FrequentlyAccessedItemsListOne;
                if (list.Contains(key))
                {
                    MoveToTop(ref FrequentlyAccessedItemsListTwo, key);
                    hasFault = true;
                }
                else
                {
                    hasFault = true;
                    EvictIfNeeded();
                    AddToTop(ref FrequentlyAccessedItemsListOne, key);
                }
            }

            return (hasHit, hasFault);
        }

        public void Add(int key, int value)
        {
            if (Items.ContainsKey(key))
            {
                Items[key] = value;
                MoveToTop(ref FrequentlyAccessedItemsListTwo, key);
            }
            else
            {
                if (FrequentlyAccessedItemsListOne.Count >= FrameSize)
                {
                    var lruList = (FrequentlyAccessedItemsListOne.Count == FrameSize) ? FrequentlyAccessedItemsListOne : InfrequentlyAccessedItemsListOne;
                    RemoveFromCache(lruList.Last.Value);
                    AddToTop(ref InfrequentylAccessedItemsListTwo, key);
                }
                else
                {
                    AddToTop(ref FrequentlyAccessedItemsListOne, key);
                }
                Items[key] = value;
            }
        }

        private static void MoveToTop(ref LinkedList<int> list, int key)
        {
            list.Remove(key);
            list.AddFirst(key);
        }

        private static void AddToTop(ref LinkedList<int> list, int key)
        {
            list.AddFirst(key);
        }

        private void RemoveFromCache(int key)
        {
            Items.Remove(key);

            FrequentlyAccessedItemsListOne.Remove(key);
            FrequentlyAccessedItemsListTwo.Remove(key);
            InfrequentlyAccessedItemsListOne.Remove(key);
            InfrequentylAccessedItemsListTwo.Remove(key);
        }

        private void EvictIfNeeded()
        {
            if (FrequentlyAccessedItemsListOne.Count >= FrameSize)
            {
                RemoveFromCache(FrequentlyAccessedItemsListOne.Last.Value);
                AddToTop(ref InfrequentlyAccessedItemsListOne, FrequentlyAccessedItemsListOne.Last.Value);
            }
        }

        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new();
                FrequentlyAccessedItemsListOne = new();
                FrequentlyAccessedItemsListTwo = new();
                InfrequentlyAccessedItemsListOne = new();
                InfrequentylAccessedItemsListTwo = new();
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
