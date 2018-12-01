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
    public ConcurrentDictionary<string, string> Links = new ConcurrentDictionary<string, string>(ConcurrencyLevel, InitialDictionarySize);
    public ConcurrentBag<string> WebpagesToBeSearched = new ConcurrentBag<string>();
    private static Random rng = new Random();
    private List<Thread> threads = new List<Thread>();

    // default constructor
    // not used
     public Webpage() {}

    // constructor
    // input: title (string) of webpage & list of one or more links from the webpage
    // output: new webpage object
     public Webpage(string new_title, List<string> new_links, ref ConcurrentDictionary<string, Webpage> Webpages) {
         Title = new_title;
         addLinks(new_links, ref Webpages);
         for (var i = 0; i < threads.Count; i++) {
             threads[i].Join();
         }
     }

    // appends one or more links to the list
    // inputs: list of links
    // outputs: 
    // side-effect: adds link to link list for object
     private void addLinks(List<string> links_to_add, ref ConcurrentDictionary<string, Webpage> Webpages) {
        for(var i = 0; i < links_to_add.Count; i++) {
            string link = links_to_add[i];
            Links.TryAdd(link.ToString(), "https://en.wikipedia.org/wiki/"+link);
            WebpagesToBeSearched.Add(link);

            ParameterizedThreadStart start = new ParameterizedThreadStart(crawlPage);
            Thread thread = new Thread(crawlPage);
            threads.Add(thread);
            threads[i].Start(Webpages);
        }
     }

     static void crawlPage(object _webpages) {
         
     }
}