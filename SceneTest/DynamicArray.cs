using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class DynamicArray<T>
    {
        // Fields
        protected int m_capcity;
        protected T[] m_data;
        protected int m_size;

        // Methods
        public DynamicArray()
        {
            this.m_data = null;
            this.m_size = 0;
            this.m_capcity = 0;
            this.capcity = 0x20;
        }

        public DynamicArray(int cap)
        {
            this.m_data = null;
            this.m_size = 0;
            this.m_capcity = 0;
            this.capcity = cap;
        }

        public void add(T v)
        {
            this.pushBack(v);
        }

        public void clear()
        {
            this.m_data = null;
            this.m_size = 0;
            this.m_capcity = 0;
        }

        public bool contains(T v)
        {
            return (this.indexOf(v) >= 0);
        }

        public void copyTo(T[] array, int index)
        {
            if (((array != null) && (index >= 0)) && ((index + this.m_size) <= array.Length))
            {
                this.m_data.CopyTo(array, index);
            }
        }

        public int indexOf(T v)
        {
            return Array.IndexOf<T>(this.m_data, v);
        }

        public void insert(int index, T value)
        {
        }

        public T popBack()
        {
            if (this.m_size <= 0)
            {
                return default(T);
            }
            return this.m_data[this.m_size--];
        }

        public T popFront()
        {
            if (this.m_size <= 0)
            {
                return default(T);
            }
            T local = this.m_data[0];
            Array.Copy(this.m_data, 1, this.m_data, 0, this.m_size - 1);
            this.m_size--;
            return local;
        }

        public void pushBack(T v)
        {
            if (this.m_capcity < (this.m_size + 1))
            {
                this.capcity *= 2;
            }
            this.m_data[this.m_size++] = v;
        }

        public void pushBack(T[] v, int count)
        {
            int capcity = this.m_capcity;
            while (capcity < (this.m_size + count))
            {
                capcity *= 2;
            }
            this.capcity = capcity;
            for (int i = 0; i < count; i++)
            {
                this.m_data[this.m_size++] = v[i];
            }
        }

        public bool remove(T v)
        {
            if (this.m_size <= 0)
            {
                return false;
            }
            int index = Array.IndexOf<T>(this.m_data, v);
            if (index < 0)
            {
                return false;
            }
            if (index == (this.m_size - 1))
            {
                this.m_size--;
                return true;
            }
            Array.Copy(this.m_data, index + 1, this.m_data, index, (this.m_size - index) - 1);
            this.m_size--;
            return true;
        }

        public void removeAt(int index)
        {
            if ((this.m_size > 0) && (index < this.m_size))
            {
                Array.Copy(this.m_data, index + 1, this.m_data, index, (this.m_size - index) - 1);
                this.m_size--;
            }
        }

        public void skip(int count)
        {
            if (count > this.m_size)
            {
                this.m_size = 0;
            }
            else
            {
                Array.Copy(this.m_data, count, this.m_data, 0, this.m_size - count);
                this.m_size -= count;
            }
        }

        // Properties
        public T back
        {
            get
            {
                if (this.m_size <= 0)
                {
                    return default(T);
                }
                return this.m_data[this.m_size - 1];
            }
        }

        public int capcity
        {
            get
            {
                return this.m_capcity;
            }
            set
            {
                if (this.m_capcity < value)
                {
                    T[] array = new T[value];
                    if (this.m_data != null)
                    {
                        this.m_data.CopyTo(array, 0);
                    }
                    this.m_data = array;
                    this.m_capcity = value;
                }
            }
        }

        public T[] data
        {
            get
            {
                return this.m_data;
            }
        }

        public T front
        {
            get
            {
                if (this.m_size <= 0)
                {
                    return default(T);
                }
                return this.m_data[0];
            }
        }

        public bool isFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool isReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool isSynchronized
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= this.m_size)
                {
                    return default(T);
                }
                return this.m_data[index];
            }
            set
            {
                if (index < this.m_size)
                {
                    this.m_data[index] = value;
                }
            }
        }

        public virtual int length
        {
            get
            {
                return this.m_size;
            }
            set
            {
                if (this.capcity < value)
                {
                    this.capcity = value;
                }
                this.m_size = value;
            }
        }

        public object syncRoot
        {
            get
            {
                return this;
            }
        }
    }

}
