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
    private bool exit = false;
    private int threadCount = 0;
    private const int MAXTHREADS = 10;

    private ArticleCrawler() {}

    public ArticleCrawler(ArticleCollection c) {
        Articles = c;
    }

    public void Start() {
        while (!exit) {
            Crawl();
        }
        Console.WriteLine("exiting crawler");
    }

    private void Crawl() {
        foreach (Webpage w in Articles.GetLinksToCrawl()) {
            foreach (string link in w.Links.Keys) {
                Webpage t = Articles.WebpageInDictionary(link);
                if (t != null) {
                    continue;
                }
                else {
                    bool threadCreated = false;
                    while (threadCreated == false) {
                        if (threadCount >= MAXTHREADS) {
                            Thread.Sleep(2000);
                            continue;
                        }
                        try {
                            WaitCallback callback = new WaitCallback(GetPage);
                            ThreadPool.QueueUserWorkItem(callback, link);
                            threadCreated = true;
                            threadCount++;
                        } catch (OutOfMemoryException e) {
                            Console.WriteLine("Not enough memory to create thread... Will wait and try again..." + e);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            Console.WriteLine("sleeping");
            Thread.Sleep(10000);
        }
    }

    private void GetPage(object s) {
        string str = s.ToString();
        Webpage w = Articles.WebpageInDictionary(str);
        if (w == null) {
            Console.WriteLine("requesting: "+str);
            WikipediaWebRequest r = new WikipediaWebRequest(str, Articles);
            threadCount--;
        }
    }

    public void QuitArticleCrawler() {exit = true;}
}