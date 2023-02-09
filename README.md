# HatTrick

Online betting shop simulator.

**Author:** Davor Penzar <br/>
**Date:** February, 2023

## Info

This repository implements a simple online betting shop simulator. The shop is implemented through all three:

1. **Database** &ndash; the simulator generates and maintains its own database. There is no need to connect it to an existing one or creating it manually.
2. **Web API** &ndash; apart from the database and the front-end application, there is an API layer in the middle that connects the two, simultaneously enforcing the betting shop's rules, ensuring proper data is put into the database, and delivering the data in the correct scale (e. g. serving only the relevant events) and format ([*JSON*](http://en.wikipedia.org/wiki/JSON)) to the front-end.
3. **Front-end** &ndash; the interactive web application with user-friendly interface (graphical user interface (GUI)) that allows the user to explore the offer, place bets, deposit and withdraw money to/from their account, and check their past tickets.

The layers are implemented as the following:

| Main Layer | Project                                                   | Layer                                                                              | Implementation                                                                                                                                                    |
|------------|-----------------------------------------------------------|------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Database   | &mdash;                                                   | [Database](http://en.wikipedia.org/wiki/Database)                                  | [SQLite](http://sqlite.org/) (can be easily changed&mdash;see below)                                                                                              |
| Database   | [HatTrick.Models](HatTrick.Models/HatTrick.Models.csproj) | [Database model](http://en.wikipedia.org/wiki/Database_model)                      | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/)                                                           |
| Back-end   | [HatTrick.DAL](HatTrick.DAL/HatTrick.DAL.csproj)          | [DAL](http://en.wikipedia.org/wiki/Data_access_layer)                              | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/), [Entity Framework](http://learn.microsoft.com/en-gb/ef/) |
| Back-end   | [HatTrick.BLL](HatTrick.BLL/HatTrick.BLL.csproj)          | [BLL](http://en.wikipedia.org/wiki/Business_logic#Business_logic_and_tiers/layers) | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/)                                                           |
| Back-end   | [HatTrick.API](HatTrick.API/HatTrick.API.csproj)          | [Web API](http://en.wikipedia.org/wiki/Web_API)                                    | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [ASP.NET](http://learn.microsoft.com/en-gb/aspnet/core/)                                                   |
| Front-end  | [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj)          | [GUI](http://en.wikipedia.org/wiki/Graphical_user_interface)                       | [Vue.js](http://vuejs.org/), [TypeScript](http://typescriptlang.org/), [Bootstrap](http://getbootstrap.com/docs/5.0/)                                             |

Since [Entity Framework](http://learn.microsoft.com/en-gb/ef/) is used for accessing the database, without any custom [SQL](http://en.wikipedia.org/wiki/SQL) code but using code-first approach, with just a few changes in [`AppSettings`](HatTrick.API/appsettings.json) and [`Startup`](HatTrick.API/src/Startup.cs) one can migrate the app from [SQLite](http://sqlite.org/) to many others [DBMSes](http://en.wikipedia.org/wiki/Database#Database_management_system): see [here](http://learn.microsoft.com/en-gb/ef/core/providers/) and [here](http://learn.microsoft.com/en-gb/ef/ef6/modeling/code-first/workflows/new-database/).

## Running the App

The app **must be run in development mode**&mdash;this refers to the [HatTrick.API](HatTrick.API/HatTrick.API.csproj) and [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) projects. It cannot run in production mode without setting a proper [connection string](http://en.wikipedia.org/wiki/Connection_string) in the [production `AppSettings`](HatTrick.API/appsettings.json), because the [connection string](http://en.wikipedia.org/wiki/Connection_string) is currently set only in the [development `AppSettings`](HatTrick.API/appsettings.Development.json) and the Vue.js [proxy server](http://en.wikipedia.org/wiki/Proxy_server) is set only for [`devServer.proxy`](http://cli.vuejs.org/config/#devserver-proxy) in [`vue.config`](HatTrick.Web/vue.config.js). See below how to achieve running in development mode.

### Through [Visual Studio](http://visualstudio.microsoft.com/)

The application is implemented as a [Visual Studio Solution](http://learn.microsoft.com/en-gb/visualstudio/ide/solutions-and-projects-in-visual-studio/#solutions). The simplest way to run the app is to [clone the repo](http://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository/), open [HatTrick.sln](HatTrick.sln) in [Visual Studio](http://visualstudio.microsoft.com/), and run the [HatTrick.API](HatTrick.API/HatTrick.API.csproj) and [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) projects in parallel: see [here](http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue/#set-the-startup-project) and [here](http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue/#start-the-project). By default, the app will run in development mode, but this can be changed, as explained [here](http://learn.microsoft.com/en-gb/aspnet/core/fundamentals/environments/#development-and-launchsettingsjson) and [here](http://cli.vuejs.org/guide/mode-and-env.html#modes-and-environment-variables)

### Manually

TO DO: explain
