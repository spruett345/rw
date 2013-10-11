using System;
using System.Collections;
using System.Collections.Generic;

namespace Rw.Collections
{
    public class ImmutableList<T> : IEnumerable<T>, IEnumerable
    {
        public static ImmutableList<T> Empty = new ImmutableList<T>();

        public readonly ImmutableList<T> Tail;
        public readonly T Value;
        public readonly int Size;

        public ImmutableList(IEnumerable<T> collection)
        {
            var stack = new Stack<T>();
            foreach (var element in collection)
            {
                stack.Push(element);
            }
            Tail = Empty;
            while (stack.Count > 1)
            {
                Tail = Tail.Prepend(stack.Pop());
            }
            Value = stack.Pop();
            Size = Tail.Size + 1;
        }

        private ImmutableList()
        {
            Value = default(T);
            Tail = null;
            Size = 0;
        }
        private ImmutableList(T value, ImmutableList<T> tail)
        {
            Value = value;
            Tail = tail;
            Size = tail.Size + 1;
        }

        private bool IsLast()
        {
            return Tail == Empty;
        }

        public ImmutableList<T> Prepend(T value)
        {
            return new ImmutableList<T>(value, this);
        }
        public ImmutableList<T2> Map<T2>(Func<T, T2> f)
        {
            var stack = new Stack<T2>();
            foreach (var val in this)
            {
                stack.Push(f(val));
            }
            var list = ImmutableList<T2>.Empty;
            while (stack.Count > 0)
            {
                list = list.Prepend(stack.Pop());
            }
            return list;
        }
        public ImmutableList<T> Filter(Func<T, bool> f)
        {
            var stack = new Stack<T>();
            foreach (var val in this)
            {
                if (f(val))
                {
                    stack.Push(val);
                }
            }
            var list = Empty;
            while (stack.Count > 0)
            {
                list = list.Prepend(stack.Pop());
            }
            return list;
        }
        public T2 Reduce<T2>(Func<T2, T, T2> f, T2 start)
        {
            var reduced = start;
            foreach (var val in this)
            {
                reduced = f(reduced, val);
            }
            return reduced;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = this;
            while (current != Empty)
            {
                yield return current.Value;
                current = current.Tail;
            }
        }
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            var current = this;
            while (current != Empty)
            {
                yield return current.Value;
                current = current.Tail;
            }
        }
    }
}

