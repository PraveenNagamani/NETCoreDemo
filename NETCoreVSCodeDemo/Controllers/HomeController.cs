using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using MongoDB.Driver.Core.Configuration;
using NETCoreVSCodeDemo.Models;

namespace NETCoreVSCodeDemo.Controllers;

public class HomeController : Controller
{
    String ErrMsg = "";
    DBConnect dbconnect = new DBConnect();
    private readonly string Mongoconnstring = "";
    private readonly string Orclconnstring = "";
    private readonly ILogger<HomeController> _logger;
    private readonly DBConnect dbconn;

    public HomeController(ILogger<HomeController> logger,IConfiguration connstring,DBConnect dbconn)
    {
        _logger = logger;
        this.dbconn = dbconn;
        Mongoconnstring = connstring["ConnectionStrings:MongoConnection"];
        Orclconnstring = connstring["ConnectionStrings:OrclConnection"];
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        // setting log level at appsettings 
        _logger.LogInformation("inside privacy");
        string[,] Filterparams = new string[1,3];
        Filterparams[0,0] = "random_no" ; Filterparams[0,1] = "Eq" ; Filterparams[0,2] = "1313068";

        ViewBag.jsonstring =  dbconnect.GetNoSQLBson(Mongoconnstring,"random_no_data",ref ErrMsg,Filterparams);
        _logger.LogWarning($" bson data received {ViewBag.jsonstring} ");
        _logger.LogError($" bson data received showing error {ViewBag.jsonstring} ");
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpGet]
    public IActionResult ProfileView()
    {
         return View();
    }
    [HttpPost]
    public IActionResult ProfileView(string buttonid,string username,string emailid)
    {
        if(buttonid == "btnsave") {
            CreateProfile(username,emailid);
        }
        if(ErrMsg != string.Empty){ 
            
            return RedirectToAction("Error");
        }
        else{
            return View(SearchProfile(username));
        }
        
        
    }


    public bool CreateProfile(string username,string emailid)
    {
        try{
            Profile p = new Profile(){ username=username, emailid = emailid };
             dbconn.Add(p);
             dbconn.SaveChanges();
             return true;
        }
        catch(Exception e)
        {
            ErrMsg = e.GetBaseException().ToString();
            _logger.LogCritical("CreateProfile : " + ErrMsg);
            return false;
        }
        
    }

     public List<Profile> SearchProfile(string username)
    {
        var profiles = dbconn.Profiles
        .Where(u => u.username == username)
        .ToList();
        return profiles;

      
    }
   

}
