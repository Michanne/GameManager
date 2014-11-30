
Welcome !


The Nokia Imaging SDK makes some of the technologies that Nokia uses in its own imaging applications available to developers. It is an efficient library for manipulating image data captured and stored by mobile devices. The features include decoding and encoding JPEG images, applying filters and effects, blending, cropping, rotating, and resizing, all with customization and unlimited undo.


If you are updating from beta version, we have introduced an API break with the 1.0 version: make sure you read the section "Graduated from Beta - new in SDK 1.0" below!



New Users
=========

If this is your first time with the Nokia Imaging SDK, welcome, we are glad to have you with us! Two little hints to get you started off on the right foot:
  - Take a quick peek at our documentation : http://developer.nokia.com/Resources/Library/Lumia/#!nokia-imaging-sdk.html
  - Some version of Visual studio may not find the references to the Nokia Imaging SDK that were added to your project by NuGet.  To fix things, simply close your project and reopen it. 
  


Graduated from beta – new in SDK 1.0
====================================

We ran a successful beta phase, with the Future Capture competition resulting in some very cool stuff done with the Imaging SDK, and we received valuable feedback. Now, as a result of that feedback, we implemented the most requested features – changing filter parameters dynamically, implementing your own filters, as well as amending features such as support for a Chroma Key filter.

To support the new features and to fully align with WinRT conventions, we had to change the APIs, which means that apps written for the beta will need to be migrated over if one wants to use the new release.  To help you with migration, we have put together a how to upgrade guide presenting the key differences between the Beta and 1.0.

http://developer.nokia.com/Resources/Library/Lumia/#!nokia-imaging-sdk.html


Previous version of the SDK
===========================

It is possible to revert to the previous version of the SDK. The Beta version of the Nokia Imaging SDK is available via Visual Studio NuGet package manager or as a download. The offline documentation and API reference for the Beta SDK are available as offline packages.

http://developer.nokia.com/Resources/Library/Lumia/#!nokia-imaging-sdk.html


If you need to revert to the previous version of the SDK, run the following commands in Visual Studio Package Manager Console:
PM> Uninstall-Package NokiaImagingSDK
PM> Install-Package NokiaImagingSDK -Version 0.9.3




Copyright (c) 2012-2013, Nokia
All rights reserved.





