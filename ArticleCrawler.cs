////////////
// File Name: ArticleCrawler.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 4, 2018
// Refer to License for Use Terms
////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ArticleCrawler
{
    private ArticleCollection Articles;
    private ForbiddenLinks ForbiddenLinksCollection;
    private bool exit = false;
    private int threadCount = 0;
    private const int MAXTHREADS = 100;

    private ArticleCrawler() {}

    public ArticleCrawler(ArticleCollection c, ForbiddenLinks l) {
        Articles = c;
        ForbiddenLinksCollection = l;
    }

    public void Start() {
        while (!exit) {
            Crawl();
        }
        Console.WriteLine("exiting crawler");
    }

    private void Crawl() {
        while (!Articles.LinksToCrawl.IsEmpty) {
            Webpage w;
            GetPageObject pageObject = new GetPageObject();
            Articles.LinksToCrawl.TryDequeue(out w);

            while (!w.WebpagesToBeSearched.IsEmpty) {
                string temp = "";
                bool successfulTake = false;
                
                while (!successfulTake) {
                    successfulTake = w.WebpagesToBeSearched.TryDequeue(out temp);
                }

                Webpage t = Articles.WebpageInDictionary(temp);
                if (t != null) {
                    continue;
                }
                else {
                    bool threadCreated = false;
                    while(!threadCreated) {
                        if (threadCount >= MAXTHREADS) {
                            w.WebpagesToBeSearched.Enqueue(temp);
                            Thread.Sleep(2000);
                        }
                        try {
                            WaitCallback callback = new WaitCallback(GetPage);
                            pageObject.Page = w;
                            pageObject.Str = temp;
                            ThreadPool.QueueUserWorkItem(callback, pageObject);
                            threadCreated = true;
                            threadCount++;
                            Thread.Sleep(200);
                        }
                        catch (OutOfMemoryException e) {
                            Console.WriteLine("Not enough memory to create thread... Will wait and try again..." + e);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }
    }

    private void GetPage(object o) {
        GetPageObject obj = (GetPageObject)o;
        string _str = obj.Str;
        Webpage parent = obj.Page;
        Webpage w = Articles.WebpageInDictionary(_str);
        if (w == null) {
            Console.WriteLine("requesting: "+_str);
            WikipediaWebRequest r = new WikipediaWebRequest(_str, Articles, ForbiddenLinksCollection, parent);
            threadCount--;
        }
    }

    public void QuitArticleCrawler() {exit = true;}
}