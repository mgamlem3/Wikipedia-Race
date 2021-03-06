////////////
// File Name: Webpage.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Copyright (C) 2018 Michael Gamlem III
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

// Webpage object that stores Wikipedia pages
public class Webpage {
    public string Title { get; set; }
    ////
    // used to determine how big the dictionary should be
        private static int ProcessorCount = Environment.ProcessorCount;
        private static int ConcurrencyLevel = ProcessorCount * 2;
        private static int InitialDictionarySize = 50;
    ////
    // stores links that website has to other websites
    public ConcurrentDictionary<string, int> Links = new ConcurrentDictionary<string, int>(ConcurrencyLevel, InitialDictionarySize);
    // stores websites that have not been verified to be in the database yet
    public ConcurrentQueue<string> WebpagesToBeSearched = new ConcurrentQueue<string>();
    // Webpage that linked to this page
    private Webpage Parent;
    // used to store how many hops from the parent we are
    // TODO: maybe use to implement custom concurrent priority queue
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

     // returns parent of page
     public Webpage GetParent() {
         return Parent;
     }
}