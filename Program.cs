////////////
// File Name: Program.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Refer to License for Use Terms
////////////

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Wikipedia_Race
{
    class Program
    {
        // search for the finishing website
        void searchForWebsite(string StartPageName, string FinishPageName, ArticleCollection Webpages, ForbiddenLinks Forbidden) {
            // get data for first webpage
            WikipediaWebRequest request = new WikipediaWebRequest("wiki/"+StartPageName, Webpages, Forbidden, null);          
            if (request.SuccessfulWebRequest()) {
                WikipediaSearcher s = null;
                ArticleCrawler c = new ArticleCrawler(Webpages, Forbidden);
                Thread crawler = new Thread(() => c.Start());
                crawler.Start();
                crawler.Priority = ThreadPriority.Normal;

                Console.WriteLine("Creating Searcher....");
                Thread searcher = new Thread(() => {s = new WikipediaSearcher(Webpages, StartPageName, FinishPageName, Forbidden, "BFS");});

                searcher.Start();
                searcher.Priority = ThreadPriority.AboveNormal;
                    
                searcher.Join();

                try {
                    crawler.Abort();
                }
                catch (PlatformNotSupportedException) {
                    Console.WriteLine("System does not support abort of threads.");
                }
            }
        }

        private bool checkIfPageExists(string page) {
            HttpStatusCode status;
            try {   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.wikipedia.org/wiki/"+page);
                request.Method = WebRequestMethods.Http.Get;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                status = response.StatusCode;
                return status.Equals(HttpStatusCode.OK);
            }
            catch (WebException e) {
                Console.WriteLine("Exception caught during checking if webpage exists: " + e);
            }
            return false;
        }

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
        // async void Run() {
        void Run() {
            // stores list of links not to search
            ForbiddenLinks ForbiddenLinksCollection = new ForbiddenLinks();
            // stores master list of webpages
            ArticleCollection Articles = new ArticleCollection(ForbiddenLinksCollection);
            // Task<bool> StartExists;
            bool StartExists;
            // Task<bool> FinishExists;
            bool FinishExists;
            string StartPageName;
            string FinishPageName;
            
            // Ask for start page
            Console.WriteLine("Welcome\nWhich Wikipedia Page would you like to begin at?");
            // StartPageName = Console.ReadLine();
            StartPageName = "United States";
            StartPageName.Replace(' ', '_');

            // check if start page exists
            // StartExists =  checkIfPageExistsAsync(StartPageName);
            StartExists =  checkIfPageExists(StartPageName);

            // Ask for end page
            Console.WriteLine("Which Wikipedia Page would you like to end with?");
            // FinishPageName = Console.ReadLine();
            FinishPageName = "Dog";
            FinishPageName.Replace(' ', '_');
            
            // check if finish page exists
            // FinishExists =  checkIfPageExistsAsync(FinishPageName);
            FinishExists =  checkIfPageExists(FinishPageName);

            // if (await StartExists == true && await FinishExists == true) {
            if (StartExists && FinishExists) {
                // WikipediaWebRequest request = new WikipediaWebRequest(StartPageName, ref Webpages);
                searchForWebsite(StartPageName, FinishPageName, Articles, ForbiddenLinksCollection);
            }
            else {
                Console.WriteLine("Unknown Error");
            }

            Console.WriteLine("Terminating Program");
        }
        static void Main(string[] args)
        {
            Program P = new Program();
            P.Run();
        }
    }
}
