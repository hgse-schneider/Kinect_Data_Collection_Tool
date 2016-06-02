#Kinect Data Collection Tool

##Description
User-friendly software for collecting data from the kinect sensor (v2). 

Opening prompt:

![alt tag](https://dl.dropboxusercontent.com/u/2554340/Github/prompt.PNG)

Main window:

![alt tag](https://dl.dropboxusercontent.com/u/2554340/Github/kinect_view.PNG)


##Features
Right now, generates a log file containing the following information:
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
* [https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx Visual studio]

##Dependencies
* .NET framework
* [https://www.microsoft.com/en-us/download/details.aspx?id44561 Kinect v2 SDK]

##Next Steps
* Give the option to save audio and video files (or just screenshots)
* 
