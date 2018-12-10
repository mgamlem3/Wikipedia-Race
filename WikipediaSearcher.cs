////////////
// File Name: WikipediaSearcher.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 1, 2018
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
public class WikipediaSearcher
{
    private Stack<Webpage> PathTaken = new Stack<Webpage>();
    private string StartPage;
    private string FinishPage;
    private ArticleCollection Articles;
    private Stopwatch watch = new Stopwatch();
    private ForbiddenLinks ForbiddenLinksCollection;

    WikipediaSearcher() {}

    public WikipediaSearcher(ArticleCollection Webpages, string Start, string Finish, ForbiddenLinks l) {
        StartPage = Start;
        FinishPage = Finish;
        Articles = Webpages;
        ForbiddenLinksCollection = l;
        
        watch.Start();
        SearchDFS();
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds);
    }

    private void SearchBFS() {
        Webpage currentPage, nextPage;
        string nextPageString;
        bool successfulTake, answerFound = false;

        currentPage = Articles.GetWebpage(StartPage);
        // get first page
            // pop all webpages to be searched
            // put on q
                // when webpage exhausted push to path taken
                    // pop off q and search that page pages to be searched
                    // put on q
                    // when webpage exhausted put on q

        while (currentPage != null) {
            PathTaken.Push(currentPage);
            foreach (string w in currentPage.WebpagesToBeSearched) {
                nextPage = Articles.GetWebpage(w);
                if (nextPage != null) {
                    answerFound = checkIfAnswerFound(nextPage);

                    if (answerFound) {
                        break;
                    }
                }
            }
            foreach (KeyValuePair<string, int> URL in currentPage.Links) {
                
            }
        }
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
                        if (nextPage == null) {
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
            return true;
        }
        else if (temp == null) {
            // does the current page contain the link to the answer?
            found = Articles.WebpageContainsLinkToAnswer(page.Title, FinishPage);
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
        }
        else {
            Console.WriteLine("Answer was not found :(");
            Console.WriteLine("You Win.");
        }
    }
}