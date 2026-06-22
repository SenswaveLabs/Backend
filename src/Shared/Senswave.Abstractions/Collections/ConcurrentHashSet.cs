using System.Collections;
using System.Collections.Concurrent;

namespace Senswave.Abstractions.Collections;


public class ConcurrentHashSet<T> : ISet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dict;

    public ConcurrentHashSet()
    {
        _dict = new ConcurrentDictionary<T, byte>();
    }

    public ConcurrentHashSet(IEnumerable<T> collection)
    {
        _dict = new ConcurrentDictionary<T, byte>();
        foreach (var item in collection)
        {
            _dict.TryAdd(item, 0);
        }
    }

    public ConcurrentHashSet(IEqualityComparer<T> comparer)
    {
        _dict = new ConcurrentDictionary<T, byte>(comparer);
    }

    public bool Add(T item) => _dict.TryAdd(item, 0);

    void ICollection<T>.Add(T item) => Add(item); // explicit implementation for ISet<T>

    public bool Remove(T item) => _dict.TryRemove(item, out _);

    public bool Contains(T item) => _dict.ContainsKey(item);

    public void Clear() => _dict.Clear();

    public int Count => _dict.Count;

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => _dict.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(T[] array, int arrayIndex) => _dict.Keys.CopyTo(array, arrayIndex);

    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            Add(item);
        }
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        foreach (var item in this.ToArray())
        {
            if (!set.Contains(item))
                Remove(item);
        }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            Remove(item);
        }
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        foreach (var item in set)
        {
            if (!Remove(item))
                Add(item);
        }
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        return this.All(set.Contains);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return other.All(Contains);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        if (Count <= set.Count) return false;
        return set.All(Contains);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        if (Count >= set.Count) return false;
        return this.All(set.Contains);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return other.Any(Contains);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        var set = new HashSet<T>(other, _dict.Comparer);
        if (Count != set.Count) return false;
        return this.All(set.Contains);
    }
}