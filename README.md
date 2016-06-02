#Kinect Data Collection Tool

##Description
User-friendly software for collecting data from the kinect sensor (v2). 

Opening prompt:

![alt tag](https://dl.dropboxusercontent.com/u/2554340/Github/prompt.PNG)

Main window:

![alt tag](https://dl.dropboxusercontent.com/u/2554340/Github/kinect_view.PNG)


##Features
Generates a log file with the following information:
* Skeleton data
	* x,y,z positions of each joint 
	* records whether the joint position is inferred or not
	* records each hand state (open,closed,pointing) with a confidence score
* Face data
	* records the pitch, yaw and roll of the face
	* records if each eye and the mouth are open/closed
	* records if the user is happy, engaged, wearing glasses, looking away

##Requirements
* Windows 8 or 10
* 64-bit (x64) processor (not tested on x86)
* USB3 port (doesn't work with USB2)
* [Visual studio](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx)

##Dependencies
* .NET framework 4.5
* [Kinect v2 SDK](https://www.microsoft.com/en-us/download/details.aspx?id44561)

##Next Steps
* add sound events to the log file (e.g., if a person is talking)
* Give the option to save audio and video files (or just screenshots)

