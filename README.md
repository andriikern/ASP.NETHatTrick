# Hat-Trick

Online betting shop simulator.

**Author:** Davor Penzar <br/>
**Date:** February, 2023

## Licence

**This repository is licensed under the [GNU General Public Licence v3.0](http://gnu.org/licenses/gpl-3.0.html).** Its dependencies might be licensed under different licences: before using the code for purposes other than personal, see which dependencies are used and under what licences they are issued.

## Info

**This repository implements a simple online betting shop simulator.** The shop is implemented through all three:

1. **Database** &ndash; the simulator generates and maintains its own [database](http://en.wikipedia.org/wiki/Database). There is no need to connect it to an existing one or creating it manually.
2. **Web API** &ndash; apart from the [database](http://en.wikipedia.org/wiki/Database) and the front-end application, there is an [API](http://en.wikipedia.org/wiki/Web_API) layer in the middle that connects the two, simultaneously enforcing the betting shop's rules, ensuring proper data is put into the [database](http://en.wikipedia.org/wiki/Database), and delivering the data in the correct scale (e. g. serving only the relevant events) and format ([*JSON*](http://en.wikipedia.org/wiki/JSON)) to the front-end.
3. **Front-end** &ndash; the interactive web application with user-friendly interface ([graphical user interface (GUI)](http://en.wikipedia.org/wiki/Graphical_user_interface)) that allows the user to explore the offer, place bets, deposit and withdraw money to/from their account, and check their past tickets.

The actual layered abstraction/implementation of the app is the following:

| Main Layer | Project                                                   | Layer                                                                              | Implementation                                                                                                                                                                                                           |
|------------|-----------------------------------------------------------|------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Database   | &mdash;                                                   | [Database](http://en.wikipedia.org/wiki/Database)                                  | [SQLite](http://sqlite.org/) (can be easily changed&mdash;see below)                                                                                                                                                     |
| Database   | [HatTrick.Models](HatTrick.Models/HatTrick.Models.csproj) | [Database model](http://en.wikipedia.org/wiki/Database_model)                      | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/)                                                                                                                  |
| Back-end   | [HatTrick.DAL](HatTrick.DAL/HatTrick.DAL.csproj)          | [DAL](http://en.wikipedia.org/wiki/Data_access_layer)                              | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/), [Entity Framework](http://learn.microsoft.com/en-gb/ef/)                                                        |
| Back-end   | [HatTrick.BLL](HatTrick.BLL/HatTrick.BLL.csproj)          | [BLL](http://en.wikipedia.org/wiki/Business_logic#Business_logic_and_tiers/layers) | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [.NET](http://learn.microsoft.com/en-gb/dotnet/)                                                                                                                  |
| Back-end   | [HatTrick.API](HatTrick.API/HatTrick.API.csproj)          | [Web API](http://en.wikipedia.org/wiki/Web_API)                                    | [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/), [ASP.NET](http://learn.microsoft.com/en-gb/aspnet/core/), [Swashbuckle](http://learn.microsoft.com/en-gb/aspnet/core/tutorials/getting-started-with-swashbuckle/) |
| Front-end  | [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj)          | [GUI](http://en.wikipedia.org/wiki/Graphical_user_interface)                       | [Vue.js](http://vuejs.org/), [TypeScript](http://typescriptlang.org/), [JavaScript](http://en.wikipedia.org/wiki/JavaScript), [Bootstrap](http://getbootstrap.com/docs/5.0/)                                             |

Since [Entity Framework](http://learn.microsoft.com/en-gb/ef/) is used for accessing the database, without any custom [SQL](http://en.wikipedia.org/wiki/SQL) code but using code-first approach, with just a few changes in the [`AppSettings`](HatTrick.API/appsettings.json) and [`Startup`](HatTrick.API/src/Startup.cs) one can migrate the app from [SQLite](http://sqlite.org/) to many others [DBMSes](http://en.wikipedia.org/wiki/Database#Database_management_system): see [here](http://learn.microsoft.com/en-gb/ef/core/providers/) and [here](http://learn.microsoft.com/en-gb/ef/ef6/modeling/code-first/workflows/new-database/). The resulting [database model](http://en.wikipedia.org/wiki/Database_model) is visualised as a directed graph in the [*DatabaseModel.dgml* file](DatabaseModel.dgml), which was auto-generated from the [`Context` class](HatTrick.DAL/src/Context.cs) using [EF Core Power Tools](http://learn.microsoft.com/en-gb/ef/core/extensions/#ef-core-power-tools).

## Running the App

First, [clone the repo](http://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository/). Once the code is available locally, **the app must be run in development mode**&mdash;this refers to the [HatTrick.API](HatTrick.API/HatTrick.API.csproj) and [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) projects. It cannot run in production mode because the [connection string](http://en.wikipedia.org/wiki/Connection_string) is currently set only in the [development `AppSettings`](HatTrick.API/appsettings.Development.json), and the [Vue.js](http://vuejs.org/) [proxy server](http://en.wikipedia.org/wiki/Proxy_server) is set only for the [`devServer.proxy`](http://cli.vuejs.org/config/#devserver-proxy) in [`vue.config`](HatTrick.Web/vue.config.js). See below how to achieve running in development mode.

### Through [Visual Studio](http://visualstudio.microsoft.com/)

The application is implemented as a [Visual Studio Solution](http://learn.microsoft.com/en-gb/visualstudio/ide/solutions-and-projects-in-visual-studio/#solutions). The simplest way to run the app is to open the [HatTrick.sln solution](HatTrick.sln) in [Visual Studio](http://visualstudio.microsoft.com/), and run the [HatTrick.API](HatTrick.API/HatTrick.API.csproj) and [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) projects in parallel: see [here](http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue/#set-the-startup-project) and [here](http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue/#start-the-project). By default, the app will run in development mode, but this can be changed, as explained [here](http://learn.microsoft.com/en-gb/aspnet/core/fundamentals/environments/#development-and-launchsettingsjson) and [here](http://cli.vuejs.org/guide/mode-and-env.html#modes-and-environment-variables).

### Manually

Again, the [HatTrick.API](HatTrick.API/HatTrick.API.csproj) and [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) projects must be run in parallel. To run the former in development mode, see [here](http://learn.microsoft.com/en-gb/aspnet/core/fundamentals/environments/#development-and-launchsettingsjson) and [here](http://learn.microsoft.com/en-gb/aspnet/core/fundamentals/environments/#set-environment-on-the-command-line); note that the `Development` environment is already defined in the [`launchSettings`](HatTrick.API/Properties/launchSettings.json). To run the latter in development mode, see [here](http://cli.vuejs.org/guide/mode-and-env.html); more about running the [HatTrick.Web](HatTrick.Web/HatTrick.Web.esproj) is available in its own [`README`](HatTrick.Web/README.md).

## Remarks

Obviously, this is a simulator. What ever the user *does* with the *money* using the app, no real-world transactions take place in the background. Also, although the original (non-promoted) offer, which is currently available through sample data, was copied from one Croatian betting provider at one specific point in time, the coefficients do not necessarily reflect competitors' potential to accomplish the favourable outcomes.

However, there are some more important implications, regarding the architecture of the solution at hand:

1. All [web API](http://en.wikipedia.org/wiki/Web_API) endpoints allow passing a time point, either for searching the database (e. g. not to display past events) or for specifying when a certain user-initiated action was done. Also, the front-end maintains its own hard-coded fixed time-point at which all actions are done, and it passes it in all communication with the back-end. In reality, the server would take care of recording time, and it would not expose functionality for manipulating the time. This was done mainly to make sure the offer never expires on the front-end.
    * **IMPORTANT.** When testing the [web API](http://en.wikipedia.org/wiki/Web_API), make sure you pass a time-point no earlier than the 1<sup>st</sup> of January, 2023, and no later than the 8<sup>th</sup> of February, 2023. If no time-point is passed, or a too early or a too late one is passed, the offer might seem empty, even if there are clearly some data in the database.
2. No authentication is implemented in the app. The database allows having multiple users, each ticket and transaction must be related to a registered active user, and the front-end keeps a hard-coded user id which it then uses for communicating with the back-end. On the other hand, the back-end accepts the id as any other parameter, meaning that any one could send requests to the back-end on behalf of any one else, simply by passing the other user's id. If the requests were otherwise valid, the back-end would accept them, which is a major vulnerability, especially when dealing with financial transactions.
3. The code is far from ideal. First of all, this was my first serious experience with [Vue.js](http://vuejs.org/), and therefore I am probably not even aware of all downsides of my solution there. Also, regarding the front-end, the web design could be much better, but I mostly focused on the business logic in the app, and just made a simple [Bootstrap-powered](http://getbootstrap.com/docs/5.0/) web page for presenting it as a usable app. Second of all, some [C#](http://learn.microsoft.com/en-gb/dotnet/csharp/) classes in the back-end could probably be split into smaller classes, using a higher level of abstraction, but this is something one would do in a professional environment, for a production quality code, having serious resources available (e. g. time, money, people&hellip;).
4. The [database](http://en.wikipedia.org/wiki/Database) is very simplified and exposes only the entities and properties needed for the app. In real life, there is some additional information that would be associated with the entities used by the app:
    * value segmentation of users &ndash; as the segmentations are needed only for internal business processes, it is irrelevant for a simulator like this one,
    * stake factor grading of users &ndash; similar as the previous, only it could be used to further limit the maximal amount to bet, which adds no value to the simulator,
    * entities for tournaments, competitors, and maybe even locations &ndash; sure, the tournaments table could be utilised for this app, to present the events in the offer groupped by tournaments, but, for it to make sense, more events should be present in the sample data; for other tables I do not see a signifficant value they would add to the app if present,
    * keeping track of rescheduled events and having the rescheduled events reference the original events &ndash; again, I only see the value added by this in internal business processes, such as when analysing how many different events were offered during a certain period (month, year, &c.).
5. Although the [database model](http://en.wikipedia.org/wiki/Database_model) is simplified, the sample data contains some unused entries which fit into the model, but utilising them would make the simulator much more complex than it is (e. g. live fixtures or *cashed out* ticket status).

## To Do

1. Implement deposit/withdrawal mechanism.
2. Implement displaying of ticket's selections.
3. Minor refinement of the code (e. g. extracting some parts of larger [Vue.js](http://vuejs.org/) components into dedicated smaller components).
4. Add promoted fixtures/markets/outcomes to the offer (the mechanism is already ready, but the data is not present in the database).
5. Describe the solution here in [`README`](README.md).
