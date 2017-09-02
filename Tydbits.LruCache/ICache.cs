namespace Tydbits.LruCache
{
    public interface ICache
    {
        int Size { get; }
        void Set(string key, object value);
        object Get(string key);
    }
}