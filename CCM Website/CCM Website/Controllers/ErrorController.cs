using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using CCM_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Controllers;

public class ErrorController : Controller
{
    [Route("404")]
    public IActionResult PageNotFound()
    {
        return View("404");
    }

    [Route("403")]
    public IActionResult Error403()
    {
        return View("403");
    }

    [Route("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View("AccessDenied");
    }
}
