Author: Marwa AlAlawi 
Email: malalawi@mit.edu 


-------------System Requirements----------------


- Solidworks 2021 (earlier versions might work, but has not been tested)-- Note that SW only works on Windows
- Visual Studio 2015 or higher (2015 and 2019 have been tested)
- CodeStack.SWEx.PMPage & AddIn (download from Visual Studio Nuget Manager)
- .NETframework V4.6+ installed (otheriwse will not work) 
- Language Used: C#



-----File Format--------

- The Visual Studio C# add-on is called "Trace Generator" and should have the following files within it; 
	1) .vs (folder) 
	2) packages (folder) -- contains additional library add-ins used for Solidworks
	3) TraceGenerator (folder) -- houses all the C# files and assets 
	4)  TraceGenerator.sln (visual studio solution file -- this should be run in admin mode to use the plugin)



---- How to run ----------

- Open Visual studio and run as administrator 
- Open the TraceGenerator.sln file 
- Within Visual studio, you will find a hierarchy of codes and files. Navigate to "Addin.cs" and press the "start button" at top 
of the visual studio tool bar. 
- Solidworks should open up, with a tab called "MechSense" at the top toolbar. This will house 3 tool commands ("Rotational Motion Sensors"), ("Conductive Routing Path"), ("Export Models"). 
- if Solidworks does not show up: right click on trace generator (in the VS hierarchy) and select properties. in Properties, navigate to "Debug". Select "start external program" and set this external application to Solidworks based on the file path of Solidworks in your device. Solidworks file path must be ".exe" file, otherwise will not work.


-----Editor's commands------

MechSense has three main commands in its UI: 

1) Rotational Motion Sensors: Creates the MechSense patches on both stationary and rotary faces (opens up a UI window on the side)
2) Conductive Routing Path: Creates ruoting paths (cylindrical cross section) based on a 3D sketch supplied by the user (performs both sweep extrusing and cutting); 
3) Export Models: Exports all models housed under "Solid Bodies folder" in SW tree/hierarchy to ".STL" format.



----- Using the conductive routing path tool ----- 

- Need to have an object to subtract from (ie a geometry to route traces through), otherwise will throw an error.
- Sketch has to be 3D and should be exited in order to use tool (otherwise, sketch won't be selectable by the routing tool UI)


-----Exporting files ----------------

- Current export path is set to downloads. Should be changed based on your export location and file path; 


-----------Accessing File/folder for modification ----------------


	- Open the VS file "Tracegenerator.sln"
	- Please make sure you are running this as admin, or else you will get build errors (access denied)
	- If dependencies codestack.SWex is missing: Manage Nuget Libraries--> browse--> look up both .AddIn and .PMpage
	- Make sure you have .NETframework V4.6+ installed (otheriwse will not work) 
	- Make sure that in VS hierearchy, and under "References" that all three solidworks interop files have embedded interop type set to "false"
	- Ensure that build file set to "register for COM interop" in build* (right click--> properties)
	- AddIn.cs houses majority of functional code. All the other .cs files house UI related code and are accessed by AddIn.cs;

---Limitations ---- 

1) If the face of the geometry has holes (not the hole for the shaft but holes for fasteners etc) or other features that are extrude cut then this may interfere with the generator and the resulting geometry may look wrong; 


Note: This has only been tested on one laptop (MSI, Windows). In case something isn't working, it might be an assignment for some local paths that I have missed to bring up in the READ ME file. 
