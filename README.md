# SimpleForum
SimpleForum is a lightweight, free open source forum application written in ASP.NET Core.

## Building
To use the project the following dependencies are required:
- .NET Core 3.1 SDK
- Powershell
- Bash
- Screen

To build the project, run build.ps1. The program is built to the build directory.<br>
Set the following environment variables, either on the system or the run.sh script
- DbConnectionString, in the standard MariaDB connection string format
- MailConnectionString, in the format: address;port;name;sender address;username;password

To run the program, run run.sh. Each service can be accessed by screen. The session names can be changed in the script. 

## Acknowledgements
This program is built using the following open source libraries and software
- [ASP.NET Core](https://github.com/dotnet/aspnetcore)
- [EF Core](https://github.com/dotnet/efcore)
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- [NetCore.MailKit](https://github.com/myloveCc/NETCore.MailKit)

## Notes
This project was written for an extended college project, and thus some functions which are typically achieved with the use of libraries are implemented manually.

The project will also not be receiving any further updates, as it reached it's completion for the project.