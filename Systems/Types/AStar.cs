using System;
using System.Collections.Generic;
using System.Linq;

namespace Hebert.Types.AStar
{
    /// <summary> Represents a node within a larger graph. </summary>
    /// <typeparam name="T"> The type of object this node represents. </typeparam>
    public class GraphNode<T> : IEquatable<GraphNode<T>> where T : class, IGraphable
    {
        /// <summary> The node's unique identifier in the graph. </summary>
        public readonly Int32 Id;

        /// <summary> The value this node represents. </summary>
        public readonly T Value;

        /// <summary> The immediate neighbours to this node. </summary>
        public readonly HashSet<GraphNode<T>> Neighbours;


        /// <summary> The known cost to reach the current node from the beginning node. </summary>
        public Single G { get; private set; }

        /// <summary> The estimated cost, its heuristic, to reach the end node from this one. </summary>
        public Single H { get; private set; }

        /// <summary> The estimated cost of the route from the start node to the end node from this node's perspective. </summary>
        public Single F => G + H;


        /// <summary> Represents a node within a larger graph. </summary>
        /// <param name="id"> The node's unique identifier in the graph. </param>
        /// <param name="value"> The value this node represents. </param>
        public GraphNode(Int32 id, T value)
        {
            Id = id;
            Value = value;
            Neighbours = new HashSet<GraphNode<T>>();
        }


        /// <summary> Set the node's cost values. </summary>
        /// <param name="g"> The known cost to reach the current node from the beginning node. </param>
        /// <param name="h"> The estimated cost, its heuristic, to reach the end node from this one. </param>
        public void SetCosts(Single g, Single h)
        {
            G = g;
            H = h;
        }


        /// <inheritdoc/>
        public Boolean Equals(GraphNode<T>? other) => Id == other?.Id;


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(Id);
    }


    /// <summary> Indicates that the object can be used in a node graph for pathfinding. </summary>
    public interface IGraphable
    {
        /// <summary> Get an array of all the objects bordering this one. </summary>
        /// <returns> The list of all this object's neighbouring objects. </returns>
        public IGraphable[] GetNeighbours();

        /// <summary> Calculate the 'cost' to reach the given object from this one. </summary>
        /// <param name="other"> The other object to attempt to reach. </param>
        /// <returns> The cost to move from this object to the other. </returns>
        public Single CalculateHeuristic(IGraphable other);
    }


    /// <summary> A data object representing a collection of nodes with edges connecting them. </summary>
    /// <typeparam name="T"> The type of object a node represents. </typeparam>
    public class Graph<T> where T : class, IGraphable
    {
        /// <summary> The nodes within the graph. </summary>
        private readonly HashSet<GraphNode<T>> _nodes;


        /// <summary> A data object representing a collection of nodes with edges connecting them. </summary>
        /// <param name="values"> An array of the objects to build the graph around. </param>
        private Graph(T[] values)
        {
            // Initialise the nodes.
            _nodes = new HashSet<GraphNode<T>>();
            Dictionary<T, GraphNode<T>> map = new Dictionary<T, GraphNode<T>>();
            for (Int32 i = 0; i < values.Length; i++)
            {
                GraphNode<T> node = new GraphNode<T>(i, values[i]);
                _nodes.Add(node);
                map.Add(values[i], node);
            }

            // Add the neighbours for each node.
            foreach (GraphNode<T> node in _nodes)
            {
                IGraphable[] neighbours = node.Value.GetNeighbours();
                foreach (IGraphable neighbour in neighbours)
                {
                    node.Neighbours.Add(map[(T)neighbour]);
                }
            }
        }


        /// <summary> Build a graph to represent the given objects. </summary>
        /// <param name="values"> An array of the objects to build the graph around. </param>
        /// <returns> The constructed  graph object. </returns>
        public static Graph<T> Build(T[] values) => new Graph<T>(values);


        /// <summary> Calculate a path between the given objects. </summary>
        /// <param name="startObject"> The object to start traversing from. </param>
        /// <param name="endObject"> The object to attempt to reach. </param>
        /// <param name="maxIterations"> The amount of iterations we allow before we stop, assuming that the path is inaccessible. </param>
        /// <returns> A path between the two objects. An empty array indicates that there isn't one. </returns>
        public T[] CalculatePath(T startObject, T endObject, Int32 maxIterations = 10000)
        {
            GraphNode<T>? startNode = _nodes.FirstOrDefault(x => x.Value == startObject) ?? null;
            GraphNode<T>? endNode = _nodes.FirstOrDefault(x => x.Value == endObject) ?? null;
            if (startNode == null || endNode == null)
            {
                return Array.Empty<T>();
            }

            return CalculatePath(startNode, endNode, maxIterations).Select(x => x.Value).ToArray();
        }


        /// <summary> Calculate a path between the given nodes. </summary>
        /// <param name="startNode"> The node to start traversing from. </param>
        /// <param name="endNode"> The node to attempt to reach. </param>
        /// <param name="maxIterations"> The amount of iterations we allow before we stop, assuming that the path is inaccessible. </param>
        /// <returns> A path between the two nodes. An empty array indicates that there isn't one. </returns>
        public GraphNode<T>[] CalculatePath(GraphNode<T> startNode, GraphNode<T> endNode, Int32 maxIterations = 10000)
        {
            SortedSet<GraphNode<T>> openSet = new SortedSet<GraphNode<T>>(new GraphNodeComparer<T>());          // The nodes that still need to be searched.
            HashSet<GraphNode<T>> closedSet = new HashSet<GraphNode<T>>();                                      // The nodes that have been fully searched and discarded.
            Dictionary<Int32, GraphNode<T>> parentMap = new Dictionary<Int32, GraphNode<T>>();                  // A map of parents to their children.

            startNode.SetCosts(0f, startNode.Value.CalculateHeuristic(endNode.Value));
            openSet.Add(startNode);                                 // Start searching from the initial node.
            Int32 iteration = 0;
            while (iteration < maxIterations && openSet.Count > 0)  // Ensure we do not exceed the max iterations.
            {
                iteration++;

                // Pop the current for examination.
                GraphNode<T> current = openSet.First();

                // If the current is the end, we've found it!
                if (current.Equals(endNode))
                {
                    return ReconstructPath(parentMap, current);
                }

                // We're about to explore the current, node so we move it to closed.
                openSet.Remove(current);
                closedSet.Add(current);

                foreach (GraphNode<T> neighbour in current.Neighbours)
                {
                    // If the neighbour hasn't already been explored/
                    if (!closedSet.Contains(neighbour))
                    {
                        Single h = current.Value.CalculateHeuristic(neighbour.Value);
                        Single tentativeG = current.G + h;

                        // If the neighbour is worth exploring, add it to the open set.
                        if (tentativeG < neighbour.G || !openSet.Contains(neighbour))
                        {
                            neighbour.SetCosts(tentativeG, h);
                            parentMap[neighbour.Id] = current;
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            // A path couldn't be found.
            return Array.Empty<GraphNode<T>>();
        }


        /// <summary> Reconstruct the path from the given node to the starting position. </summary>
        /// <param name="parentMap"> The parent-child map. </param>
        /// <param name="node"> The current node. </param>
        /// <returns> A path starting from the beginning of the graph and ending at the current node. </returns>
        private GraphNode<T>[] ReconstructPath(Dictionary<Int32, GraphNode<T>> parentMap, GraphNode<T> node)
        {
            List<GraphNode<T>> path = new List<GraphNode<T>>() { node };

            GraphNode<T> current = node;
            while (parentMap.ContainsKey(current.Id))
            {
                current = parentMap[current.Id];
                path.Add(current);
            }

            path.Reverse();
            return path.ToArray();
        }
    }


    /// <summary> The comparer to use when sorting graph nodes. </summary>
    /// <typeparam name="T"> The type of object this node represents. </typeparam>
    public class GraphNodeComparer<T> : IComparer<GraphNode<T>> where T : class, IGraphable
    {
        /// <inheritdoc/>
        public Int32 Compare(GraphNode<T>? x, GraphNode<T>? y)
        {
            if (x == null || y == null) return 0;

            Int32 compare = x.F.CompareTo(y.F);
            if (compare == 0)
            {
                compare = x.Id.CompareTo(y.Id);
            }
            return compare;
        }
    }
}
