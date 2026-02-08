# Planning
We are going to create a web application.
There are few mandatory parts of it.
I'm going to describe each and every important setup detail.
As soon as I'll finish with my description I want you to act as a senior architect and ask me questions going deep into the details of our future architecture.
When we identify all needed requirements and technical nuances we will build an implementation plan.

## Application description
We build a simple web application called "HelpMeManageHealth" or shorter "HMMH", which helps to track user's weight as well as daily calories intake.
So that the user could see the basic statistics for the consumed food as well as long term impact on the weight.
We might consider application extension with different health and fitness features in the future hence our application need to be written using best practices and be as modular and extendable as possible.

## Application high level architecture
The whole project consists of the following parts:
* Mono repository with all the code and tools.
* Web UI to allow the end user to interact with our application.
* API to perform input data processing and provide some data to the UI and end user.
* Database to store all the application data.

### Mono repository setup
Anything we build we put in the monorepo. I want to use [Nx](https://nx.dev/) as the core monorepo technology. There must be Node.js no-code project created in the root of the repo in order to execute repository-level commands and general monorepo management and maintenance. Use nx best practices to organize the repository structure. Make sure that npm is configured to not duplicated packages and act according to our monorepo setup.
There are two main types of project planned either .Net Core API or React UI application hence there must be linters and formatting tools installed and configured for each specific type of monorepo project.

### Web UI setup
The core technology for UI application is React powered by typescript. I want to use [Mantine](https://mantine.dev/) as my component library. If there are no ready to use components available there I prefer to write one using existing components as building block. If there is a need to write CSS I want it to be a separate .css file placed near the related component. At the first iterations I don't want to write unit tests or any other type of tests, those will be added in the future iterations.

### API setup
The API will be organized as the latest .Net Core project. We need to follow best practices to build REST API.
There will be no tests in the first iteration. This API will be connected to the Postgresql database and will use latest EntityFramework as ORM layer.

### Database setup
We will use Postgresql as the database engine. I want to be able to spin up separate Postgresql docker container from the API project to run the database.

## Project initialization phases skeleton
Here is a raw description on the phases of implementation plan:
* Extract from the description and write custom instruction file or files. Perform commit.
* Initialize the nx monorepo with basic configuration. Perform commit.
* Add some default UI project as per setup description. There must be no real business logic and main goal is to get some demo page. At this stage we want to set up specific linters and formatters and update base monorepo commands. Perform commit.
* Add some default API as per setup description without any business logic. We need to get the basic demo endpoint and there must be base docker file. Perform commit.
* Add Postgres container to the API project as part of separate database sub project. Perform commit.

As soon as those basic steps are completed we will be ready to plan the business logic phase.