namespace WhyNotEarth.Meredith.App.Controllers
{
    using System.Threading.Tasks;
    using Company;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Pages;

    public class HomeController : Controller
    {
        protected CompanyService CompanyService { get; }

        protected PageDatabase PageDatabase { get; }

        protected PageDatabaseOptions PageDatabaseOptions { get; }

        public HomeController(
            CompanyService companyService,
            PageDatabase pageDatabase,
            IOptions<PageDatabaseOptions> pageDatabaseOptions)
        {
            CompanyService = companyService;
            PageDatabase = pageDatabase;
            PageDatabaseOptions = pageDatabaseOptions.Value;
        }

        [Route("/{companyKey}/{pageKey?}")]
        public async Task<IActionResult> Index(string companyKey, string pageKey)
        {
            var page = await CompanyService.GetCompanyPage(companyKey, pageKey);
            return View(page);
        }

        [Route("Refresh/{key}")]
        public async Task<IActionResult> Refresh(string key)
        {
            if (key == PageDatabaseOptions.Key)
            {
                await PageDatabase.Load();
                return Content("Database refreshed");
            }

            return Content("Bad refresh key");
        }
    }
}