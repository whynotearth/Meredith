namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;

    [ApiVersion("0")]
    [Route("/api/v0/hotels")]
    [EnableCors]
    public class HotelController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        public HotelController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }
    }
}