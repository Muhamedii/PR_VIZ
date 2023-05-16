namespace WebviewAppShared.Data.Services
{
    public interface IPageReplacementAlgorithmsService<TKey,TValue>
    {
        public void Add(TValue value);
        public TValue Get(TKey key);
    }
}
