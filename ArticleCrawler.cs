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
public class ArticleCrawler
{
    private ArticleCollection Articles;
    private bool exit = false;

    private ArticleCrawler() {}

    public ArticleCrawler(ArticleCollection c) {
        Articles = c;
        Controller();
    }

    private void Controller() {
        while (!exit) {
            Crawl();
        }
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
                        try {
                            Console.WriteLine("passing: "+link);
                            ThreadPool.QueueUserWorkItem(GetPage, link);
                            threadCreated = true;
                        } catch (OutOfMemoryException e) {
                            Console.WriteLine("Not enough memory to create thread... Will wait and try again..." + e);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }
    }

    private void GetPage(object s) {
        string str = s.ToString();
        Webpage w = Articles.WebpageInDictionary(str);
        if (w == null) {
            Console.WriteLine("requesting: "+str);
            WikipediaWebRequest r = new WikipediaWebRequest(str, Articles);
        }
    }

    public void QuitArticleCrawler() {exit = true;}
}