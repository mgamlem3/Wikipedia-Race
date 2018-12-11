////////////
// File Name: Webpage.cs
// Project: Wikipedia Race
// Created By: Michael Gamlem III
// Created On: November 28, 2018
// Copyright (C) 2018 Michael Gamlem III
// Refer to License for Use Terms
////////////

// used to store data that is passed into thread for ArticleCrawler
public class GetPageObject
{
    public Webpage Page {get; set;}
    public string Str {get; set;}
    public GetPageObject() {}
}