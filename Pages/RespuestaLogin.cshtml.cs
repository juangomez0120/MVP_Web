using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MVP_Web.Model;
using System.Web;
using System.Net.Http;

public class RespuestaLoginModel : PageModel
{
    [BindProperty]
    public string ResponseBody { get; set; }

    public void OnGet(string result)
    {
        ResponseBody = result;
    }
}