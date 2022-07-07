# XodoCloudServer🐱‍🐉
Server for uploading and storing files
## What is it?
This site is suitable for those who do not want to tremble at the thought that their precious files will be lost  
They are stored on the server and the user can download them back at any time. 
## Launch Guide :elephant:
#### Clone repository
```
$ git clone https://github.com/XodoSan/XodoCloudServer.git
```

#### Check if the following dependencies are installed
#### 1) MS SQL Express. Replace the connection string to MS SQL in `HodoCloudAPI/appsettings.json` with your own
#### 2) Visual Studio Community Edition
#### 3) Setup XodoCloudAPI/nlog.config for logs:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
autoReload="true"
internalLogLevel="Info"
internalLogFile="c:\temp\internal-nlog-AspNetCore.txt">

<!— enable asp.net core layout renderers —>
<extensions>
<add assembly="NLog.Web.AspNetCore"/>
</extensions>

<!— the targets to write to —>
<targets>
<!— File Target for all log messages with basic details —>
<target xsi:type="File" name="allfile" fileName="c:\temp\nlog-AspNetCore-all-${shortdate}.log"
layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}" />

<!— File Target for own log messages with extra web details using some ASP.NET core renderers —>
<target xsi:type="File" name="ownFile-web" fileName="c:\temp\nlog-AspNetCore-own-${shortdate}.log"
layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}|url: ${aspnet-request-url}|action: ${aspnet-mvc-action}|${callsite}" />

<!--Console Target for hosting lifetime messages to improve Docker / Visual Studio startup detection —>
<target xsi:type="Console" name="lifetimeConsole" layout="${MicrosoftConsoleLayout}" />
</targets>

<!— rules to map from logger name to target —>
<rules>
<!--All logs, including from Microsoft-->
<logger name="*" minlevel="Trace" writeTo="allfile" />

<!--Output hosting lifetime messages to console target for faster startup detection —>
<logger name="Microsoft.Hosting.Lifetime" minlevel="Info" writeTo="lifetimeConsole" final="true" />

<!--Skip non-critical Microsoft logs and so log only own logs (BlackHole) —>
<logger name="Microsoft.*" maxlevel="Info" final="true" />
<logger name="System.Net.Http.*" maxlevel="Info" final="true" />

<logger name="*" minlevel="Trace" writeTo="ownFile-web" />
</rules>
</nlog>
```

## Press the green button "run" and enjoy :bee:
