## Parole Word Game
A clone of the now famous Wordle game, but in Italian, with your choice of 5 or 6 letter words.

## Prerequisites 

### Operating System  
Parole will only work on Windows 10 or Windows 11 64 bit edition.
There is no plan to port to the web, Windows 32 bit, Mac, iOS, Android, and any variation of Unix.

### .Net Runtime
Parole requires the installation of the .Net runtime version 8.0 that you can download from Microsoft, from this page, here: 
[.Net 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

You will need the "For Windows" "x64" ".NET Desktop Runtime 8.0.x", scroll down the webpage to find the link usualy placed on the right column.

## Install
There is no installation "wizard". Follow the easy steps below: 

Download the following zip archive: 
https://github.com/LaurentInSeattle/Parole/blob/master/Parole.zip
The zip file should be exactly 9,559,414 bytes. (9.11 MB)
Move the zip archive to a convenient location on your computer.
Extract the zip file. Do not try to run the executable without extracting first.
The extracted folder should only contain Parole.exe and WebView2Loader.dll.

Run (open or double-click) the Parole.Exe file.

### Security 
When running the game for the first time, it is likely that some Windows settings or some third party "security software" installed on your computer may try to block it.
Bypass - or not, the warnings...
The program uses WebView2 to lookup the web and a dedicated folder will be created in the program folder: "Parole.exe.WebView2", this is normal.

### Build your own... Make changes, fix a bug...
All you need if Visual Studio 2022 Community Edition. 
Clone the repo', open the solution, build, etc.

### Tips
Parole creates an "history" file in the following location: 
C:\Users\<YourAccountName>\AppData\Local\Lyt\Parole
Deleting this file will clear all statistics and reset the word lists.

You can play offline. Just close the web view to avoid the 404 lookups...