# AssetViewer

## What is this?
This is a Unity project made for downloading assets from a remote uri and importing the models and textures, which is done
during runtime.

## Technical
A native C++ library (OBJMANDLL by Gerhard Reitmayr, 2012) is used to parse .OBJ files. Custom wrapper functions
had to be written in order to get the generated .dll working with Unity. C++ functions can be seen in C++/ObjMan.cpp. According C#
functions in Assets/_AssetView/Scripts/AssetImporter.cs.

Currently, the models are downloaded from a NodeJS server set up on Heroku.

## Platform
This currently works only on PC. 
