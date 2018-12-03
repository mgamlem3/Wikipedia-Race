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
public class WikipediaSearcher
{
    private Stack<Webpage> PathTaken = new Stack<Webpage>();
    private string StartPage;
    private string FinishPage;

    private ArticleCollection Articles;

    WikipediaSearcher() {}

    public WikipediaSearcher(ArticleCollection Webpages, string Start, string Finish) {
        StartPage = Start;
        FinishPage = Finish;
        Articles = Webpages;
        Search();
    }

    private void Search() {
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
                    if (PathTaken.Count >= 30) {
                        PathTaken.Pop();
                    }
                } catch (NullReferenceException e) {
                    Console.WriteLine("No webpages have been located yet. " + e);
                    WikipediaWebRequest r = new WikipediaWebRequest(StartPage, Articles);
                    currentPage = Articles.GetWebpage(StartPage);
                }
        }

        PrintResults(answerFound);
    }

    private bool checkIfAnswerFound(Webpage page) {
        Webpage temp = Articles.WebpageInDictionary(FinishPage);
        if (temp == null) {
            return false;
        }
        else {
            return true;
        }
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