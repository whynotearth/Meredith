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
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.Hotel
{
    public class ReservationService
    {
        private const string MetadataReservationIdKey = "Reservation.Id";
        private const string MetadataUserIdKey = "User.Id";

        protected MeredithDbContext MeredithDbContext { get; }

        protected ClaimsPrincipal User { get; }

        protected UserManager UserManager { get; }

        protected StripeService StripeService { get; }

        public ReservationService(
            MeredithDbContext meredithDbContext,
            ClaimsPrincipal user,
            UserManager userManager,
            StripeService stripeService)
        {
            MeredithDbContext = meredithDbContext;
            User = user;
            UserManager = userManager;
            StripeService = stripeService;
        }

        public async Task<Reservation> CreateReservation(
            int roomTypeId,
            DateTime startDate,
            DateTime endDate,
            string fullName,
            string email,
            string message,
            string phone,
            int numberOfGuests)
        {
            var roomType = await MeredithDbContext.RoomTypes
                .Where(rt => rt.Id == roomTypeId)
                .Select(rt => new
                {
                    Price = rt.Prices
                        .Where(p => p.Date >= startDate && p.Date < endDate)
                        .Sum(p => p.Amount),
                    PaidDays = rt.Prices
                        .Where(p => p.Date >= startDate && p.Date < endDate)
                        .Count(),
                    AvailableRooms = rt.Rooms
                        .Where(r => !r.Reservations
                            .Any(re => re.Start >= startDate && re.End <= endDate))
                        .ToList()
                })
                .FirstOrDefaultAsync();
            if (roomType == null)
            {
                throw new RecordNotFoundException();
            }

            if (roomType.AvailableRooms.Count == 0)
            {
                throw new InvalidActionException("There are no rooms available of this type");
            }

            var totalDays = endDate.Subtract(startDate).TotalDays;
            if (totalDays <= 0)
            {
                throw new InvalidActionException("Invalid number of days to reserve");
            }

            if (roomType.PaidDays != totalDays)
            {
                throw new InvalidActionException("Not all days have prices set");
            }

            var user = await UserManager.GetUserAsync(User);
            var reservation = new Reservation
            {
                Amount = roomType.Price,
                Created = DateTime.UtcNow,
                Start = startDate,
                End = endDate,
                Email = email,
                Message = message,
                Name = fullName,
                NumberOfGuests = numberOfGuests,
                Phone = phone,
                RoomId = roomType.AvailableRooms.First().Id,
                User = user
            };
            MeredithDbContext.Reservations.Add(reservation);
            await MeredithDbContext.SaveChangesAsync();
            return reservation;
        }

        public async Task<string> PayReservation(int reservationId, decimal amount, Dictionary<string, string> metadata)
        {
            var (reservation, company, user) = await GetReservation(reservationId);
            if (reservation == null)
            {
                throw new RecordNotFoundException();
            }

            var stripeAccountId = await GetStripeAccountFromCompany(company.Id);

            metadata[MetadataReservationIdKey] = reservationId.ToString();
            metadata[MetadataUserIdKey] = user.Id.ToString();

            var paymentIntent = await StripeService.CreatePaymentIntent(stripeAccountId, amount, user.Email, metadata);

            var payment = new Payment
            {
                Amount = amount,
                Created = DateTime.UtcNow,
                ReservationId = reservation.Id,
                Status = Payment.Statuses.IntentGenerated,
                PaymentIntentId = paymentIntent.Id,
                UserId = user.Id
            };
            MeredithDbContext.Payments.Add(payment);
            await MeredithDbContext.SaveChangesAsync();

            return paymentIntent.ClientSecret;
        }

        public async Task ConfirmPayment(string json, string stripSignatureHeader)
        {
            var paymentIntent =  StripeService.ConfirmPaymentIntent(json, stripSignatureHeader);

            var payment = new Payment
            {
                Amount = (long) paymentIntent.Amount,
                Created = DateTime.UtcNow,
                ReservationId = int.Parse(paymentIntent.Metadata[MetadataReservationIdKey]),
                Status = Payment.Statuses.Fulfilled,
                PaymentIntentId = paymentIntent.Id,
                UserId = int.Parse(paymentIntent.Metadata[MetadataUserIdKey])
            };

            MeredithDbContext.Payments.Add(payment);
            await MeredithDbContext.SaveChangesAsync();
        }

        private async Task<(Reservation, Company, User)> GetReservation(int reservationId)
        {
            var user = await UserManager.GetUserAsync(User);
            var results = await MeredithDbContext.Reservations
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
            var stripeAccountId = await MeredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();

            if (stripeAccountId == null)
            {
                if (await MeredithDbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new RecordNotFoundException($"Company {companyId} does not have Stripe configured");
                }

                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            return stripeAccountId;
        }
    }
}