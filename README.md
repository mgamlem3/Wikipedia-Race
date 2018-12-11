# About
This Project was created for CS 315: Distributed Scalable Computing at Whitworth University Fall 2018.

**Current Version: 1.0**

_**All work unless otherwise stated is completed by Michael Gamlem III.**_

Project started on Wednesday, November 28, 2018.

# Important License Information
**Copyright (C) 2018 Michael Gamlem III**

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

See License for full terms

## Contact Information
Please open an issue on GitHub if you need to contact me for any reason.

# Description
Program that will attempt to reach a Wikipedia page from another start page by only using the links between pages.

# Overview of Design
One program class is created that manages the entire program. This class will spawn one `WikipediaSearcher` and one `ArticleCrawler` after verifying that the beginning and ending pages exist. Additionally, one `ArticleCollection` will be spawned to act as a database for the run.

## WikipediaSearcher
This thread will spawn and search through the database in either a depth-first or breadth-first manner.

> ### How to Change Search Type:
> 1) Open `Program.cs`
> 2) Go to where WikipediaSearcher is created
>     * Currently line 38
> 3) Change string at end
>     * Options are `BFS` and `DFS`

## ArticleCrawler
> **Important Note:** Only one should be created per program. Creating more than one could lead to deadlock due to insufficient number of threads available in the thread pool.

The ArticleCrawler will take from the queue of webpages to be crawled in the ArticleCollection. Once a webpage is popped, it will be queued in the threadpool to be downloaded (if it does not already exist in the dictionary). Once that webpage has been downloaded and parsed, all of its links will be added to the queue.

## ArticleCollection
This acts as the database for the current program run. Currently, the database is intentionally discarded at the end of each program run. This was chosen so that the program closely emulated the process that a human would follow.

This class levereges `System.Collections.Generic` and `System.Collections.Concurrent` data structures to house all the data for each program execution. Adding and modification of `Webpages` stored in the database is threadsafe because the `ConcurrentDictionary` is used to store all entries.

# How To Compile
1) Ensure .NET is installed on your host machine.
    * Read the [install guide](https://docs.microsoft.com/en-us/dotnet/framework/install/) provided by Microsoft.
2) Navigate to project folder in termina and type `dotnet run`.

# Known Issues
- Async page validation upon start is broken
- Intermittent duplicate requests to download a page
- Unimplemented dynamic thread pool size calculation

# References
- [Microsoft .NET docs](https://docs.microsoft.com/en-us/dotnet/framework/)
