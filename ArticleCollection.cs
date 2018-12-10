////////////
// File Name: ArticleCollection.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 3, 2018
// Refer to License for Use Terms
////////////

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class ArticleCollection {
    private ConcurrentDictionary<string, Webpage> dictionary = new ConcurrentDictionary<string, Webpage>();
    private ConcurrentBag<Webpage> LinksToCrawl = new ConcurrentBag<Webpage>();
    private ForbiddenLinks ForbiddenLinksCollection;
    public ArticleCollection(ForbiddenLinks l) {
        ForbiddenLinksCollection = l;
    }

    public void AddWebpage(Webpage w) {
        Console.WriteLine("adding webpage "+w.Title);
        dictionary.TryAdd(w.Title, w);
        LinksToCrawl.Add(w);
    }

    public ConcurrentBag<Webpage> GetLinksToCrawl() {
        return LinksToCrawl;
    }

    public Webpage GetWebpage(string requestedPage) {
        Webpage w = null;
        bool success = dictionary.TryGetValue(requestedPage.Replace("/wiki/", "").ToLower(), out w);
        if (!success) {
            success = TryToGetWebpage(requestedPage);
        }
        if (success) {
            dictionary.TryGetValue(requestedPage.Replace("/wiki/", "").ToLower(), out w);
            return w;
        }
        return null;
    }
    
    public Webpage WebpageInDictionary(string requestedPage) {
        Webpage w = null;
        if (requestedPage.Contains("wiki/")) {
            requestedPage = requestedPage.Remove(0, 5);
        }
        requestedPage = requestedPage.Replace("_", " ").ToLower();
        bool success = dictionary.TryGetValue(requestedPage.ToLower(), out w);
        if (!success) {
            return null;
        }
        return w;
    }

    private bool TryToGetWebpage(string requestedPage) {
        WikipediaWebRequest r = new WikipediaWebRequest(requestedPage, this, ForbiddenLinksCollection);
        if (r == null) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool WebpageContainsLinkToAnswer(string webpage, string answer) {
        Webpage w = WebpageInDictionary(webpage);
        foreach (var value in w.Links) {
            if (answer.ToLower() == value.Key.ToLower()) {
                return true;
            }
        }
        return false;
    }
}