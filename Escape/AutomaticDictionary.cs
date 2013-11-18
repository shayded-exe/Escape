using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape
{
    class AutomaticDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public Func<TValue, TKey> KeySelector { get; private set; }
        public AutomaticDictionary(Func<TValue, TKey> keySelector)
        {
            KeySelector = keySelector;
        }

        public void Add(TValue value)
        {
            Add(KeySelector(value), value);
        }
    }
}
