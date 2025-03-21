# SH04 - Course Content Mapping System
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE) 
[![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?logo=dotnet
)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-8913fc?logo=bootstrap&logoColor=fff)](#)

## Installation Guide
### Installation
Clone the repository and use branch main or the latest release. 

Or download the latest release.

---
### Setup
#### Azure Setup
Set up an App Service to host the application using Windows.

Set up an SQL Server to host your database.

Set up an SQL Database. Your connection string can be found here and will need to be added to the project later.

#### Application Data
Modify files to match your deployment

In `CCM Website/CCM Website/appsettings.json`
modify the database connection string `ConnectionStrings:DefaultConnection` to connect to your database.

The database for the WebApp should be populated with required setup data on the first build of the App. This data mainly populates the many drop-down lists (which can be later edited via the Admin Page).

This populations script is located in the following file `CCM Website/CCM Website/Data/AppDbInitialiser.cs`

#### Login/SSO Setup
Login to the site is handled via Microsoft **EntraID** (formally AzureAD), giving SSO login.

Create a Microsoft EntraID App Registration supporting the types of accounts you want to support. 

The link information between your EntraID setup and the webapplication can be edited in `CCM Website/CCM Website/appsettings.json` under the `AzureAD` dictionary. Both the `TenantId` and `ClientId` should be edited to change the SSO setup.

The sites Admin permissions is handled via Microsoft EntraID. To set this up:

In your projects Microsoft EntraID App Roles create a role called “Admin”. This role will be used to determine who can enter the admin pages on our site.

In your Enterprise Application in Users and Groups you can assign the accounts that you want to be admins to the “Admin” role in our site.

