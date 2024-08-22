# Estuary-Unity-SDK

## Description
The Estuary Unity SDK (dubbed **Mangrove SDK**) is a Unity plugin which allows for easy connection 
to a running instance of [Mangrove](https://github.com/Al-Estuary/mangrove),
the backend server component of Estuary.  This plugin includes a simple 
example scene and a boilerplate prefab to help speed up development with 
Estuary.  Our SDK leverages the 
[SocketIO](https://github.com/itisnajim/SocketIOUnity)
Unity plugin, so connections can be made conveniently by specifying 
the local IP address and port number of your Mangrove instance.

## Give us a Star! ⭐
If you find Estuary helpful, please give us a star!  Your support means a lot! 
If you find any bugs or would like to request a new feature, feel free to open an
issue!
## Installation
<ol>
<li> Install SocketIO into your Unity project
<ol>
<li> Copy the SocketIO plugin URL: 

```https://github.com/itisnajim/SocketIOUnity.git``` 
</li>
<li> In your Unity project, open Window -> Package Manager </li>
<li> Click on the "&#43;" button at the top left and select "Add package from git URL" </li>
<li> Paste the link there and click "Add" or press enter</li>
</li>
<li> ***Optional***  If the above step does not work, the SocketIO Unity plugin 
may have changed.  In this case, consult the 
<a href="https://github.com/itisnajim/SocketIOUnity?tab=readme-ov-file#installation">
SocketIO plugin installation guide</a>.
</li>
</ol>
<li> Install the Estuary Unity SDK by repeating the previous step but using this URL instead:

```https://github.com/Al-Estuary/Estuary-Unity-SDK.git```
</li>