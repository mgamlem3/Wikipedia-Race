////////////
// File Name: ArticleCollection.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 3, 2018
// Refer to License for Use Terms
////////////

/* 
    This file was created with help from https://www.koderdojo.com/blog/breadth-first-search-and-shortest-path-in-csharp-and-net-core.
    The graph and adjacency list used to implement breadth-first search is modified from the above link. (First Modified December 10, 2018).
    Modifications:
        Modified to be used in a multithreaded environment
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class Graph<T> {
        public Graph() {}
        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T,T>> edges) {
            foreach(var vertex in vertices)
                AddVertex(vertex);

            foreach(var edge in edges)
                AddEdge(edge);
        }

        public ConcurrentDictionary<T, HashSet<T>> AdjacencyList { get; } = new ConcurrentDictionary<T, HashSet<T>>();

        public void AddVertex(T vertex) {
            AdjacencyList[vertex] = new HashSet<T>();
        }

        public void AddEdge(Tuple<T,T> edge) {
            if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2)) {
                AdjacencyList[edge.Item1].Add(edge.Item2);
                AdjacencyList[edge.Item2].Add(edge.Item1);
            }
        }
    }