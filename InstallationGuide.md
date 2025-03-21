# SH04 - Course Content Mapping System
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE) 
[![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?logo=dotnet
)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-8913fc?logo=bootstrap&logoColor=fff)](#)

## Installation Guide
### Installation
INSTALLATION INSTRUCTIONS HERE

### Setup
The database for the WebApp should be populated with required setup data on the first build of the App. This data mainly populates the many drop-down lists (which can be later edited via the Admin Page).

This populations script is located in the following file `CCM Website/CCM Website/Data/AppDbInitialiser.cs`

User account permissions and roles and not handled via the WebApp but via EntraID (formally AzureAD). INSTRUCTIONS FOR SETTING USERS TO ADMIN HERE