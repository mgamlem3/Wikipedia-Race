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
public class WikipediaSearcher
{
    private Stack<Webpage> PathTaken = new Stack<Webpage>();
    private string StartPage;
    private string FinishPage;
    private ArticleCollection Articles;
    private Stopwatch watch = new Stopwatch();

    WikipediaSearcher() {}

    public WikipediaSearcher(ArticleCollection Webpages, string Start, string Finish) {
        StartPage = Start.ToLower();
        FinishPage = Finish.ToLower();
        Articles = Webpages;
        
        watch.Start();
        Search();
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds);
    }

    private void Search() {
        ArticleCrawler crawler = new ArticleCrawler(Articles);
        Webpage currentPage, nextPage;
        string nextPageString;
        bool successfulTake, answerFound = false;

        currentPage = Articles.GetWebpage(StartPage);
        PathTaken.Push(currentPage);

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
                    WikipediaWebRequest r = new WikipediaWebRequest(StartPage, Articles);
                    currentPage = Articles.GetWebpage(StartPage);
                }
        }
        crawler.QuitArticleCrawler();
        PrintResults(answerFound);
    }

    private bool checkIfAnswerFound(Webpage page) {
        // is the current page the answer?
        Webpage temp = Articles.WebpageInDictionary(FinishPage.ToLower());
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
                Console.WriteLine(page.Title);
            }
        }
        else {
            Console.WriteLine("Answer was not found :(");
            Console.WriteLine("You Win.");
        }
    }
}