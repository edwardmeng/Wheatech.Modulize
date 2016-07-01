using System;
using System.Collections.Generic;
using System.Linq;
using Wheatech.Collection;

namespace Wheatech.Modulize
{
    internal class DependentNode<T>
    {
        private readonly ICollection<DependentNode<T>> _incomings = new List<DependentNode<T>>();
        private readonly ICollection<DependentNode<T>> _outcommings = new List<DependentNode<T>>();

        public DependentNode(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public DependentNode<T>[] Incomings => _incomings.ToArray();

        public DependentNode<T>[] Outcommings => _outcommings.ToArray();

        public void AddIncoming(T incoming)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            AddIncoming(new DependentNode<T>(incoming));
        }

        public void AddIncoming(DependentNode<T> incoming)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            if (!_incomings.Contains(incoming))
            {
                _incomings.Add(incoming);
            }
        }

        public void AddOutcoming(T outcoming)
        {
            if (outcoming == null) throw new ArgumentNullException(nameof(outcoming));
            AddOutcoming(new DependentNode<T>(outcoming));
        }

        public void AddOutcoming(DependentNode<T> outcoming)
        {
            if (outcoming == null) throw new ArgumentNullException(nameof(outcoming));
            if (!_outcommings.Contains(outcoming))
            {
                _outcommings.Add(outcoming);
            }
        }

        public void RemoveIncoming(T incoming)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            RemoveIncoming(new DependentNode<T>(incoming));
        }

        public void RemoveIncoming(DependentNode<T> incoming)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            _incomings.Remove(incoming);
        }

        public void RemoveOutcomming(T outcoming)
        {
            if (outcoming == null) throw new ArgumentNullException(nameof(outcoming));
            RemoveOutcomming(new DependentNode<T>(outcoming));
        }

        public void RemoveOutcomming(DependentNode<T> outcoming)
        {
            if (outcoming == null) throw new ArgumentNullException(nameof(outcoming));
            _outcommings.Remove(outcoming);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            var other = obj as DependentNode<T>;
            return other != null && Equals(Value, other.Value);
        }

        public static DependentNode<T>[] CreateGraph(IEnumerable<T> values, Func<T, T[]> dependencyLocator)
        {
            var nodes = new OrderedDictionary<T, DependentNode<T>>();
            foreach (var value in values)
            {
                if (!nodes.ContainsKey(value))
                {
                    nodes.Add(value, new DependentNode<T>(value));
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                var currentNode = nodes[i];
                foreach (var dependency in dependencyLocator(currentNode.Value))
                {
                    if (!Equals(dependency, currentNode.Value))
                    {
                        DependentNode<T> dependencyNode;
                        if (!nodes.TryGetValue(dependency, out dependencyNode))
                        {
                            dependencyNode = new DependentNode<T>(dependency);
                            nodes.Add(dependency, dependencyNode);
                        }
                        currentNode.AddOutcoming(dependencyNode);
                        dependencyNode.AddIncoming(currentNode);
                    }
                }
            }
            return nodes.Values.ToArray();
        }

        public static T[] SortValues(IEnumerable<T> values, Func<T, T[]> dependencyLocator, out T[] circleValues)
        {
            var graph = CreateGraph(values, dependencyLocator).ToList();
            var sorted = new List<T>();
            var queue = new Queue<DependentNode<T>>(graph.Where(node => node.Outcommings.Length == 0));
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Outcommings.Length == 0)
                {
                    foreach (var incomeNode in node.Incomings)
                    {
                        incomeNode.RemoveOutcomming(node);
                        queue.Enqueue(incomeNode);
                    }
                    sorted.Add(node.Value);
                    graph.Remove(node);
                }
            }
            if (graph.Count > 0)
            {
                var circle = new List<DependentNode<T>>();
                var currentNode = graph[0];
                while (!circle.Contains(currentNode))
                {
                    circle.Add(currentNode);
                    currentNode = currentNode.Outcommings.First();
                }
                circleValues = circle.Select(node => node.Value).ToArray();
            }
            else
            {
                circleValues = null;
            }
            return sorted.ToArray();
        }
    }
}
