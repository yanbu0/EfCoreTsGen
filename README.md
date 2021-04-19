# EfCoreTsGen
Generates typescript classes from Entity Framework scaffolded classes

Install using dotnet CLI:

`dotnet tool install -g EfCoreTsGen`

Run via powershell or command line: `efcoretsgen [PathToConfig.json]`<br/>
<b>Breaking change:</b> You must enter the path to the settings file as a parameter, if it is in the same directory simply run `efcoretsgen .`
Config file must be named `efcoretsgen.settings.json` 

Config:

modelPath: path, relative to config file, that contains EF Core generated model files

excludeStrings: array of strings that if contained in the name of the file, the typescript generator will ignore, use to ignore EF Core generated context files, or models you do not want to include in typescript

outputPath: path relative to the config file to output the generated typescript files

example:<br/>
{<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"modelPath":".\\Models",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"excludeStrings":[<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Context"<br/>
&nbsp;&nbsp;&nbsp;&nbsp;],<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"outputPath":"..\\FrontEnd\\src\\models"<br/>
}
