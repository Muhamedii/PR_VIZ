using System;
using System.Collections.Generic;
using System.Linq;
using WebviewAppShared.Data.Models;
using WebviewAppShared.Data.Models.Lru;

namespace WebviewAppShared.Data.Services
{
    public class LruService : PageReplacementParameters
    {
        public List<(List<int>, bool isPageFound)> ItemsInFrameState { get; set; } = new();
        private Dictionary<int, LinkedListNode<Item>> Items = new();
        private LinkedList<Item> LrulinkedList = new();

        private void Remove()
        {
            var linkedListNode = LrulinkedList.First;

            if (linkedListNode != null)
            {
                Items.Remove(linkedListNode.Value.Key);
                LrulinkedList.RemoveFirst();
            }
        }
        private void ReArrange(LinkedListNode<Item> linkedListNode)
        {
            LrulinkedList.Remove(linkedListNode);
            LrulinkedList.AddLast(linkedListNode);
        }
        public (bool hasHit, bool hasFault) Replace(int key, int value)
        {
            bool hit = default;
            bool fault = default;

            if (Items.TryGetValue(key, out var linkedListNode))
            {
                linkedListNode.Value.Value = value;
                ReArrange(linkedListNode);

                hit = true;
                ItemsInFrameState.Add((Items.Select(x => x.Key).ToList(), isPageFound: true));
            }
            else
            {
                fault = true;

                if (Items.Count >= FrameSize)
                {
                    Remove();
                }

                var newItem = new Item { Key = key, Value = value };
                var newLinkedListNode = new LinkedListNode<Item>(newItem);
                LrulinkedList.AddLast(newLinkedListNode);
                Items[key] = newLinkedListNode;
                ItemsInFrameState.Add((Items.Select(x=>x.Key).ToList(), isPageFound: false));


            }
            return (hit, fault);
        }
        public Result GetResult(RequestPayload inputData)
        {
            Result result = new() { FrameSizePageFailRatioData = new(), RatioSummary = new() };
            foreach (var frameSizeOption in inputData.Payload)
            {
                Items = new Dictionary<int, LinkedListNode<Item>>();
                LrulinkedList = new();
                FrameSize = frameSizeOption.FrameSize;

                int faults = 0;
                int hits = 0;

                foreach (var page in frameSizeOption.Pages)
                {
                    var (hasHit, hasFault) = Replace(page, page);

                    if (hasFault)
                        faults++;
                    if (hasHit)
                        hits++;
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
