////////////
// File Name: Program.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 9, 2018
// Refer to License for Use Terms
////////////

using System.Collections.Generic;
using System.IO;

// used to store all links that should not be saved in database
public class ForbiddenLinks
{
    private List<string> links = new List<string>();
    
    // constructor
    public ForbiddenLinks() {
        // retrieve links from file
        string[] lines = File.ReadAllLines("./ForbiddenLinks.txt");
        foreach (string s in lines) {
            links.Add(s);
        }
    }

    // used to compare string against forbidden links
    // input: string to check
    // output: true if okay, false if not
    public bool CheckLink(string link) {
        foreach (string s in links) {
            if (link.Contains(s)) {
                return false;
            }
        }
        return true;
    }
}