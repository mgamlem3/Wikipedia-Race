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

    WikipediaSearcher() {}

    WikipediaSearcher(ref ConcurrentDictionary<string, Webpage> Webpages, string Start, string Finish) {
        StartPage = Start;
        FinishPage = Finish;
        Search(ref Webpages);
    }

    private void Search(ref ConcurrentDictionary<string, Webpage> Webpages) {
        Webpage currentPage, nextPage;
        string nextPageString;
        bool successfulTake, answerFound = false;

        Webpages.TryGetValue(StartPage, out currentPage);
        PathTaken.Push(currentPage);

        while(PathTaken.Count > 0) {
            currentPage = PathTaken.Peek();
            
            // get next page to be searched
            successfulTake = currentPage.WebpagesToBeSearched.TryTake(out nextPageString);

            if (successfulTake) {
                Webpages.TryGetValue(nextPageString, out nextPage);
                PathTaken.Push(nextPage);
                answerFound = checkIfAnswerFound(nextPage, ref Webpages);
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
        }

        PrintResults(answerFound);
    }

    private bool checkIfAnswerFound(Webpage page, ref ConcurrentDictionary<string, Webpage> Webpages) {
        Webpage temp;
        
        return Webpages.TryGetValue(FinishPage, out temp);
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