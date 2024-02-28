#Her henter vi dotnet SDK (software development tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
#Vi sætte vores arbejds directory til at være /app
WORKDIR /app

#Vi kopiere alt fra .csproj filen til vores root
COPY *.csproj ./
RUN dotnet restore

#vi kopiere alt fra filer fra vores service til vores root
COPY . ./

#Vi kører kommandoen "publish"
#CHATGPT forklaring
# The dotnet publish command in this case is used to publish a .NET application. Let's break down the command:
#     dotnet publish: This is the command used to publish a .NET application. It prepares the application for deployment by generating all the necessary files.
#     -c Release: This specifies the configuration for the build. In this case, it's set to "Release," which means the application will be built with optimizations for release rather than debugging. 
#       Release builds are typically optimized for performance.
#     -o out: This option specifies the output directory where the published files will be placed. In this example, the output directory is set to a folder named "out." 
#           The published files will include everything needed to run the application, such as binaries, dependencies, and configuration files.
#           So, when you run dotnet publish -c Release -o out, you are telling the .NET CLI to publish the application using the Release configuration and place the output files in the "out" directory. 
# This is often done before deploying the application to a server or packaging it for distribution.
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

#Den klager over noget med abseloute path, da det er et multi-stage build
#Multi-stage build er når man bygger imaged i flere stadier og f.eks. efterlader filer man ikke skal bruge
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]
