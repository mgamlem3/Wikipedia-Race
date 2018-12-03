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
    private List<object> ThreadReturn = new List<object>();

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
     private void addLinks(List<string> links_to_add, ref ConcurrentDictionary<string, Webpage> Webpages) {
        var _webpages = Webpages;
        for(var i = 0; i < links_to_add.Count; i++) {
            string link = links_to_add[i];
            Links.TryAdd(link.ToString(), "https://en.wikipedia.org/wiki/"+link);
            WebpagesToBeSearched.Add(link);
            // object o = null;
            // try {
            //     Thread thread = new Thread(() => { o = crawlPage(links_to_add, _webpages, i); });
            //     thread.Start();
                
            //     ThreadReturn.Add(o);
            //     threads.Add(thread);
            // } catch (OutOfMemoryException e) {
            //     Console.WriteLine("Not enough memory to create another thread." + e);
            // }
        }
        // foreach (Thread t in threads) {
        //     t.Join();
        // }
        // combineConcurrentDictionaries(ref Webpages);
     }

     private static object crawlPage(List<string> pages_to_search, ConcurrentDictionary<string, Webpage> _webpages, int tid) {
        for(var i = 0; i < 10; i++) {
            WikipediaWebRequest temp = new WikipediaWebRequest(pages_to_search[i], ref _webpages);
        }
        return _webpages;
     }

     private void combineConcurrentDictionaries(ref ConcurrentDictionary<string, Webpage> _webpages) {
         foreach (ConcurrentDictionary<string, Webpage> o in ThreadReturn) {
             foreach (var item in o) {
                 _webpages.AddOrUpdate(
                     item.Key,
                     item.Value,
                     (key, value) => value = item.Value
                 );
             }
         }
     }
}