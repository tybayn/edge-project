# Ty Bayn EDGE Project

Junior/Senior Year EDGE Project (SUU 2019): Photo and Voice Recognition software intended to be a very basic Artificial Intelligence with basic Machine Learning.

The software is a Visual Studio Project written in C#. It utilizes a microphone and camera in order to learn people's faces and names. Once a person is added it will attempt to remember that individual the next time it sees them.

This software is currently only designed for Windows systems, specifically Windows 10 with .NET.

## Prerequisites

#### Compiler

This software was built in the Microsoft Visual Studio Community 2019 IDE, and is intended to be compiled in the same IDE. It will likely work with newer versions of the same software. 

Opening "tybaynEDGEproject.sln" will open the entire solution.

#### External Libraries

Many parts of this software are linked to or dependent on other libraries and source 
code. Any source code that is not compatible with the MIT license directly has been 
excluded from this repository and must be downloaded/included separately. 

Any and all external libraries are owned and copyrighted by their respective authors and licensed under their respective licenses and ARE NOT included under the MIT license of this repository. They are used in an unaltered state and simply linked to.

##### Excluded NuGet Packages

The following libraries can simply be added using Visual Studio's NuGet Package manager:
* Accord.NET (v3.8.0) -by Accord.NET
* Accord.Vision (v3.8.0) -by Accord.NET
* NAudio (v1.10.0) -by Mark Heath

##### Excluded Third Party Libraries
The following are not available on NuGet and the .dll should be downloaded and linked within Visual Studio:
* XNAFan.ImageComparison -by Jakob Farian Krarup
  * Available From: https://www.codeproject.com/KB/graphics/374386/JustTheDll_1.6.zip
  * File Needed: XNAFan.ImageComparison.dll

##### Included Third Party Libraries
The "WebCam_Capture.dll" library is licensed under MIT and is included. It is in "tybaynEDGEproject/lib/WebCam_Capture.dll" and just needs to be linked within Visual Studio.

##### NOTICE
PLEASE READ THE "NOTICE" DOCUMENT FOR MORE INFORMATION ON THE EXTERNAL LIBRARIES REFERENCED AND THEIR LICENSING.

#### File Structure
Ensure that the following file structure is maintained as the project is downloaded:

```shell
tybaynEDGEproject.sln
tybaynEDGEproject/
|  App.config
|  AudioCompareHandler.cs
|  AudioHandler.cs
|  FileHandler.cs
|  Form1.cs
|  Form1.Designer.cs
|  Form1.resx
|  Helper.cs
|  PitchShift.cs
|  Program.cs
|  tybaynEDGEPproject.csproj
|  VideoCompareHandler.cs
|  VideoHandler.cs
|  WaveFormVisualizer.cs
|  WebCam.cs
|
└──bin/
|  └──data/
|  |  └──resources/
|  |  |  |  faceOverlay.png
|  |  |
|  |  ...
|  ...
|
└──lib/
|  |  WebCam_Capture.dll
|
...
```
## Agreement
By using this software you confirm that you have read this document, the NOTICES file pertaining to external and excluded libraries, and the LICENSE document and agree to all the terms within.

## License
[MIT](https://choosealicense.com/licenses/mit/)