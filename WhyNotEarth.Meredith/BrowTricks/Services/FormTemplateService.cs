using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormTemplateService : IFormTemplateService
    {
        private readonly IDbContext _dbContext;
        private readonly IClientService _clientService;
        private readonly TenantService _tenantService;

        public FormTemplateService(TenantService tenantService, IDbContext dbContext, IClientService clientService)
        {
            _tenantService = tenantService;
            _dbContext = dbContext;
            _clientService = clientService;
        }

        public async Task CreateDefaultsAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplates = GetDefaultTemplates(tenant.Id);

            _dbContext.FormTemplates.AddRange(formTemplates);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateAsync(string tenantSlug, FormTemplateModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplate = Map(new FormTemplate(), model, tenant.Id);

            _dbContext.FormTemplates.Add(formTemplate);
            await _dbContext.SaveChangesAsync();

            return formTemplate.Id;
        }

        public async Task EditAsync(string tenantSlug, int formTemplateId, FormTemplateModel model, User user)
        {
            await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId && item.IsDeleted == false);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            formTemplate = Map(formTemplate, model, formTemplate.TenantId);

            _dbContext.FormTemplates.Update(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<FormTemplate>> GetListAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplates = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .Where(item => item.TenantId == tenant.Id && item.IsDeleted == false)
                .ToListAsync();

            foreach (var formTemplate in formTemplates)
            {
                formTemplate.Items = formTemplate.Items.OrderBy(item => item.Id).ToList();
            }

            return formTemplates;
        }

        public async Task DeleteAsync(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            formTemplate.IsDeleted = true;

            _dbContext.FormTemplates.Update(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<FormTemplate> GetAsync(int formTemplateId, User user)
        {
            var formTemplate = await GetAsync(formTemplateId);

            await _clientService.ValidateOwnerOrClientAsync(formTemplate.TenantId, user);

            return formTemplate;
        }

        public async Task<FormTemplate> GetAsync(int formTemplateId)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId && item.IsDeleted == false);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            formTemplate.Items = formTemplate.Items.OrderBy(item => item.Id).ToList();

            return formTemplate;
        }

        private FormTemplate Map(FormTemplate formTemplate, FormTemplateModel model, int tenantId)
        {
            formTemplate.TenantId = tenantId;
            formTemplate.Name = model.Name;
            formTemplate.Items = model.Items?.Select(Map).ToList() ?? new List<FormItem>();
            formTemplate.CreatedAt ??= DateTime.UtcNow;

            return formTemplate;
        }

        private FormItem Map(FormItemModel model)
        {
            return new FormItem
            {
                Type = model.Type.Value,
                IsRequired = model.IsRequired.Value,
                Value = model.Value,
                Options = model.Options
            };
        }

        private List<FormTemplate> GetDefaultTemplates(int tenantId)
        {
            return new List<FormTemplate>
            {
                new FormTemplate
                {
                    Name = "Pre and Post Care Agreement",
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                    Items = new List<FormItem>
                    {
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.Pdf,
                            Value =
                                "I have read, or had read to me, the above Pre and Post Care instructions and expectations. I agree to follow the above directions and understand that how I heal depends on how closely I follow said directions. I understand a touchup appointment will be needed to complete the process.",
                            Options = new List<string>
                            {
                                "https://res.cloudinary.com/whynotearth/image/upload/pg_1/v1602252454/BrowTricks/backend/Pre_and_Post_Care_Agreement_djswql.png",
                                "https://res.cloudinary.com/whynotearth/image/upload/pg_2/v1602252454/BrowTricks/backend/Pre_and_Post_Care_Agreement_djswql.png",
                                "https://res.cloudinary.com/whynotearth/image/upload/pg_3/v1602252454/BrowTricks/backend/Pre_and_Post_Care_Agreement_djswql.png",
                                "https://res.cloudinary.com/whynotearth/image/upload/pg_4/v1602252454/BrowTricks/backend/Pre_and_Post_Care_Agreement_djswql.png"
                            }
                        }
                    }
                },
                new FormTemplate
                {
                    Name = "Booking Fee and Cancellation Policy",
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                    Items = new List<FormItem>
                    {
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.AgreementRequest,
                            Value = @"Upon booking your first appointment, a $50 non- refundable deposit is charged to secure your appointment. The deposit will be deducted from the total cost of your initial procedure; the remaining balance will be due on your appointment day.  Your booking fee is non refundable, non transferable, and can only be applied to the scheduled service for the date and time secured. Please be sure about your appointment day and time before booking. 

If you need to reschedule, you may do so with Cancellation fee within 48 hours of your appointment.  A new booking fee will be charged for the new appointment date.  If you are unable to make your appointment, you MUST give a 48 hour notice to avoid a Cancellation Fee.  In this instance, 20% of the appointment is charged.  This pays for the time that was set aside for you and your appointment.  Your Booking Fee in this case is applied to the 25% charge, and a new Booking Fee must be paid to reschedule.  
All lost booking fees and cancellation fees may be waived at the discretion of owner on a case by case basis. 

I fully understand the Booking Fee and Cancellation Fee Policy, and hereby forfeit my Booking Fee to secure my appointment spot. 
I will also give 48 hour notice or pay 20% for my appointment reservation."
                        }
                    }
                },
                new FormTemplate
                {
                    Name = "Disclosure and Consent for Intradermal Cosmetic Procedure",
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                    Items = new List<FormItem>
                    {
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = "Your Full Name"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I have requested information below relating to the procedure of Intradermal Cosmetics so that I may make an informed decision as to whether or not to undergo the procedure.

Micropigmentation is the process of implanting micro pockets of pigment into the epidermis and upper dermal layer of the skin. This is a form of tattooing used for permanent cosmetics. I voluntarily request my intradermal technician to perform the following procedures:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I hereby authorize photographs of the work performed to be taken both before and after treatment, and that said photographs may be used for purposes of advertising and/or training purposes. I may request full face photos to be approved before posting.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"Are you under the care of a physician? If yes, please state the medical treatments, medications, and procedures you are being treated for or have in the past.

Please also list your Physician's Name and Phone Number. We must have this information to make sure any conditions will not affect the healing of your tattoo, or if it involves possible allergies to dye, pigment, or numbing agents."
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I have been told that there may be known and unknown risks and hazards related to the performance of the planned procedure and I understand that no warranty or guarantees have been made as to the results.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I acknowledge the manufacturers of the pigment and numbing agents require spot testing and specifically disclaims any responsibility for adverse reaction to applied products. I understand spot testing may identify individuals who develop immediate allergic reactions, however spot testing does not identify individuals who may have a delayed allergic reaction to pigment.

I have been told that this procedure may involve discomfort. I also understand that I can ask for more additional numbing as needed, but that the more numbing that is applied, the higher the risk of having an adverse reaction.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I understand the markings are permanent and that there is the possibility of hyperpigmentation resulting from a procedure, especially to individuals prone to hyperpigmentation from scars or other injuries. The pigment also has the risk of fanning or spreading, also known as pigment migration.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I understand that a follow up touchup procedure will most likely be needed. As little as 50% of the pigment may remain after healing.  Pigment will need to be re-implanted for a fully finished look. I also understand that annual touchups may also be needed. Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I understand other risks involved may include, but are not limited to:
allergic and other reactions to products applied during and after the procedure, skin irritation, swelling, and discomfort.  When reactions occur, they are generally from numbing products, pigment, or aftercare ointments. I understand opting for numbing cream increases the change of a reaction, and will notify my technician if I do not want it used. I understand a test patch is recommended by most manufacturers.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I have read, or have had read to me, and have received a copy of the Post Procedure Instructions and I understand its content. I understand that as with any procedure that opens the skin, infections can also rarely occur; following aftercare properly will greatly reduce this risk. I understand picking my scabs can cause scarring, and therefore will not do so.  I understand my tattoo is not permanent in my skin until it has fully healed.  I will do my best to follow after care instructions to the best of my ability.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I accept full responsibility for any and all, present and future, medical treatments and expenses I may incur in the event I need to seek treatments for any known or unknown reason associated with this procedure.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I have been given an opportunity to ask questions about the procedures to be done and the risks and hazards involved, and I believe that I have sufficient information to give informed consent.
Initial Here:"
                        },
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.TextResponse,
                            Value = @"I certify that I have read, or had read to me, the content of this consent, and I fully understand its contents.
Initial Here:"
                        },
                    }
                }
            };
        }
    }
}