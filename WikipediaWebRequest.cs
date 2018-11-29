////////////
// File Name: WikipediaWebRequest.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Refer to License for Use Terms
////////////

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Used to request webpage from Wikipedia
// Will parse and store HTML links and page title 
public class WikipediaWebRequest
{
    private string RequestedPage;
    private bool Success = false;

    private const string TITLE_REGEX_PATTERN = @"(?:<h1[\w\d\s=\D]+>)([\w\d\s]+)(?:<\/h1>)";
    private const string LINK_REGEX_PATTERN =  @"(?:<a href=\D\/wiki\/)([\w]+)";
    private Regex TITLE_REGEX = new Regex(TITLE_REGEX_PATTERN, RegexOptions.IgnoreCase);

    public WikipediaWebRequest() {}

    public WikipediaWebRequest(string requested_page, ref List<Webpage> webpages) {
        RequestedPage = requested_page;
        Controller(ref webpages);
    }

    private void Controller(ref List<Webpage> webpages) {
        string response = NavigateToWebpage();
        if (response != null) {
            ParseResposne(response, ref webpages);
        }
    }

    // navigates to webpage and retrieves HTML
    // input: 
    // output: string of resposne
    private string NavigateToWebpage() {
        try {
            WebClient client = new WebClient();
            string response = client.DownloadString("https://en.wikipedia.org/wiki/"+RequestedPage);
            
            if (response != null || response != "") {
                Success = true;
                return response;
            }

            return null;

        }
        catch (WebException e) {
            Console.WriteLine("\nWebException Caught!");
            Console.WriteLine("Message: " + e.ToString());
            return null;
        }
        catch (ArgumentNullException e) {
            Console.WriteLine("\nArgumentNullExceptionCaught!");
            Console.WriteLine("Message: " + e.ToString());
            return null;
        }
    }

    // searches response HTML for data that will be used
    //    links and page title
    // input: raw HTML response, reference to master webpage list
    // output: 
    // creates new webpage if successfully parsed
    private void ParseResposne(string responseHTML, ref List<Webpage> webpages) {
        Match title_match = TITLE_REGEX.Match(responseHTML);
        MatchCollection links_match = Regex.Matches(responseHTML, LINK_REGEX_PATTERN);//LINK_REGEX.Match(responseHTML);

        Console.WriteLine("Title Matches");
        // TODO: only take one match
        for (var i = 0; i < title_match.Length; i++) {
            Console.WriteLine(title_match.Groups[i].ToString());
        }


        List<string> unordered_links = new List<string>();

        Console.WriteLine("Link Matches");
        // gather all matches from webpage
        foreach (Match match in links_match) {
            // gather all match groups from current match
            GroupCollection groups = match.Groups;
            // only save the matching group, discard non-matching groups
            unordered_links.Add(groups[1].ToString());
            // Console.WriteLine(groups[1].ToString());
        }
        Webpage newWebpage = new Webpage(title_match.ToString(), unordered_links);

        // TODO: add webpage to master list
    }

    // used to determine if webpage naviagation was successful
    public bool SuccessfulWebRequest() => Success;
}