using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A Thread Safe Dictionary that can be indexed by Key or Value.
/// </summary>
/// <typeparam name="TKey">The type of key.</typeparam>
/// <typeparam name="TValue">The type of value.</typeparam>
/// <remarks>
/// TKey and TValue cannot be the same type, otherwise the collection becomes ambiguous.
/// </remarks>
public class BijectiveConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _forward = new();
    private readonly Dictionary<TValue, TKey> _reverse = new();
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    static BijectiveConcurrentDictionary()
    {
        if (typeof(TKey) == typeof(TValue))
        {
            throw new ArgumentException($"Invalid Arguments. A Bijective Concurrent Dictionary cannot have the same type for both Key and Value. Key: {typeof(TKey)}, Value: {typeof(TValue)}.");
        }
    }

    /// <summary>
    /// Get the <typeparamref name="TValue"/> associated with this <typeparamref name="TKey"/>.
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <returns>The value associated with the key.</returns>
    public TValue this[TKey key]
    {
        get 
        { 
            _lock.EnterReadLock();
            try
            {
                return _forward[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        set 
        { 
            AddOrUpdate(key, value); 
        }
    }

    /// <summary>
    /// Get the <typeparamref name="TKey"/> associated with this <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="val">The value to find.</param>
    /// <returns>The key associated with the value.</returns>
    public TKey this[TValue val]
    {
        get 
        {
            _lock.EnterReadLock();
            try
            {
                return _reverse[val];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        set 
        { 
            AddOrUpdate(value, val); 
        }
    }

    /// <summary>
    /// The collection of keys for this dictionary.
    /// </summary>
    public ICollection<TKey> Keys
    {
        get 
        { 
            _lock.EnterReadLock();
            try
            {
                return _forward.Keys;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// The collection of values for this dictionary.
    /// </summary>
    public ICollection<TValue> Values 
    { 
        get 
        {
            _lock.EnterReadLock();
            try
            {
                return _forward.Values;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    } 

    /// <summary>
    /// The number of pairs in this dictionary.
    /// </summary>
    public int Count
    {
        get 
        {
            _lock.EnterReadLock();
            try
            {
                return _forward.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Is this dictionary readonly?
    /// </summary>
    public bool IsReadOnly
    {
        get 
        { 
            return false; 
        }
    }
    

    /// <summary>
    /// Add a <typeparamref name="TKey"/> and <typeparamref name="TValue"/> pair to the dictionary.
    /// </summary>
    /// <param name="key">The key to be added.</param>
    /// <param name="value">The value to be associated.</param>
    /// <exception cref="ArgumentException">Both key and value must be unique.</exception>
    public void Add(TKey key, TValue value)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_forward.ContainsKey(key))
                throw new ArgumentException("Duplicate key.", nameof(key));
            if (_reverse.ContainsKey(value))
                throw new ArgumentException("Duplicate value.", nameof(value));

            _forward.Add(key, value);
            _reverse.Add(value, key);
        }
        finally 
        { 
            _lock.ExitWriteLock(); 
        }
    }

    private void AddOrUpdate(TKey key, TValue value)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_forward.TryGetValue(key, out var oldValue))
            {
                _reverse.Remove(oldValue);
            }

            if (_reverse.TryGetValue(value, out var oldKey))
            {
                _forward.Remove(oldKey);
            }

            _forward[key] = value;
            _reverse[value] = key;
        }
        finally
        { 
            _lock.ExitWriteLock(); 
        }
    }

    /// <summary>
    /// Does this dictionary contain this key?
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <returns>True if the dictionary contains this key, false otherwise.</returns>
    public bool ContainsKey(TKey key)
    {
        _lock.EnterReadLock();
        try
        {
            return _forward.ContainsKey(key);
        }
        finally
        { 
            _lock.ExitReadLock(); 
        }
    }

    /// <summary>
    /// Does this dictionary contain this value?
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <returns>True if the dictionary contains this value, false otherwise.</returns>
    public bool ContainsValue(TValue value)
    {
        _lock.EnterReadLock();
        try
        {
            return _reverse.ContainsKey(value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Does this dictionary contain this key?
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <returns>True if the dictionary contains this key, false otherwise.</returns>
    public bool Contains(TKey key)
    {
        return ContainsKey(key);
    }

    /// <summary>
    /// Does this dictionary contain this value?
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <returns>True if the dictionary contains this value, false otherwise.</returns>
    public bool Contains(TValue value)
    {
        return ContainsValue(value);
    }

    /// <summary>
    /// Try to get the value associated with the key.
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <param name="value">The variable to output the value into.</param>
    /// <returns>True if the dictionary contains this key, false otherwise.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        _lock.EnterReadLock();
        try
        {
            return _forward.TryGetValue(key, out value!);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Try to get the key associated with the value.
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <param name="key">The variable to output the key into.</param>
    /// <returns>True if the dictionary contains this value, false otherwise.</returns>
    public bool TryGetKey(TValue value, out TKey key)
    {
        _lock.EnterReadLock();
        try
        {
            return _reverse.TryGetValue(value, out key!);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Remove the key and its value from the dictionary.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if successful, false otherwise. Also returns false if the key does not exist.</returns>
    public bool Remove(TKey key)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_forward.TryGetValue(key, out var value))
                return false;
            _forward.Remove(key);
            _reverse.Remove(value);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Remove the value and its key from the dictionary.
    /// </summary>
    /// <param name="value">The value to remove.</param>
    /// <returns>True if successful, false otherwise. Also returns false if the value does not exist.</returns>
    public bool Remove(TValue value)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_reverse.TryGetValue(value, out var key))
                return false;
            _reverse.Remove(value);
            _forward.Remove(key);
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    /// <summary>
    /// Remove all entries from this dictionary.
    /// </summary>
    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _forward.Clear();
            _reverse.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        _lock.EnterReadLock();
        try
        {
            return new List<KeyValuePair<TKey, TValue>>(_forward).GetEnumerator();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Add the KeyValue pair to the dictionary.
    /// </summary>
    /// <param name="item">The KeyValuePair to add.</param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Check if the dictionary contains a KeyValuePair.
    /// </summary>
    /// <param name="item">The KeyValuePair to find.</param>
    /// <returns>True if the KeyValuePair exists, false otherwise.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        _lock.EnterReadLock();
        try
        {
            return _forward.TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        finally
        { 
            _lock.ExitReadLock(); 
        }
    }

    /// <summary>
    /// Copy the contents of the dictionary to an array of KeyValuePairs, starting at the specified index.
    /// </summary>
    /// <param name="array">The array to copy into.</param>
    /// <param name="arrayIndex">The index to start copying to.</param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _lock.EnterReadLock();
        try
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_forward).CopyTo(array, arrayIndex);
        }
        finally
        { 
            _lock.ExitReadLock(); 
        }
    }

    /// <summary>
    /// Remove a KeyValuePair from the dictionary.
    /// </summary>
    /// <param name="item">The KeyValuePair to remove.</param>
    /// <returns>True if the KeyValuePair was successfully removed, false otherwise. Also returns false if the KeyValuePair does not exist.</returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    { 
        return Remove(item.Key);
    }
}
