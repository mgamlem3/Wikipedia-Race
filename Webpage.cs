////////////
// File Name: Webpage.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Refer to License for Use Terms
////////////

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
public class Webpage {
    public string Title { get; set; }
    private static int ProcessorCount = Environment.ProcessorCount;
    private static int ConcurrencyLevel = ProcessorCount * 2;
    private static int InitialDictionarySize = 50;
    public ConcurrentDictionary<string, int> Links = new ConcurrentDictionary<string, int>(ConcurrencyLevel, InitialDictionarySize);
    public ConcurrentQueue<string> WebpagesToBeSearched = new ConcurrentQueue<string>();
    private Webpage Parent;
    private int HopsFromParent;

    // default constructor
    // not used
     private Webpage() {}

    // constructor
    // input: title (string) of webpage & list of one or more links from the webpage
    // output: new webpage object
     public Webpage(string new_title, List<string> new_links, Webpage parent) {
         Title = new_title;
         if (parent == null) {
             HopsFromParent = 0;
         }
         else {
            HopsFromParent = parent.HopsFromParent + 1;
         }
         Parent = parent;
         addLinks(new_links);
     }

     public Webpage GetParent() {
         return Parent;
     }

    // appends one or more links to the list
    // inputs: list of links
    // outputs:
     private void addLinks(List<string> links_to_add) {
        for(var i = 0; i < links_to_add.Count; i++) {
            if (links_to_add[i].ToLower().Contains("wikipedia")) {
                continue;
            }
            string link = links_to_add[i].TrimEnd('_');
            Links.AddOrUpdate(link, 0, (key, oldValue) => oldValue++);
            WebpagesToBeSearched.Enqueue(link);
        }
     }
}