# XServer.Core
A slippy map for .NET Core, based on PTV xServer.NET

![screenshot](https://raw.githubusercontent.com/XServer.Core/d42e7b0e5bc92e86a2db6890ec796ba12fc7a87b/Screenshot.jpg)

This is a stripped-down version of https://github.com/ptv-logistics/xserver.net which runs on .NET Core WPF. 

What's missing:

* Projections-Library - only supports WGS84 (Longitude, Latitude) and (Web-)Mercator.
* WMTS layer - although WMSs supporting WebMercator (EPSG:3857) can be used.
* FormsMap - but ElemenHost can be used to embed into a WinForms application.
* "native" xServer support - xServer1/2 can be added via the WMS / tile api though.
