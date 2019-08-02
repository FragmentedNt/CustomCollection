using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace CustomCollections
{
    /// <summary>
    /// Fixed size circular buffer
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class CircularBuffer<T> : IEnumerable<T>
    {
        #region Field

        T[] data;
        int top, bottom;    // top:latest data index, bottom:oldest data index
        int mask;

        /// <summary>
        /// Number of stored elements 
        /// </summary>
        public int Count { get; private set; }

        public int Capacity { get { return data.Length; } }

        #endregion
        #region Initializer

        public CircularBuffer() : this(256) { }

        /// <summary>
        /// Initialized by specifying the initial maximum capacity
        /// </summary>
        /// <param name="capacity">initial maximum capacity</param>
        public CircularBuffer(int capacity)
        {
            capacity = Pow2((uint)capacity);
            this.data = new T[capacity];
            this.top = -1;
            this.bottom = 0;
            this.mask = capacity - 1;
            this.Count = 0;
            //   Console.WriteLine($"mask:{Convert.ToString(this.mask, 2)}");
        }

        static int Pow2(uint n)
        {
            --n;
            int p = 0;
            for (; n != 0; n >>= 1) p = (p << 1) + 1;
            return p + 1;
        }

        #endregion
        #region プロパティ

        /// <summary>
        /// Read element from buffer index i
        /// </summary>Gets the element at the specified index.
        /// <param name="i">The zero-based index of the element to get.</param>
        /// <returns>Element</returns>
        public T this[int i]
        {
            get
            {
                if ((i < 0 && this.Count < -i) || (0 <= i && this.Count <= i))
                    throw new IndexOutOfRangeException("");
                if (this.Count != this.data.Length && i < 0)
                    return this.data[(this.top + i + 1) & this.mask];
                return this.data[(i + this.bottom) & this.mask];
            }
        }

        #endregion
        #region Enqueue, Dequeue

        /// <summary>
        /// Adds an element to the end of the CircularBuffer<T>.
        /// </summary>
        /// <param name="elem">The element to add to the CircularBuffer<T>.</param>
        public void Enqueue(T elem)
        {
            if (((this.top + 1) & this.mask) == this.bottom && top >= 0) this.bottom = (this.bottom + 1) & this.mask;
            this.top = (this.top + 1) & this.mask;
            this.data[this.top] = elem;
            ++this.Count;
            if (this.Count > this.data.Length) this.Count = this.data.Length;
        }

        /// <summary>
        /// Removes and returns the element at the beginning of the CircularBuffer<T>.
        /// </summary>
        /// <returns>Element at the beginning of the CircularBuffer<T>.</returns>
        /// <exception cref="System.InvalidOperationException">Throw when called this if Count==0</exception>  
        public T Dequeue()
        {
            if (this.Count == 0) throw new InvalidOperationException("No data in buffer");
            T result = this[0];
            this.bottom = (this.bottom + 1) & this.mask;
            --this.Count;
            // Console.WriteLine($"Deq > top:{this.top}  bottom:{this.bottom}  Count:{this.Count}");
            return result;
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the buffer.
        /// </summary>
        /// <paramref name="elem">When this method returns, if the operation was successful, result contains the object removed. If no object was available to be removed, the value is unspecified.</paramref>
        /// <returns> True:if an element was removed and returned from the beginning of the CircularBuffer<T>.</returns>
        public bool TryDequeue(out T result)
        {
            result = default(T);
            result = this[0];
            if (this.Count == 0) return false;
            this.bottom = (this.bottom + 1) & this.mask;
            --this.Count;
            // Console.WriteLine($"TryDeq > top:{this.top}  bottom:{this.bottom}  Count:{this.Count}");
            return true;
        }

        /// <summary>
        /// Expanding the buffer size
        /// </summary>
        /// <remarks>
        /// Expanded by 2 times
        /// </remarks>
        public void Extend()
        {
            T[] data = new T[this.data.Length * 2];
            int i = 0;
            foreach (T elem in this)
            {
                data[i] = elem;
                ++i;
            }
            this.top = this.data.Length-1;
            this.bottom = 0;
            this.data = data;
            this.mask = data.Length - 1;
        }

        /// <summary>
        /// Get String
        /// </summary>
        /// <param name="format">A compositr string format</param>
        public string ToString(string format)
        {
            string result = $"Count:{Count} Capacity:{Capacity} Top:{top} Bottom:{bottom} Mask:{mask}{Environment.NewLine}";
            for (int i = 0; i < this.Count; i++)
            {
                result += String.Format(format, this[i]);
            }
            result += Environment.NewLine;
            return result;
        }

        /// <summary>
        /// Get String
        /// </summary>
        public override string ToString()
        {
            string format = "{0:X" + (Marshal.SizeOf(typeof(T)) * 2).ToString() + "} ";
            return this.ToString(format);
        }

        #endregion
        #region IEnumerable<T> member

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; ++i)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Fixed size ConqurrentQueue
    /// </summary>
    public class FixedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public int Capacity { get; private set; }

        public FixedConcurrentQueue(int capacity)
        {
            Capacity = capacity;
        }

        public new void Enqueue(T item)
        {
            T output;
            while (Capacity <= Count)
            {
                TryDequeue(out output);
            }
            base.Enqueue(item);
        }
    }
}