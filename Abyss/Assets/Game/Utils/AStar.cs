using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
	public class Context<T>
	{
		public bool isDebug = false;
		public List<T> path;
		public T start;
		public Predicate<T> procTermination;
		public Func<T, List<T>> procAdjacencies;
		public Func<T, T, float> procWeight;
		public Func<T, float> procDistanceEstimator;
		public Func<T, int> procTrait;
	}
	public static bool Evaluate<T>(Context<T> ctx)
	{
		if (ctx.procTermination.Invoke(ctx.start)) 
		{
			ctx.path = new List<T>();
			return true;
		}

		var opened = new MinHeap<T>();
		var map = new Dictionary<int, Node<T>>();
		var closed = new HashSet<int>();
		var startNode = new Node<T>(ctx.start, 0, 0);
		foreach (var adj in ctx.procAdjacencies.Invoke(ctx.start))
		{
			var node = new Node<T>(adj,
			                       ctx.procWeight.Invoke(ctx.start, adj),
			                       ctx.procDistanceEstimator(adj));
			node.backwardRef = startNode;
			map.Add(ctx.procTrait.Invoke(node.substance), node);
			opened.AddNode(node);
		}
		closed.Add(ctx.procTrait.Invoke(ctx.start));
		List<Node<T>> matches = new List<Node<T>>();

		while (opened.Length > 0)
		{
			if (ctx.isDebug)
			{
				var str = "";
				opened.Foreach((n)=>{
					var s = n.substance as SlotWrapper2D;
					str += " [" + s.pos.x + "," + s.pos.y + "," + (n.dstStart + n.dstEndEstimated) + "] |";
				});
				Debug.LogWarning(str);
			}

			matches.Clear();
			var cur = opened.ExtractMin();
			if (ctx.procTermination.Invoke(cur.substance))
			{
				ctx.path = new List<T>();
				var step = cur;
				while (null != step)
				{
					ctx.path.Add(step.substance);
					step = step.backwardRef;
				}
				return true;
			}

			var adjs = ctx.procAdjacencies.Invoke(cur.substance);
			closed.Add(ctx.procTrait.Invoke(cur.substance));
			map.Remove(ctx.procTrait.Invoke(cur.substance));

			foreach (var adj in adjs)
			{
				if (closed.Contains(ctx.procTrait.Invoke(adj)))
				{
					continue;
				}
				var estimation = ctx.procDistanceEstimator.Invoke(adj);
				var dstIncrease = ctx.procWeight.Invoke(cur.substance, adj);
				if (map.ContainsKey(ctx.procTrait.Invoke(adj)))
				{
					var exist = map[ctx.procTrait.Invoke(adj)];
					if (cur.dstStart + dstIncrease + estimation < exist.dstStart + exist.dstEndEstimated)
					{
						exist.dstStart = cur.dstStart + dstIncrease;
						exist.backwardRef = cur;
						opened.HeapifyAll();
					}
				}
				else
				{
					var node = new Node<T>(adj, cur.dstStart + dstIncrease, ctx.procDistanceEstimator(adj));
					node.backwardRef = cur;
					map.Add(ctx.procTrait.Invoke(adj), node);
					opened.AddNode(node);
				}
			}
		}

		return false;
	}

	private class Node<T>
	{
		public Node(T substance, float dstStart, float dstEndEstimated)
		{
			this.substance = substance;
			this.dstStart = dstStart;
			this.dstEndEstimated = dstEndEstimated;
		}
		public T substance;
		public float dstStart;
		public float dstEndEstimated;
		public Node<T> backwardRef;
	}

	private class MinHeap<T>
	{
		public class Wrapper
		{
			public Wrapper(Node<T> node, int index)
			{
				this.node = node;
				this.index = index;
			}
			public Node<T> node;
			public int index;

			public bool ComparePriority(Wrapper other)
			{
				if (node.dstStart + node.dstEndEstimated < other.node.dstStart + other.node.dstEndEstimated)
				{
					return true;
				}
				else if (node.dstStart + node.dstEndEstimated > other.node.dstStart + other.node.dstEndEstimated)
				{
					return false;
				}
				else
				{
					return node.dstEndEstimated < other.node.dstEndEstimated;
				}
			}
			public bool IsRoot { get { return 0 == index; } }
			public int LeftChildIndex { get { return (index + 1) * 2 - 1; } }
			public int RightChildIndex { get { return (index + 1) * 2; } }
			public int ParentIndex { get { return (index + 1) / 2 - 1; } }
		}
		private int length = 0;
		public int Length { get { return length; } }

		private Wrapper[] buffer = new Wrapper[16];
		public Node<T> ExtractMin()
		{
			var ret = buffer[0].node;
			if (length > 1)
			{
				Swap(buffer[0], buffer[length - 1]);
			}
			buffer[length - 1] = null;
			length--;
			HeapifyDownward(buffer[0]);

			return ret;
		}

		public void HeapifyDownward(Wrapper w)
		{
			if (null == w) return;
			var left = LeftChild(w);
			var right = RightChild(w);
			if (null == left && null == right)
			{
				return;
			}
			else if (null != left && null == right)
			{
				if (left.ComparePriority(w))
				{
					Swap(w, left);
				}
			}
			else
			{
				var isLeft = left.ComparePriority(right);
				if (isLeft && left.ComparePriority(w))
				{
					Swap(w, left);
					HeapifyDownward(w);
					return;
				}
				if (!isLeft && right.ComparePriority(w))
				{
					Swap(w, right);
					HeapifyDownward(w);
				}
			}
		}

		public void Foreach(Action<Node<T>> proc)
		{
			foreach (var n in buffer)
			{
				if (null == n) return;
				proc.Invoke(n.node);
			}
		}

		public void AddNode(Node<T> node)
		{
			var w = new Wrapper(node, length);
			buffer[length] = w;
			HeapifyUpward(buffer[length]);
			length++;

			if (length >= buffer.Length)
			{
				Expansion();
			}
		}

		public void HeapifyAll()
		{
			for (int i = length - 1; i > 0; i--)
			{
				HeapifyUpward(buffer[i]);
			}
		}

		private Wrapper LeftChild(Wrapper w)
		{
			var idx = w.LeftChildIndex;
			return (idx < length) ? buffer[idx] : null;
		}
		private Wrapper RightChild(Wrapper w)
		{
			var idx = w.RightChildIndex;
			return (idx < length) ? buffer[idx] : null;
		}
		private Wrapper Parent(Wrapper w)
		{
			if (w.IsRoot) return null;
			return buffer[w.ParentIndex];
		}
		private void HeapifyUpward(Wrapper w)
		{
			if (w.IsRoot) return;
			var parent = Parent(w);
			if (w.ComparePriority(parent))
			{
				Swap(w, parent);
				HeapifyUpward(w);
			}
		}
		private void Swap(Wrapper w1, Wrapper w2)
		{
			var tmp = w1.index;
			buffer[w2.index] = w1;
			buffer[w1.index] = w2;
			w1.index = w2.index;
			w2.index = tmp;
		}
		private void Expansion()
		{
			var tmp = new Wrapper[buffer.Length * 2];
			Array.Copy(buffer, tmp, buffer.Length);
			buffer = tmp;
		}
	}
}
