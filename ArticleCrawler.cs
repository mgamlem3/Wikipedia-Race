////////////
// File Name: ArticleCrawler.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 4, 2018
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// used to automatically download wikipedia pages
public class ArticleCrawler
{
    // article collection to add wikipedia articles to
    private ArticleCollection Articles;
    // list of links not to add
    private ForbiddenLinks ForbiddenLinksCollection;
    // used to tell crawler to quit
    private bool exit = false;
    // counts number of threads in use
    private int threadCount = 0;
    // max number o fthreads allowed
    private const int MAXTHREADS = 100;

    // default constructor, not used
    private ArticleCrawler() {}

    // constructor
    public ArticleCrawler(ArticleCollection c, ForbiddenLinks l) {
        Articles = c;
        ForbiddenLinksCollection = l;
    }

    // used to start article crawler, must be called after creation
    public void Start() {
        while (!exit) {
            Crawl();
        }
        Console.WriteLine("exiting crawler");
    }

    // action to perform for each link that is crawled
    private void Crawl() {
        while (!Articles.LinksToCrawl.IsEmpty) {
            // variables
            Webpage w;
            GetPageObject pageObject = new GetPageObject();
            Articles.LinksToCrawl.TryDequeue(out w);

            // while webpage still has pages that need to be searched
            while (!w.WebpagesToBeSearched.IsEmpty) {
                string temp = "";
                bool successfulTake = false;
                
                // keep trying until take is successful
                while (!successfulTake) {
                    successfulTake = w.WebpagesToBeSearched.TryDequeue(out temp);
                }

                // temp webpage to see if page already has been found
                Webpage t = Articles.WebpageInDictionary(temp);
                if (t != null) {
                    // webpage already downloaded
                    continue;
                }
                else {
                    bool threadCreated = false;
                    // try until thread can be created
                    while(!threadCreated) {
                        if (threadCount >= MAXTHREADS) {
                            // too many threads, put back and sleep for a few seconds
                            w.WebpagesToBeSearched.Enqueue(temp);
                            Thread.Sleep(2000);
                        }
                        // try to queue thread in thread pool
                        try {
                            WaitCallback callback = new WaitCallback(GetPage);
                            // set information for thread
                            pageObject.Page = w;
                            pageObject.Str = temp;
                            // queue thread
                            ThreadPool.QueueUserWorkItem(callback, pageObject);
                            threadCreated = true;
                            threadCount++;
                            // slow down crawler to attempt to avoid race conditions and multiple threads searching for same article
                            Thread.Sleep(200);
                        }
                        catch (OutOfMemoryException e) {
                            // :(
                            Console.WriteLine("Not enough memory to create thread... Will wait and try again..." + e);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }
    }

    // function given to thread
    // input: GetPageObject
    private void GetPage(object o) {
        // cast generic object back to GetPageObject
        GetPageObject obj = (GetPageObject)o;
        string _str = obj.Str;
        Webpage parent = obj.Page;

        // check again to see if webpage alread in dictionary
        Webpage w = Articles.WebpageInDictionary(_str);
        if (w == null) {
            // download webpage
            Console.WriteLine("requesting: "+_str);
            WikipediaWebRequest r = new WikipediaWebRequest(_str, Articles, ForbiddenLinksCollection, parent);
            threadCount--;
        }
    }

    public void QuitArticleCrawler() {exit = true;}
}