newrelic-unity
================================

### Prototype for a New Relic Unity Plugin 


Prerequisites
--------------------

### Unity 
* [Mac or Windows](http://unity3d.com/unity/download)
* Uses UnityEngine.dll

### ulink
* [ulink Evaluation](http://www.muchdifferent.com/?page=game-download) 

### Mono
* Uses System.Net (to make api calls)


Instructions 
--------------------

1 Clone the repo into your the Assets directory of your project

    git clone https://github.com/nickfloyd/nuget-test.git; remove-item nuget-test/.git -Recurse -Force

2 Open your project and add drag the NewRelicULinkServerInstrumentation into your main camera / scene properties window

3 Enter your license key

4 You may need to change the Platform GUID in the PlatformReporter.cs file
	private string guid = "YOURGUIDHERE"

Notes 
--------------------

* This is stictly a prototype and most likely will not work "out of box"
* This assumes that you have a trial / full version of ulink installed 
* This project was built using [the Bootcamp demo](https://www.assetstore.unity3d.com/#/content/1376)
