# SH04 - Course Content Mapping System
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE) 
[![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?logo=dotnet
)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-8913fc?logo=bootstrap&logoColor=fff)](#)

## Project Overview
CCM Website is a WebApp for the [University of Glasgow Learning Innovation Support Unit's](https://www.gla.ac.uk/myglasgow/learningandteaching/lisu/) Course Content Mapping system. This application has taken and altered the existing spreadsheet version, to work as an WebApp. This will allow for more centralised management of the individual workbooks created.

## Project Solution
### Proposed Solution
Create an ASP.NET web application, allowing for users to sign in via UofG SSO and access and edit their created workbooks. 
Each workbook would have its information stored in a DB, with each workbook having a Workbook Attributes. Workbooks would then have linked weeks, which in turn have linked learning activities. 
### Requirements
A Web Application that through SSO login, allows University staff to view and create Course Content Mapping Workbooks. 

Workbooks should be created with different lengths (number of weeks) and linked to a specific learning platform. This learning platform link restricting the types of activities that can be assigned in the courses different weeks.

For each week in the workbook, the course lead (assigned on creation) should be able to add any number of weekly activities, selected from the available activities for the workbooks linked learning platform. The course lead should also be able to assign a number of graduate attributes to each week.

See the [requirements wiki page](https://github.com/HamishTheGinger/LISU-Course-Content-Mapping-System/wiki/Project-Requirements) for more in-depth requirements.

### Solution Design
Front-End Design: [Figma Prototype ](https://www.figma.com/proto/Hf2XEAaav9YKt5Q6M19D0j/Basic-Wireframe?page-id=0%3A1&node-id=21-3&node-type=canvas&viewport=-13%2C427%2C0.04&t=0nrmnLeMZ7QZ4RJZ-1&scaling=scale-down&content-scaling=fixed&starting-point-node-id=21%3A3&show-proto-sidebar=1)

Database Design: [Database Entity Relationship Diagram](https://lucid.app/lucidchart/278d5713-5828-45ea-9cbf-9a1a21c81fa9/edit?viewport_loc=-665%2C-597%2C3071%2C1572%2C.VNTkfv.2FQ1&invitationId=inv_f088981d-a442-4b4c-aaac-7738f8466f3a)

## Software Stack
This project has been created using [ASP.NET V8](https://dotnet.microsoft.com/en-us/). The project is running a [Razor Frontend](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-9.0), using [Bootstrap V5](https://getbootstrap.com/) styling.

### Packages
Packages are installed and managed via [NuGet](https://www.nuget.org/), the default .NET packet manager. The packges are contained with the respective projects `.csproj` file.
#### CCM Website
Microsoft.AspNetCore.Authentication.OpenIdConnect 8.0.6 \
Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.10 \
Microsoft.EntityFrameworkCore.Design 9.0.1 \
Microsoft.EntityFrameworkCore.Sqlite 9.0.1 \
Microsoft.EntityFrameworkCore.SqlServer 9.0.1 \
Microsoft.EntityFrameworkCore.Tools 9.0.1 \
Microsoft.Identity.Web 2.19.1 \
Microsoft.Identity.Web.UI 2.19.1 \
Microsoft.VisualStudio.Web.CodeGeneration.Design 8.0.6 \
X.PagedList 10.5.7 \
X.PagedList.Mvc.Core 10.5.7

#### CCM Website Tests
coverlet.collector 6.0.0 \
Microsoft.AspNetCore.Mvc 2.3.0 \
Microsoft.EntityFrameworkCore.InMemory 9.0.1 \
Microsoft.NET.Test.Sdk 17.8.0 \
Moq 4.20.72 \
Moq.EntityFrameworkCore 9.0.0.1 \
xunit 2.5.3 \
xunit.runner.visualstudio 2.5.3

## User Guide
Please see the [installation guide](InstallationGuide.md) for installation instructions.

### User Side
On login (via SSO) to the webApp users are greeted with the home page which displays a preview of workbooks the user owns as well as the top (last edited) UofG workbooks.

From this view (using the navbar) the user can either navigate to their own workbooks (to view/edit/create) or to the search page to view a larger selection of UofG workbooks.

#### Viewing Workbooks
Workbooks can be viewed by any user by either selecting the workbook from the home page or selecting the view workbooks button from the search page.

There are 2 main ways to view the workbook, via the Overview page, or via the weekly pages. 
- The overview contains summarised information taken from the workbooks individual weeks.
- The week page contains a detailed list of the activities and graduate attributes assigned to that week, as well as a number of statistics for that week.

A user can also share the workbook, allowing any other UofG staff to directly access it via the workbooks unique link.

#### Creating and Editing Workbooks
All users can create their own workbooks. This is done via the MyWorkbooks Page `IndexDomain/Workbooks`. When creating a workbook the user will be asked for the following information
- Course Name
- Course Code [Optional]
- Pip Reference [Optional]
- Course Lead
- Course Length [Cannot be changed later]
- Learning Platform [Cannot be changed later]
- Workbook Area
- Collaborators 

After completing the creation form, users will be taken to their new workbooks details page. There is the option to edit the workbooks details (information above).

To add content to the workbook, the user should select the appropriate week button. They will then have the opportunity to create activities and assign graduate attributes. Once data is added to the workbook both the week and overview tables and charts will automatically populate and update.

When creating an activity the following information is needed
- Week Number 
- Activity Type [Learning Platform Restricted]
- Task Title
- Task Staff
- Task Time [Entered as HH:MM]
- Task Status
- Learning Type
- Task Location
- Task Approach

From the week page a user would be able to edit the activity, as well as changing the order of activities. 

User are **not** able to delete workbooks, and would be required to contact LISU/Admins if they wished for a workbook to be deleted.


#### Settings
There are a number of accessibility settings available at `IndexDomain/Home/Settings`. These allow for the Font Style and size to be edited as well as allowing the user to select either Light or Dark mode.

### Admin Side
The admin side of the site (`CCM Website/CCM Website/Areas/Admin/`) can be accessed from `IndexDomain/Admin`. If a user does not have access, an access denied page will be displayed (determined via SSO role on login) 

This admin are of the site allows for admin users to manipulate a large amount of the data on the site.
- Manage Workbooks
    - Create new Workbooks
    - View All Workbooks    
    - Edit and Delete **all** workbooks on the site.
- Manage Fields
    - Create, Edit and Delete dropdown options across the site
    - Activities, Graduate Attributes, Learning Platforms, Learning Types, Task Locations, Task Progress Status, Task Approach, Workbook Areas
- Links and Dependency Tables
    - Allows access to a number of relationship/inner-working tables.
    - Allows for access in the case of an error
    - Week<->Workbook , Week<->Activity, Week/Workbook<->Graduate Attribute






