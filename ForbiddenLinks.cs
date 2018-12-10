////////////
// File Name: Program.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: December 9, 2018
// Refer to License for Use Terms
////////////

using System.Collections.Generic;
using System.IO;

public class ForbiddenLinks
{
    private List<string> links = new List<string>();
    public ForbiddenLinks() {
        string[] lines = File.ReadAllLines("./ForbiddenLinks.txt");
        foreach (string s in lines) {
            links.Add(s);
        }
    }

    public bool CheckLink(string link) {
        foreach (string s in links) {
            if (link.Contains(s)) {
                return false;
            }
        }
        return true;
    }
}