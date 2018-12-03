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
        bool success = dictionary.TryGetValue(requestedPage, out w);
        if (!success) {
            success = tryToGetWebpage(requestedPage);
        }
        if (!success) {
            return null;
        }
        return w;
    }
    
    public Webpage WebpageInDictionary(string requestedPage) {
        Webpage w = null;
        bool success = dictionary.TryGetValue(requestedPage, out w);
        if (!success) {
            return null;
        }
        return w;
    }
    private bool tryToGetWebpage(string requestedPage) {
        WikipediaWebRequest r = new WikipediaWebRequest(requestedPage, this);
        if (r == null) {
            return false;
        }
        else {
            return true;
        }
    }
}