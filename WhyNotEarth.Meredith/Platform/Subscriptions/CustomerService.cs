namespace WhyNotEarth.Meredith.Platform.Subscriptions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Services;

    public class CustomerService
    {
        public CustomerService(
            IDbContext meredithDbContext,
            IStripeCustomerService stripeCustomerService)
        {
            _meredithDbContext = meredithDbContext;
            _stripeCustomerService = stripeCustomerService;
        }

        private readonly IDbContext _meredithDbContext;

        private readonly IStripeCustomerService _stripeCustomerService;

        public async Task<Customer> AddCustomerAsync(int tenantId, int? companyId = null)
        {
            var tenant = await _meredithDbContext.Tenants
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
            {
                throw new RecordNotFoundException($"Tenant {tenantId} not found");
            }

            var company = await _meredithDbContext.Companies
                .Include(c => c.StripeAccount)
                .FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null && companyId.HasValue)
            {
                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            if (company != null && company?.StripeAccount?.StripeUserId == null)
            {
                throw new InvalidActionException($"Company {companyId} not configured for stripe");
            }

            var stripeCustomerId = await _stripeCustomerService.AddCustomerAsync(tenant.Owner.Email, tenant.Owner.FullName, company?.StripeAccount?.StripeUserId);
            var customer = new Customer
            {
                CompanyId = companyId,
                TenantId = tenant.Id,
                StripeId = stripeCustomerId,
            };
            _meredithDbContext.PlatformCustomers.Add(customer);
            await _meredithDbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<PaymentCard> AddCardAsync(int tenantId, string? token)
        {
            var customer = await _meredithDbContext.PlatformCustomers
                .Include(c => c.Company)
                .ThenInclude(c => c!.StripeAccount)
                .FirstOrDefaultAsync(c => c.TenantId == tenantId);
            if (customer == null)
            {
                var tenant = await _meredithDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
                if (tenant == null)
                {
                    throw new RecordNotFoundException($"Tenant ID {tenantId} not found");
                }

                customer = await AddCustomerAsync(tenantId, tenant.CompanyId);
            }

            var cardDetail = await _stripeCustomerService.AddCardAsync(customer.StripeId, token, customer.Company?.StripeAccount?.StripeUserId);
            var card = new PaymentCard
            {
                StripeId = cardDetail.Id,
                CustomerId = customer.Id,
                Last4 = cardDetail.Last4,
                Brand = ParseBrand(cardDetail.Brand),
                ExpirationMonth = cardDetail.ExpirationMonth,
                ExpirationYear = cardDetail.ExpirationYear
            };
            _meredithDbContext.PlatformCards.Add(card);
            await _meredithDbContext.SaveChangesAsync();
            return card;
        }

        private PaymentCard.Brands ParseBrand(string brand) => brand switch
        {
            "American Express" => PaymentCard.Brands.Amex,
            "Diners Club" => PaymentCard.Brands.DinersClub,
            "Discover" => PaymentCard.Brands.Discover,
            "JCB" => PaymentCard.Brands.Jcb,
            "MasterCard" => PaymentCard.Brands.Mastercard,
            "Visa" => PaymentCard.Brands.Visa,
            "UnionPay" => PaymentCard.Brands.UnionPay,
            _ => PaymentCard.Brands.Unknown
        };

        public async Task DeleteCardAsync(int cardId)
        {
            var card = await _meredithDbContext.PlatformCards
                .Include(c => c.Customer)
                .ThenInclude(c => c!.Company)
                .ThenInclude(c => c!.StripeAccount)
                .FirstOrDefaultAsync(c => c.Id == cardId);
            await _stripeCustomerService.DeleteCardAsync(card.Customer!.StripeId, card.StripeId, card.Customer.Company?.StripeAccount?.StripeUserId);
        }

        public async Task<List<Transaction>> GetTransactions(int customerId)
        {
            var customer = await _meredithDbContext.PlatformCustomers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    c.StripeId,
                    c.Company!.StripeAccount!.StripeUserId
                })
                .FirstOrDefaultAsync();
            if (customer == null)
            {
                return new List<Transaction>();
            }

            var invoices = await _stripeCustomerService.GetInvoices(customer.StripeId, customer.StripeUserId);
            return invoices.Select(i => new Transaction
            {
                Amount = i.Total * .01m,
                Date = i.Created,
                PaymentMethod = i.Charge?.PaymentMethod,
                StatementLink = i.HostedInvoiceUrl,
                TransactionId = i.Id
            }).ToList();
        }
    }
}