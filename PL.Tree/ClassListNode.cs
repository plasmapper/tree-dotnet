using System;
using System.Collections;
using System.Collections.Generic;

namespace PL.Tree
{
    /// <summary>
    /// Represents list with ClassNode-based items.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public class ClassListNode<T> : ContainerNode, IList<T> where T : ClassNode, new()
    {
        private List<T> _items;

        /// <summary>
        /// Initializes a new instance of the ClassListNode class.
        /// </summary>
        public ClassListNode() => _items = new List<T>();

        /// <summary>
        /// Sets item values from source list item values.
        /// </summary>
        /// <param name="items">Source list</param>
        public void SetValue(IList<T> list)
        {
            ClassListNode<T> newList = new ClassListNode<T>();
            for (int i = 0; i < list.Count; i++)
            {
                newList.Add(new T());
                newList[i].SetValue(list[i]);
            }
            
            if (Count == list.Count)
                base.SetValue(newList);
            else
            {
                lock (this)
                {
                    _items = new List<T>(newList._items);
                    foreach (var item in _items)
                        ((Node)(object)item).Parent = this;
                }
                OnValueChanged();
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this)
                    return _items[index];
            }
            set => this[index].SetValue(value);
        }

        public int Count
        {
            get
            {
                lock (this)
                    return _items.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            lock (this)
            {
                T newElement = new T();
                newElement.SetValue(item);
                _items.Add(newElement);
                newElement.Parent = this;
            }
            OnValueChanged();
        }

        public void Clear()
        {
            if (Count == 0)
                return;
            lock (this)
            {
                foreach (var item in _items)
                    ((Node)(object)item).Parent = null;
                _items.Clear();
            }
            OnValueChanged();
        }

        public bool Contains(T item)
        {
            lock (this)
                return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this)
                _items.CopyTo(array, arrayIndex);
        }
        
        public int FindIndex(Predicate<T> match)
        {
            lock (this)
                return _items.FindIndex(match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this)
                return new List<T>(_items).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (this)
                return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (this)
            {
                T newElement = new T();
                newElement.SetValue(item);
                _items.Insert(index, newElement);
                newElement.Parent = this;
            }
            OnValueChanged();
        }

        public bool Remove(T item)
        {
            bool removed;
            lock (this)
                removed = _items.Remove(item);
            if (removed)
            {
                ((Node)(object)item).Parent = null;
                OnValueChanged();
            }
            return removed;
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                ((Node)(object)_items[index]).Parent = null;
                _items.RemoveAt(index);
            }
            OnValueChanged();
         }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override IEnumerable<Node> GetChildren()
        {
            lock (this)
                return new List<T>(_items);
        }

        public override object GetValue()
        {
            lock (this)
                return new List<T>(_items);
        }
        public override void SetValue(object value) => SetValue((IList<T>)value);
    }
}