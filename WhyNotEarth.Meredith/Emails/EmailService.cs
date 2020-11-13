﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Makrdown;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IDbContext _dbContext;
        private readonly SendGridService _sendGridService;
        private readonly IMarkdownService _markdownService;

        public EmailService(IDbContext dbContext, SendGridService sendGridService, IMarkdownService markdownService)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
            _markdownService = markdownService;
        }

        public async Task SendReservationEmail(HotelReservation hotelReservation, RoomType roomType, IEnumerable<HotelPrice> dailyPrices,
            decimal vatAmount, int paidDays, string? country, string phoneNumber)
        {
            var hotel = await _dbContext.Hotels
                .Include(item => item.Translations)
                .ThenInclude(item => item.Language)
                .Include(item => item.Page)
                .ThenInclude(item => item.Translations)
                .ThenInclude(item => item.Language)
                .Include(item => item.Spaces)
                .ThenInclude(item => item.Translations)
                .ThenInclude(item => item.Language)
                .Include(item => item.Amenities)
                .ThenInclude(item => item.Translations)
                .ThenInclude(item => item.Language)
                .Include(item => item.Rules)
                .ThenInclude(item => item.Translations)
                .ThenInclude(item => item.Language)
                .FirstOrDefaultAsync(item => item.Id == roomType.HotelId);

            if (hotel?.CompanyId is null)
            {
                // I don't think this is possible and I don't see any hotel without a companyId in db either
                // I don't know why we marked this is nullable
                throw new Exception("Hotel is not connected to any company.");
            }

            var templateData = new
            {
                resort = new
                {
                    featuredImage = hotel.Page.FeaturedImage,
                    h2 = hotel.Page.Translations?.FirstOrDefault(t => t.Language.Culture == "en-US")?.Title
                },
                numberOfGuests = hotelReservation.NumberOfGuests,
                checkIn = hotelReservation.Start.ToString("ddd, d MMM"),
                checkOut = hotelReservation.End.ToString("ddd, d MMM"),
                nightsCount = paidDays,
                prices = dailyPrices.Select(item => new
                {
                    date = item.Date.ToString("ddd, d MMM"),
                    amount = Math.Round(item.Amount, 2)
                }),
                vat = Math.Round(vatAmount, 2),
                amount = Math.Round(hotelReservation.Amount, 2),
                name = hotelReservation.Name,
                phoneCountry = country,
                phone = phoneNumber,
                email = hotelReservation.Email,
                message = hotelReservation.Message,
                roomDescriptionHTML = GetRoomDescription(hotel)
            };

            var to = Tuple.Create<string, string?>(hotelReservation.User.Email, hotelReservation.User.UserName);

            var emailInfo = new EmailMessage(hotel.CompanyId.Value, to)
            {
                TemplateData = templateData
            };
            await _sendGridService.SendEmailAsync(emailInfo);
        }

        private string GetRoomDescription(Hotel.Hotel hotel)
        {
            var stringBuilder = new StringBuilder();

            var html = _markdownService.ToHtml(
                hotel.Page.Translations?.FirstOrDefault(t => t.Language.Culture == "en-US")?.Description ??
                string.Empty);
            stringBuilder.Append($"<div>{html}</div>");
            stringBuilder.Append("<div>");

            var values = hotel.Spaces?.SelectMany(item => item.Translations!)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Name ?? string.Empty).ToList() ?? new List<string>();

            AddSection("Spaces", values, stringBuilder);

            values = hotel.Amenities?.SelectMany(item => item.Translations!)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Text ?? string.Empty).ToList() ?? new List<string>();

            AddSection("Amenities", values, stringBuilder);

            values = hotel.Rules?.SelectMany(item => item.Translations!)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Text ?? string.Empty).ToList() ?? new List<string>();

            AddSection("Rules", values, stringBuilder);

            values = hotel.Translations?.Where(item => item.Language.Culture == "en-US")
                .Select(item => item.GettingAround ?? string.Empty).ToList() ?? new List<string>();

            stringBuilder.Append("<!-- getting around -->");
            stringBuilder.Append($"<div>{string.Join("\n", values.Select(item => _markdownService.ToHtml(item)))}</div>");

            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private void AddSection(string header, ICollection<string> values, StringBuilder stringBuilder)
        {
            if (!values.Any())
            {
                return;
            }

            stringBuilder.Append($@"
                <!-- {header} -->
                <div>
                  <h2>{header}</h2>
                  {string.Join("\n", values.Select(item => _markdownService.ToHtml(item)))}
                </div>"
            );
        }
    }
}