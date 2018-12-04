////////////
// File Name: ArticleCollection.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 3, 2018
// Refer to License for Use Terms
////////////

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class ArticleCollection {
    private ConcurrentDictionary<string, Webpage> dictionary = new ConcurrentDictionary<string, Webpage>();
    public ArticleCollection() {}

    public void AddWebpage(Webpage w) {
        dictionary.TryAdd(w.Title, w);
    }

    public Webpage GetWebpage(string requestedPage) {
        Webpage w = null;
        bool success = dictionary.TryGetValue(requestedPage.ToLower().Replace("/wiki/", ""), out w);
        if (!success) {
            success = TryToGetWebpage(requestedPage);
        }
        if (success) {
            dictionary.TryGetValue(requestedPage.ToLower().Replace("/wiki/", ""), out w);
            return w;
        }
        return null;
    }
    
    public Webpage WebpageInDictionary(string requestedPage) {
        Webpage w = null;
        bool success = dictionary.TryGetValue(requestedPage.ToLower(), out w);
        if (!success) {
            return null;
        }
        return w;
    }
    // change to return title instead of bool
    private bool TryToGetWebpage(string requestedPage) {
        WikipediaWebRequest r = new WikipediaWebRequest(requestedPage.ToLower(), this);
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