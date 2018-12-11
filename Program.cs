// File Name: Program.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Copyright (C) 2018 Michael Gamlem III
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Wikipedia_Race
{
    // program object to be called
    class Program
    {
        // search for the finishing website
        // inputs: strings for start and end, ArticleCollection to store webpages in, ForbiddenLinks object
        void searchForWebsite(string StartPageName, string FinishPageName, ArticleCollection Webpages, ForbiddenLinks Forbidden) {
            // get data for first webpage
            WikipediaWebRequest request = new WikipediaWebRequest("wiki/"+StartPageName, Webpages, Forbidden, null);          
            
            // successful request, start program
            if (request.SuccessfulWebRequest()) {
                WikipediaSearcher s = null;
                
                // create article crawler
                ArticleCrawler c = new ArticleCrawler(Webpages, Forbidden);
                Thread crawler = new Thread(() => c.Start());
                crawler.Start();
                crawler.Priority = ThreadPriority.Normal;

                // create searcher
                Console.WriteLine("Creating Searcher....");
                Thread searcher = new Thread(() => {s = new WikipediaSearcher(Webpages, StartPageName, FinishPageName, Forbidden, "BFS");});
                searcher.Start();
                searcher.Priority = ThreadPriority.AboveNormal;

                // wait for searcher to complete   
                searcher.Join();

                // try to stop crawler
                try {
                    crawler.Abort();
                }
                catch (PlatformNotSupportedException) {
                    // TODO: investigate this further
                    Console.WriteLine("System does not support abort of threads. Please terminate manually");
                }
            }
        }

        // used to check if page that user wants to search for exists
        // input: page to search for
        // output: bool to tell if page exists or not
        private bool checkIfPageExists(string page) {
            HttpStatusCode status;
            
            // try to navigate to webpage
            try {   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.wikipedia.org/wiki/"+page);
                request.Method = WebRequestMethods.Http.Get;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                status = response.StatusCode;
                return status.Equals(HttpStatusCode.OK);
            }
            // something broke on the internet :(
            catch (WebException e) {
                Console.WriteLine("Exception caught during checking if webpage exists: " + e);
            }
            return false;
        }

        // unfinished async check for if page exists
        [Obsolete("This method has not been completed yet, please use the syncronous method.")]
        private async Task<bool> checkIfPageExistsAsync(string page) {
            HttpStatusCode status;
            try {   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.wikipedia.org/wiki/"+page);
                request.Method = WebRequestMethods.Http.Get;
                // response = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult),(object)null);
                WebResponse response = await request.GetResponseAsync();
                HttpWebResponse http_response = (HttpWebResponse)response;
                status = http_response.StatusCode;
                return status.Equals(HttpStatusCode.OK);
            }
            catch (WebException e) {
                Console.WriteLine("Exception caught during checking if webpage exists: " + e);
            }
            return false;
            
        }

        // used to run the program
        void Run() {
            // stores list of links not to search
            ForbiddenLinks ForbiddenLinksCollection = new ForbiddenLinks();
            // stores master list of webpages
            ArticleCollection Articles = new ArticleCollection(ForbiddenLinksCollection);
            bool StartExists;
            bool FinishExists;
            string StartPageName;
            string FinishPageName;
            
            // Ask for start page
            Console.WriteLine("Welcome\nWhich Wikipedia Page would you like to begin at?");
            StartPageName = Console.ReadLine();
            // StartPageName = "United States";
            StartPageName.Replace(' ', '_');

            // check if start page exists
            StartExists =  checkIfPageExists(StartPageName);

            // Ask for end page
            Console.WriteLine("Which Wikipedia Page would you like to end with?");
            FinishPageName = Console.ReadLine();
            // FinishPageName = "Federation";
            FinishPageName.Replace(' ', '_');
            
            // check if finish page exists
            FinishExists =  checkIfPageExists(FinishPageName);

            if (StartExists && FinishExists) {
                searchForWebsite(StartPageName, FinishPageName, Articles, ForbiddenLinksCollection);
            }
            else {
                Console.WriteLine("Unknown Error");
            }

            Console.WriteLine("Terminating Program");
        }

        // entry point for .NET
        static void Main(string[] args)
        {
            Program P = new Program();
            P.Run();
        }
    }
}
