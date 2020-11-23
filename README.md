# EfCoreTsGen
Generates typescript classes from Entity Framework scaffolded classes

Install using dotnet CLI:

`dotnet tool install -g EfCoreTsGen`

Run via powershell or command line: `efcoretsgen [PathToConfig.json]`
If you do not insert a property tool assumes config lives in same directory that you are running it from.

Config:

modelPath: path, relative to config file, that contains EF Core generated model files

excludeStrings: array of strings that if contained in the name of the file, the typescript generator will ignore, use to ignore EF Core generated context files, or models you do not want to include in typescript

outputPath: path relative to the config file to output the generated typescript files

example:
{<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"modelPath":".\\Models",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"excludeStrings":[<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Context"<br/>
&nbsp;&nbsp;&nbsp;&nbsp;],<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"outputPath":"..\\AIMS\\src\\models"<br/>
}
