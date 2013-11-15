using System;
using System.Collections.Generic;
using System.Linq;

namespace Escape
{
    // This better be something...
    // Some sort of abomination between a Dictionary and a List.
    // Basically a wrapper so we don't need all those "GetSomethingByID" and "GetSomethingByName" methods,
    // and neither need to do all those conversions.
    //
    // This collection may contain it all.
    /// <summary>
    /// A collection wrapper to store values with string and ID keys.
    /// </summary>
    /// <typeparam name="TEntry">Type of the value</typeparam>
    public class EntryDatabase<TEntry>
    {
        #region Declarations
        private List<TEntry> _BackingList; // Storage for elements
        private Dictionary<string, int> _Index; // Link between the key and the value
        #endregion

        #region Constructor
        /// <summary>
        /// Set up a new EntryDatabase with the specified key and value type.
        /// </summary>
        public EntryDatabase()
        {
            this._BackingList = new List<TEntry>();
            this._Index = new Dictionary<string, int>();
        }

        public EntryDatabase(IEnumerable<TEntry> entries, Func<TEntry, string> nameSelector)
            : this()
        {
            foreach (var item in entries)
            { this.Add(nameSelector(item), item); }
        }
        #endregion

        #region Modifiers
        /// <summary>
        /// Add an entry to the collection
        /// </summary>
        public void Add(string key, TEntry value)
        {
            if (this.Contains(key.ToLowerInvariant()))
                throw new ArgumentException("An item with the same key has already been added.");

            _BackingList.Add(value);
            _Index.Add(key.ToLowerInvariant(), _BackingList.IndexOf(value));
        }

        /// <summary>
        /// Remove the entry with the specified value
        /// </summary>
        public void Remove(TEntry value)
        {
            if (this.Contains(value))
            {
                int entryIndex = _BackingList.IndexOf(value);

                // Remove the entry itself
                _BackingList.RemoveAt(entryIndex);

                // Remove link from index
                // The center line searches for the TPrimaryKey for the entry's index
                _Index.Remove(
                    _Index.First(e => e.Value == entryIndex).Key.ToLowerInvariant()
                );
            }
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Remove the entry at the specified index
        /// </summary>
        public void Remove(int index)
        {
            try
            {
                // Delete the item
                _BackingList.RemoveAt(index);

                // Remove link from index
                _Index.Remove(
                    _Index.First(e => e.Value == index).Key.ToLowerInvariant()
                );
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the index of the element we want to delete is out of range, rethrow the exception.
                throw;
            }
        }

        /// <summary>
        /// Remove the entry with the specified key
        /// </summary>
        public void Remove(string key)
        {
            try
            {
                // Fetch the index for the key and remove the value
                _BackingList.RemoveAt(
                    _Index[key.ToLowerInvariant()]
                );

                // Update the index
                _Index.Remove(key.ToLowerInvariant());
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the key of the element we want to delete is out of range, rethrow the exception.
                throw;
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get the entry based on its key
        /// </summary>
        public TEntry this[string key]
        {
            get
            {
                return _BackingList[
                    _Index[key.ToLowerInvariant()]
                ];
            }
        }

        /// <summary>
        /// Get the entry based on its ID
        /// </summary>
        public TEntry this[int id]
        {
            get
            {
                return _BackingList[id];
            }
        }
        #endregion

        #region Lookups
        /// <summary>
        /// The count of elements in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return _BackingList.Count;
            }
        }

        /// <summary>
        /// Get the index associanted with the value
        /// </summary>
        public int IndexOf(TEntry value)
        {
            if (this.Contains(value))
                return _BackingList.IndexOf(value);
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Get the key for a specified value
        /// </summary>
        public string KeyOf(TEntry value)
        {
            if (this.Contains(value))
            {
                int entryIndex = _BackingList.IndexOf(value);

                // Find the key for the entry's index in the Dictionary
                return _Index.Where(i => i.Value == entryIndex).Select(s => s.Key).First().ToLowerInvariant();
            }
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Get whether the specified value exits in the collection
        /// </summary>
        public bool Contains(TEntry value)
        {
            return _BackingList.Contains(value);
        }

        /// <summary>
        /// Get whether a value with the specified index exits in the collection
        /// </summary>
        public bool Contains(int index)
        {
            try
            {
                // Trying to access the element at the specified index will throw an exception if missing.
                _BackingList.ElementAt(index);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get whether a value with the specified key exits in the collection
        /// </summary>
        public bool Contains(string key)
        {
            return _Index.ContainsKey(key.ToLowerInvariant());
        }
        #endregion

        #region Enumerators
        /// <summary>
        /// Get an IEnumerable containing the entries of the collection
        /// </summary>
        public IEnumerable<TEntry> GetEntries()
        {
            foreach (TEntry entry in _BackingList)
                yield return entry;
        }

        /// <summary>
        /// Get an IEnumerable containing KeyValuePair<>s where the Key is the numerical ID and the Value is the entry
        /// </summary>
        public IEnumerable<KeyValuePair<int, TEntry>> GetIDPairs()
        {
            foreach (KeyValuePair<string, int> indexEntry in _Index)
                yield return new KeyValuePair<int, TEntry>(indexEntry.Value, _BackingList[indexEntry.Value]);
        }

        /// <summary>
        /// Get an IEnumerable containing KeyValuePair<>s where the Key is the string key and the Value is the entry
        /// </summary>
        public IEnumerable<KeyValuePair<string, TEntry>> GetNamePairs()
        {
            foreach (KeyValuePair<string, int> indexEntry in _Index)
                yield return new KeyValuePair<string, TEntry>(indexEntry.Key, _BackingList[indexEntry.Value]);
        }
        #endregion
    }
}
