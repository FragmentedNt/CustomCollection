using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using CustomCollections;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public void prepareBufferValue(CircularBuffer<int> v, Queue<int> q, int times = -1)
        {
            var r = new Random();
            if (times < 0)
                times = v.Capacity * 2;
            for (int i = 0; i < times; i++)
            {
                int val = r.Next();
                if (q.Count + 1 > v.Capacity)
                    q.Dequeue();
                q.Enqueue(val);
                v.Enqueue(val);
            }
        }

        [TestMethod]
        public void CircularbufferEnqueueTest()
        {
            var v = new CircularBuffer<int>(10);
            var q = new Queue<int>();
            prepareBufferValue(v, q);
            var arr = q.ToArray();
            for (int i = 0; i < v.Count; i++)
            {
                Assert.AreEqual(v[i], arr[i]);
            }
        }

        [TestMethod]
        public void CircularbufferReverseTest()
        {
            var v = new CircularBuffer<int>(10);
            var q = new Queue<int>();
            prepareBufferValue(v, q);
            var rev_arr = q.Reverse().ToArray();
            for (int i = 0; i < v.Count; i++)
            {
                Assert.AreEqual(v[-1 - i], rev_arr[i]);
            }
        }

        [TestMethod]
        public void CircularbufferOverRunTest()
        {
            var v = new CircularBuffer<int>(10);
            try
            {
                v.Dequeue();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            prepareBufferValue(v, new Queue<int>());
            try
            {
                var hoge = v[v.Capacity];
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            try
            {
                var hoge = v[-v.Capacity-1];
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CircularbufferExtendTest()
        {
            var v = new CircularBuffer<int>(10);
            var q = new Queue<int>();
            prepareBufferValue(v, q);
            var arr = q.ToArray();
            for (int i = 0; i < v.Count; i++)
            {
                Assert.AreEqual(v[i], arr[i]);
            }
            v.Extend();
            prepareBufferValue(v, q, 3);
            arr = q.ToArray();
            for (int i = 0; i < v.Count; i++)
            {
                Assert.AreEqual(v[i], arr[i]);
            }
        }
    }
}
