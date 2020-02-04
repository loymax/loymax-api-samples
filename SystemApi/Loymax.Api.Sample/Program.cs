using System;
using System.Collections.Generic;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client()
            {
                BaseUrl = "https://armv2-test.loymax.tech/api/"
            };

            client.Authorization("login-here", "password-here");

            var offers = new OffersImportModel {Version = ProtocolVersion.Version20, Offers = new List<OfferDto>()};

            var offer = new OfferDto
            {
                Rules = new OfferRules {Events = new List<OfferEventDto>()}, Title = "Sample", Priority = 10,
                Partners = new OfferPartners { Partners = new List<PartnerDto>()},
                Id = Guid.NewGuid().ToString("N"),
                IsSum = false,
                WorkingState = OfferWorkingState.Running,
                ChangesState = OfferChangesState.Approved,
                ApplyChangesDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(1)
            };

            offer.Partners.Partners.Add(new PartnerDto { Id = "43168568-a7f8-4582-9d45-49857634251d" });

            var offerEvent = new PurchaseCalculateEventDto {Chains = new List<OfferActionsChainDto>()};
            var chain = new OfferActionsChainDto {Name = "Первая цепочка", Order = 0, Actions = new List<ActionDto>(), Filters = new List<FilterDto>()};
            chain.Actions.Add(new DirectDiscountActionDto { Percent = 0.1, DiscountType =  ActionDiscountType.Percent});

            offerEvent.Chains.Add(chain);
            offer.Rules.Events.Add(offerEvent);

            offers.Offers.Add(offer);
            var result = client.OfferImportExport_PostOffersAsync(offers).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
