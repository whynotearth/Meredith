using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Hotel
{
    public class ReservationService
    {
        private const string MetadataReservationIdKey = "Reservation.Id";
        private const string MetadataUserIdKey = "User.Id";

        private readonly MeredithDbContext _meredithDbContext;
        private readonly ClaimsPrincipal _user;
        private readonly IUserManager _userManager;
        private readonly IStripeService _stripeService;
        private readonly IEmailService _emailService;

        public ReservationService(MeredithDbContext meredithDbContext, ClaimsPrincipal user, IUserManager userManager,
            IStripeService stripeService, IEmailService emailService)
        {
            _meredithDbContext = meredithDbContext;
            _user = user;
            _userManager = userManager;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        public async Task<Reservation> CreateReservation(int roomTypeId, DateTime startDate, DateTime endDate,
            string fullName, string email, string? message, string? phoneCountry, string phone, int numberOfGuests)
        {
            var roomType = await _meredithDbContext.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId);

            if (roomType is null)
            {
                throw new RecordNotFoundException();
            }

            var availableRooms = await _meredithDbContext.Rooms
                .Where(r => !r.Reservations
                    .Any(re => re.Start >= startDate && re.End <= endDate))
                .ToListAsync();

            if (availableRooms.Count == 0)
            {
                throw new InvalidActionException("There are no rooms available of this type");
            }

            var totalDays = (int) endDate.Subtract(startDate).TotalDays;
            if (totalDays <= 0)
            {
                throw new InvalidActionException("Invalid number of days to reserve");
            }

            var dailyPrices = await _meredithDbContext.Prices
                .Where(p => p.Date >= startDate && p.Date < endDate).ToListAsync();
            
            var paidDays = dailyPrices.Count;

            if (paidDays != totalDays)
            {
                throw new InvalidActionException("Not all days have prices set");
            }

            var totalAmount = dailyPrices.Sum(item => item.Amount);
            // 10% VAT
            var vat = totalAmount / 10;
            totalAmount += vat;

            var user = await _userManager.GetUserAsync(_user);
            var reservation = new Reservation
            {
                Amount = totalAmount,
                Created = DateTime.UtcNow,
                Start = startDate,
                End = endDate,
                Email = email,
                Message = message,
                Name = fullName,
                NumberOfGuests = numberOfGuests,
                Phone = phone,
                RoomId = availableRooms.First().Id,
                User = user
            };

            _meredithDbContext.Reservations.Add(reservation);
            await _meredithDbContext.SaveChangesAsync();

            await _emailService.SendReservationEmail(reservation, roomType, dailyPrices, vat,
                paidDays, phoneCountry, phone);

            return reservation;
        }

        public async Task<string> PayReservation(int reservationId, decimal amount, Dictionary<string, string> metadata)
        {
            var (reservation, company, user) = await GetReservation(reservationId);
            if (reservation == null)
            {
                throw new RecordNotFoundException();
            }

            var accountId = await GetStripeAccountFromCompany(company.Id);

            metadata[MetadataReservationIdKey] = reservationId.ToString();
            metadata[MetadataUserIdKey] = user.Id.ToString();

            var paymentIntent = await _stripeService.CreatePaymentIntent(accountId, amount, user.Email, metadata);

            var payment = new Payment
            {
                Amount = amount,
                Created = DateTime.UtcNow,
                ReservationId = reservation.Id,
                Status = Payment.Statuses.IntentGenerated,
                PaymentIntentId = paymentIntent.Id,
                UserId = user.Id
            };
            _meredithDbContext.Payments.Add(payment);
            await _meredithDbContext.SaveChangesAsync();

            return paymentIntent.ClientSecret;
        }

        public async Task ConfirmPayment(string json, string stripSignatureHeader)
        {
            var paymentIntent = _stripeService.ConfirmPaymentIntent(json, stripSignatureHeader);

            var payment = new Payment
            {
                Amount = paymentIntent.Amount ?? 0,
                Created = DateTime.UtcNow,
                ReservationId = int.Parse(paymentIntent.Metadata[MetadataReservationIdKey]),
                Status = Payment.Statuses.Fulfilled,
                PaymentIntentId = paymentIntent.Id,
                UserId = int.Parse(paymentIntent.Metadata[MetadataUserIdKey])
            };

            _meredithDbContext.Payments.Add(payment);
            await _meredithDbContext.SaveChangesAsync();
        }

        private async Task<(Reservation, Company, User)> GetReservation(int reservationId)
        {
            var user = await _userManager.GetUserAsync(_user);
            var results = await _meredithDbContext.Reservations
                .Where(r => r.Id == reservationId && r.UserId == user.Id)
                .Select(r => new
                {
                    Reservation = r,
                    r.Room.RoomType.Hotel.Company
                })
                .FirstOrDefaultAsync();

            if (results.Company == null)
            {
                throw new Exception("This hotel does not have a bound company");
            }

            return (results.Reservation, results.Company, user);
        }

        private async Task<string> GetStripeAccountFromCompany(int companyId)
        {
            var stripeAccountId = await _meredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();

            if (stripeAccountId is null)
            {
                if (await _meredithDbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new RecordNotFoundException($"Company {companyId} does not have Stripe configured");
                }

                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            return stripeAccountId;
        }
    }
}