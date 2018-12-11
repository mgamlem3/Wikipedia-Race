////////////
// File Name: WikipediaSearcher.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 1, 2018
// Copyright (C) 2018 Michael Gamlem III
// Refer to License for Use Terms
////////////

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

// used to search for answer
public class WikipediaSearcher
{
    // stores path taken in DFS search
    private Stack<Webpage> PathTaken = new Stack<Webpage>();
    private static string StartPage;
    private static string FinishPage;
    // article collection with articles that have been found
    private static ArticleCollection Articles;
    // used to time how long execution takes
    private Stopwatch watch = new Stopwatch();
    private ForbiddenLinks ForbiddenLinksCollection;

    // default constructor, unused
    WikipediaSearcher() {}

    // constructor
    // note: must specify which search type to use
    public WikipediaSearcher(ArticleCollection Webpages, string Start, string Finish, ForbiddenLinks l, string searchType) {
        StartPage = Start;
        FinishPage = Finish;
        Articles = Webpages;
        ForbiddenLinksCollection = l;

        // depth first
        if (searchType == "DFS") {        
            watch.Start();
            SearchDFS();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
        // breadth first
        else if (searchType == "BFS") {
            watch.Start();
            SearchBFS();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms");
        }
        else {
            Console.WriteLine("Please enter either DFS or BFS to indicate which search type you prefer.");
        }
    }

    // breadth first search
    private void SearchBFS() {
        // variables
        Webpage currentPage;
        bool answerFound = false;
        string answer = "";
        Queue<string> q = new Queue<string>();
        
        // add something to the queue
        q.Enqueue(StartPage);
        // get first page
        currentPage = Articles.GetWebpage(StartPage, null);

        // search
        while (q.Count > 0) {
            // get vertex to search
            string vertex = q.Dequeue();
            currentPage = Articles.GetWebpage(vertex, currentPage);
            
            // if webpage has not been found, put it back on queue and search next one
            if (currentPage == null) {
                q.Enqueue(vertex);
                continue;
            }

            // if webpage has been downloaded
            foreach (var link in currentPage.Links) {
                // check to see if this is the answer
                answerFound = checkIfAnswerFound(link.Key.Remove(0,6).ToLower());
                if (answerFound) {
                    answer = link.Key;
                    break;
                }
                // not the answer, put it on the queue to be searched later
                else {
                    q.Enqueue(link.Key.Remove(0,6));
                }
            }
            if (answerFound) {
                break;
            }
        }
        // done, print results
        PrintResultsBFS(answerFound, answer, currentPage);
    }

    // depth first search
    // slow
    // may contain bugs... abandoned when realized BFS was better
    private void SearchDFS() {
        Webpage currentPage, nextPage;
        string nextPageString;
        bool successfulTake, answerFound = false;

        currentPage = Articles.GetWebpage(StartPage, null);
        if (currentPage != null) {
            PathTaken.Push(currentPage);
        }

        while(PathTaken.Count > 0) {
            currentPage = PathTaken.Peek();
                // get next page to be searched
                try {
                    successfulTake = currentPage.WebpagesToBeSearched.TryDequeue(out nextPageString);

                    if (successfulTake) {
                        // get next page down
                        nextPage = Articles.GetWebpage(nextPageString, currentPage);
                        // don't search pages that are not in database or that are the page I am currently on
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
                    WikipediaWebRequest r = new WikipediaWebRequest(StartPage, Articles, ForbiddenLinksCollection, currentPage);
                    currentPage = Articles.GetWebpage(StartPage, null);
                }
        }
        PrintResultsDFS(answerFound);
    }

    // used to determine if answer has been found
    // input: webpage
    // output: bool
    private bool checkIfAnswerFound(Webpage page) {
        // is the current page the answer?
        Console.WriteLine("Checking For Answer");
        Webpage temp = Articles.WebpageInDictionary(FinishPage);
        bool found = false;
        // answer found
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

    // used to see if webpage is the answer by page title
    // input: page title
    // output bool
    private bool checkIfAnswerFound(string page) {
        Console.WriteLine("Checking for Answer: {0}", page);
        // is the current page the answer?
        if (page.ToLower() == FinishPage.ToLower()) {
            return true;
        }
        else {
            return false;
        }
    }

    // output results of DFS search
    private void PrintResultsDFS(bool answerFound) {
        if (answerFound) {
            Console.WriteLine("Answer Found!");
            Array answerPath = PathTaken.ToArray();
            // print path taken
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

    // used to print results of BFS
    // input: if answer was found, answer string, parent webpage
    // note: last two may be null if failure
    private void PrintResultsBFS(bool answerFound, string answer, Webpage parent) {
        if (answerFound) {
            string answerText = "";
            string current = answer;
            Webpage w = parent;
            List<string> path = new List<string>();
            path.Add(answer.Remove(0,6));
            current = parent.Title;
            while (current != null) {
                path.Add(current);                
                // while (w == null) {
                //     w = Articles.GetWebpage(current, parent);
                // }
                w = w.GetParent();
                current = w?.Title;
            }
            answerText += "\n\nAnswer Found!\n";
            foreach (var s in path) {
                answerText += "/wiki/" + s.Replace(" ", "_") + "\n";
            }
            Console.WriteLine(answerText+"\n");
        }
        // TODO: page not found
            // should thoretically not happen unless you wait a REALLY long time
    }
}