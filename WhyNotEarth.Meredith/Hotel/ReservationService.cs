using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Hotel
{
    public class ReservationService
    {
        private const string MetadataReservationIdKey = "Reservation.Id";
        private const string MetadataUserIdKey = "User.Id";

        private readonly IDbContext _dbContext;
        private readonly ClaimsPrincipal _user;
        private readonly IUserService _userService;
        private readonly IStripeService _stripeService;
        private readonly IEmailService _emailService;

        public ReservationService(IDbContext dbContext, ClaimsPrincipal user, IUserService userService,
            IStripeService stripeService, IEmailService emailService)
        {
            _dbContext = dbContext;
            _user = user;
            _userService = userService;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        public async Task<HotelReservation> CreateReservation(int roomTypeId, DateTime startDate, DateTime endDate,
            string fullName, string email, string? message, string? phoneCountry, string phone, int numberOfGuests)
        {
            var roomType = await _dbContext.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId);

            if (roomType is null)
            {
                throw new RecordNotFoundException();
            }

            var availableRooms = await _dbContext.Rooms
                .Where(r => !r.Reservations
                    .Any(re => re.Start >= startDate && re.End <= endDate))
                .ToListAsync();

            if (availableRooms.Count == 0)
            {
                throw new InvalidActionException("There are no rooms available of this type");
            }

            var totalDays = (int)endDate.Subtract(startDate).TotalDays;
            if (totalDays <= 0)
            {
                throw new InvalidActionException("Invalid number of days to reserve");
            }

            var dailyPrices = await _dbContext.Prices
                .OfType<HotelPrice>()
                .Where(p => p.Date >= startDate && p.Date < endDate && p.RoomTypeId == roomType.Id).ToListAsync();

            var paidDays = dailyPrices.Count;

            if (paidDays != totalDays)
            {
                throw new InvalidActionException($"Only {paidDays} rooms are configured while {totalDays} have been reserved");
            }

            var totalAmount = dailyPrices.Sum(item => item.Amount);
            // 10% VAT
            var vat = totalAmount / 10;
            totalAmount += vat;

            var user = await _userService.GetUserAsync(_user);
            var reservation = new HotelReservation
            {
                Amount = totalAmount,
                CreatedAt = DateTime.UtcNow,
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

            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync();

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
                CreatedAt = DateTime.UtcNow,
                ReservationId = reservation.Id,
                Status = PaymentStatus.IntentGenerated,
                PaymentIntentId = paymentIntent.Id,
                UserId = user.Id
            };
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            return paymentIntent.ClientSecret;
        }

        public async Task ConfirmPayment(string json, string stripSignatureHeader)
        {
            var paymentIntent = _stripeService.ConfirmPaymentIntent(json, stripSignatureHeader);

            var payment = new Payment
            {
                Amount = paymentIntent.Amount,
                CreatedAt = DateTime.UtcNow,
                ReservationId = int.Parse(paymentIntent.Metadata[MetadataReservationIdKey]),
                Status = PaymentStatus.Fulfilled,
                PaymentIntentId = paymentIntent.Id,
                UserId = int.Parse(paymentIntent.Metadata[MetadataUserIdKey])
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<(HotelReservation, Company, User)> GetReservation(int reservationId)
        {
            var user = await _userService.GetUserAsync(_user);
            var results = await _dbContext.Reservations.OfType<HotelReservation>()
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
            var stripeAccountId = await _dbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();

            if (stripeAccountId is null)
            {
                if (await _dbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new RecordNotFoundException($"Company {companyId} does not have Stripe configured");
                }

                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            return stripeAccountId;
        }
    }
}