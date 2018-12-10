////////////
// File Name: WikipediaSearcher.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 1, 2018
// Refer to License for Use Terms
////////////

/* 
    This file was created with help from https://www.koderdojo.com/blog/breadth-first-search-and-shortest-path-in-csharp-and-net-core.
    The breadth-first search implementation is modified from the above link. (First Modified December 10, 2018).
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
public class WikipediaSearcher
{
    private Stack<Webpage> PathTaken = new Stack<Webpage>();
    private static string StartPage;
    private static string FinishPage;
    private static ArticleCollection Articles;
    private Stopwatch watch = new Stopwatch();
    private ForbiddenLinks ForbiddenLinksCollection;
    private ConcurrentQueue<string> q = new ConcurrentQueue<string>();

    WikipediaSearcher() {}

    public WikipediaSearcher(ArticleCollection Webpages, string Start, string Finish, ForbiddenLinks l, string searchType) {
        StartPage = Start;
        FinishPage = Finish;
        Articles = Webpages;
        ForbiddenLinksCollection = l;

        if (searchType == "DFS") {        
            watch.Start();
            SearchDFS();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
        else if (searchType == "BFS") {
            watch.Start();
            var shortestPath = SearchBFS(Articles.gr);
            watch.Stop();
            var path = shortestPath(FinishPage);
            Console.WriteLine("Shortest path from {0} to {1}: ", StartPage, FinishPage);
            foreach (var page in path) {
                Console.WriteLine("\t"+page);
            }
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
        else {
            Console.WriteLine("Please enter either DFS or BFS to indicate which search type you prefer.");
        }
    }

    private Func<string, IEnumerable<string>> SearchBFS(Graph<string> graph) {
        var previous = new Dictionary<string, string>();
        bool answerFound = false;
        string start = StartPage;

        q.Enqueue(start.ToLower());

        while (q.Count > 0) {
            string vertex;
            q.TryDequeue(out vertex);
            foreach(var neighbor in Articles.gr.AdjacencyList[vertex]) {
                if (previous.ContainsKey(neighbor)) {
                    continue;
                }
                answerFound = checkIfAnswerFound(neighbor);
                if (answerFound) {
                    break;
                }
                previous[neighbor] = vertex;
                
                q.Enqueue(neighbor);
            }
            if (answerFound) {
                break;
            }
        }

        Func<string, IEnumerable<string>> shortestPath = v => {
            var path = new List<string>{};

            var current = v;
            while (!current.ToLower().Equals(start.ToLower())) {
                path.Add(current);
                if (current == FinishPage) {
                    break;
                }
                current = previous[current];
            };

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;
    }

    private void SearchDFS() {
        Webpage currentPage, nextPage;
        string nextPageString;
        bool successfulTake, answerFound = false;

        currentPage = Articles.GetWebpage(StartPage);
        if (currentPage != null) {
            PathTaken.Push(currentPage);
        }

        while(PathTaken.Count > 0) {
            currentPage = PathTaken.Peek();
                // get next page to be searched
                try {
                    successfulTake = currentPage.WebpagesToBeSearched.TryTake(out nextPageString);

                    if (successfulTake) {
                        nextPage = Articles.GetWebpage(nextPageString);
                        if (nextPage == null || nextPage.Title == currentPage.Title) {
                            continue;
                        }
                        PathTaken.Push(nextPage);
                        answerFound = checkIfAnswerFound(nextPage);
                        if (answerFound){
                            break;
                        }
                    }
                    else {
                        PathTaken.Pop();
                    }
                    if (PathTaken.Count >= 7) {
                        PathTaken.Pop();
                    }
                } catch (NullReferenceException e) {
                    Console.WriteLine("No webpages have been located yet. " + e);
                    WikipediaWebRequest r = new WikipediaWebRequest(StartPage, Articles, ForbiddenLinksCollection);
                    currentPage = Articles.GetWebpage(StartPage);
                }
        }
        PrintResults(answerFound);
    }

    private bool checkIfAnswerFound(Webpage page) {
        // is the current page the answer?
        Console.WriteLine("Checking For Answer");
        Webpage temp = Articles.WebpageInDictionary(FinishPage);
        bool found = false;
        if (temp != null) {
            PathTaken.Push(page);
            return true;
        }
        else if (temp == null) {
            // does the current page contain the link to the answer?
            found = Articles.WebpageContainsLinkToAnswer(page.Title, FinishPage);
        }
        return found;
    }
    private bool checkIfAnswerFound(string page) {
        // is the current page the answer?
        Console.WriteLine("Checking For Answer");
        Webpage temp = Articles.WebpageInDictionary(FinishPage);
        bool found = false;
        if (temp != null) {
            return true;
        }
        else if (temp == null) {
            // does the current page contain the link to the answer?
            found = Articles.WebpageContainsLinkToAnswer(page, FinishPage);
        }
        return found;
    }

    private void PrintResults(bool answerFound) {
        if (answerFound) {
            Console.WriteLine("Answer Found!");
            Array answerPath = PathTaken.ToArray();
            foreach(Webpage page in answerPath) {
                Console.WriteLine("https://en.wikipedia.org/wiki/{0}", page.Title.Replace(" ", "_"));
            }
            Console.WriteLine("Gathered {0} pages to find the answer.", Articles.GetNumberOfArticles());
        }
        else {
            Console.WriteLine("Answer was not found :(");
            Console.WriteLine("You Win.");
        }
    }
}