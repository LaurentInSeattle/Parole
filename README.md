## Parole Word Game
A clone of the now famous Wordle game, but in Italian, with your choice of 5 or 6 letter words.

## License 
This software is free. 
This software is proposed to you under the terms of the MIT license. For details, check out the LICENCSE document on this webpage.

## Prerequisites 

### Operating System  
'Parole' will only work on Windows 10 or Windows 11 64 bit edition.
There is no plan to port to the web, Windows 32 bit, MacOS, iOS, Android, and any variation of Unix.

### .Net Runtime
'Parole' requires the installation of the .Net runtime version 8.0 that you can download from Microsoft, from this page, here: 
[.Net 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

You will need to install the "For Windows" "x64" ".NET Desktop Runtime 8.0.x" software component.
Scroll down the webpage to find the link usually located in the right column.

## Install
There is no installation "Wizard". Just follow the easy steps described below: 

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
Note: The program uses Microsoft WebView2 software component to lookup the web for words definitions. 
It will create a dedicated folder in the program folder: "Parole.exe.WebView2", this is normal.

### Build your own... Make changes, fix a bug...
All you need is to install the free Visual Studio 2022 Community Edition, or the Pro or Enterprise. 
Clone the repo', open the solution, build, etc.

### Tips
- Parole creates an "history" file in the following location: 
C:\Users\\<YourAccountName>\AppData\Local\Lyt\Parole
Deleting this file will clear all statistics and reset the word lists.

- You can play the game offline. Just close the web view to avoid the 404 lookups...
