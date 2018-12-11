////////////
// File Name: ArticleCollection.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 3, 2018
// Copyright (C) 2018 Michael Gamlem III
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// this is the object that acts as the database for the program
public class ArticleCollection {
    // used to store all webpages
    private ConcurrentDictionary<string, Webpage> dictionary = new ConcurrentDictionary<string, Webpage>();
    // used to store links for the article crawler to download the data for
    public ConcurrentQueue<Webpage> LinksToCrawl = new ConcurrentQueue<Webpage>();
    // used to store links that I do not want to store in the database
    private ForbiddenLinks ForbiddenLinksCollection;
    
    // constructor
    public ArticleCollection(ForbiddenLinks l) {
        ForbiddenLinksCollection = l;
    }

    // used to add webpage to the database
    // inputs: Webpage object to add
    // will also add the webpage to the article crawler's queue
    public void AddWebpage(Webpage w) {
        Console.WriteLine("adding webpage "+w.Title);
        dictionary.TryAdd(w.Title, w);
        LinksToCrawl.Enqueue(w);
    }

    // returns number of articles in database
    public double GetNumberOfArticles() { return dictionary.Count; }

    // used to see if webpage is in the database
    // inputs: requested page name, parent of requested page (can be null)
    // outputs: webpage object or null
    // note: will try to get webpage if not in dictionary before returning null
    public Webpage GetWebpage(string requestedPage, Webpage parent) {
        Webpage w = null;
        bool success = dictionary.TryGetValue(requestedPage.Replace("/wiki/", "").ToLower(), out w);
        if (!success) {
            success = TryToGetWebpage(requestedPage, parent);
        }
        if (success) {
            dictionary.TryGetValue(requestedPage.Replace("/wiki/", "").ToLower(), out w);
            return w;
        }
        return null;
    }

    // creates request to get webpage
    // note: only to be used within this class, use GetWebpage external to class
    private bool TryToGetWebpage(string requestedPage, Webpage parent) {
        WikipediaWebRequest r = new WikipediaWebRequest(requestedPage, this, ForbiddenLinksCollection, parent);
        if (r == null) {
            return false;
        }
        else {
            return true;
        }
    }

    // checks to see if webpage contins the link to the page we are searching for
    // input: string for webpage to check and the answer page
    // output: bool
    public bool WebpageContainsLinkToAnswer(string webpage, string answer) {
        Webpage w = WebpageInDictionary(webpage);
        if (w != null) {
            foreach (var value in w.Links) {
                if (answer.ToLower() == value.Key.ToLower()) {
                    return true;
                }
            }
        }
        return false;
    }

    // used to see if webpage is in dictionary
    // input: requested page name
    // output: webpage object or null
    // note: will not try to get webpage if it is not in the dictionary
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
}