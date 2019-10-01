using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Voidheart {
    /// <summary>
    /// Implementation of Dijkstra pathfinding algorithm.
    /// </summary>
    public class VDijkstraPathfinding
    {
        public Dictionary<Coord, List<Coord>> findAllPaths(Dictionary<Coord, Dictionary<Coord, int>> edges, Coord originNode)
        {
            IPriorityQueue<Coord> frontier = new HeapPriorityQueue<Coord>();
            frontier.Enqueue(originNode, 0);

            Dictionary<Coord, Coord> cameFrom = new Dictionary<Coord, Coord>();
            cameFrom.Add(originNode, default(Coord));
            Dictionary<Coord, int> costSoFar = new Dictionary<Coord, int>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<Coord, List<Coord>> paths = new Dictionary<Coord, List<Coord>>();
            foreach(Coord destination in cameFrom.Keys)
            {
                List<Coord> path = new List<Coord>();
                var current = destination;
                while (!current.Equals(originNode))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                paths.Add(destination, path);
            }
            
            return paths;
        }
        // public List<T> FindPath<T>(Dictionary<T, Dictionary<T, int>> edges, T originNode, T destinationNode)
        // {
        //     IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
        //     frontier.Enqueue(originNode,0);

        //     Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        //     cameFrom.Add(originNode, default(T));
        //     Dictionary<T, int> costSoFar = new Dictionary<T, int>();
        //     costSoFar.Add(originNode,0);

        //     while (frontier.Count != 0)
        //     {
        //         var current = frontier.Dequeue();
        //         var neighbours = GetNeigbours(edges, current);
        //         foreach (var neighbour in neighbours)
        //         {
        //             var newCost = costSoFar[current] + edges[current][neighbour];
        //             if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
        //             {
        //                 costSoFar[neighbour] = newCost;
        //                 cameFrom[neighbour] = current;
        //                 frontier.Enqueue(neighbour,newCost);
        //             }
        //         }
        //         if (current.Equals(destinationNode)) break;
        //     }
        //     List<T> path = new List<T>();
        //     if (!cameFrom.ContainsKey(destinationNode))
        //         return path;

        //     path.Add(destinationNode);
        //     var temp = destinationNode;

        //     while (!cameFrom[temp].Equals(originNode))
        //     {
        //         var currentPathElement = cameFrom[temp];
        //         path.Add(currentPathElement);

        //         temp = currentPathElement;
        //     }

        //     return path;
        // }

        protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, int>> edges, T node)
        {
            if (!edges.ContainsKey(node))
            {
                return new List<T>();
            }
            return edges[node].Keys.ToList();
        }
    }
}