﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Email
{
    public class EmailService : IEmailService
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly SendGridService _sendGridService;

        public EmailService(MeredithDbContext meredithDbContext, SendGridService sendGridService)
        {
            _meredithDbContext = meredithDbContext;
            _sendGridService = sendGridService;
        }

        public async Task SendReservationEmail(Reservation reservation, RoomType roomType, IEnumerable<Price> dailyPrices,
            decimal vatAmount, int paidDays, string country, string phoneNumber)
        {
            var hotel = await _meredithDbContext.Hotels
                .Include(item => item.Translations)
                .ThenInclude(item => item.Language)
                .Include(item => item.Page)
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

            var sendGridAccount = await GetSendGridAccount(hotel.CompanyId.Value);

            var templateData = new
            {
                bcc = sendGridAccount.Bcc,
                resort = new
                {
                    featuredImage = hotel.Page.FeaturedImage,
                    h2 = hotel.Page.Title
                },
                numberOfGuests = reservation.NumberOfGuests,
                checkIn = reservation.Start.ToString("ddd, d MMM"),
                checkOut = reservation.End.ToString("ddd, d MMM"),
                nightsCount = paidDays,
                prices = dailyPrices.Select(item => new
                {
                    date = item.Date.ToString("ddd, d MMM"),
                    amount = Math.Round(item.Amount, 2)
                }),
                vat = Math.Round(vatAmount, 2),
                amount = Math.Round(reservation.Amount, 2),
                name = reservation.Name,
                phoneCountry = country,
                phone = phoneNumber,
                email = reservation.Email,
                message = reservation.Message,
                roomDescriptionHTML = GetRoomDescription(hotel)
            };

            await _sendGridService.SendEmail(sendGridAccount.FromEmail, sendGridAccount.FromEmailName, reservation.User.Email, reservation.User.UserName, sendGridAccount.TemplateId, templateData);
        }

        private string GetRoomDescription(Data.Entity.Models.Modules.Hotel.Hotel hotel)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"<div>{Markdown.ToHtml(hotel.Page.Description)}</div>");
            stringBuilder.Append("<div>");
            
            var values = hotel.Spaces.SelectMany(item => item.Translations)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Name).ToList();

            AddSection("Spaces", values, stringBuilder);

            values = hotel.Amenities.SelectMany(item => item.Translations)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Text).ToList();

            AddSection("Amenities", values, stringBuilder);

            values = hotel.Rules.SelectMany(item => item.Translations)
                .Where(item => item.Language.Culture == "en-US")
                .Select(item => item.Text).ToList();

            AddSection("Rules", values, stringBuilder);

            values = hotel.Translations.Where(item => item.Language.Culture == "en-US")
                .Select(item => item.GettingAround).ToList();

            stringBuilder.Append("<!-- getting around -->");
            stringBuilder.Append($"<div>{string.Join("\n", values.Select(item => Markdown.ToHtml(item)))}</div>");

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
                  {string.Join("\n", values.Select(item => Markdown.ToHtml(item)))}
                </div>"
            );
        }

        private async Task<SendGridAccount> GetSendGridAccount(int companyId)
        {
            var sendGridAccount = await _meredithDbContext.SendGridAccounts
                .Where(s => s.CompanyId == companyId)
                .FirstOrDefaultAsync();

            if (sendGridAccount is null)
            {
                if (await _meredithDbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new RecordNotFoundException($"Company {companyId} does not have SendGrid configured");
                }

                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            return sendGridAccount;
        }
    }
}