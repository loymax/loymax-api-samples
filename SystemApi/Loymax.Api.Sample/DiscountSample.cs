using System;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class DiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var offer = OfferImportBuilder.Create("SampleTitle")  // Set offer name
                .WithPartner(partnerId) // Indicate partner id
                .WithState(OfferWorkingState.Running) // The offer will be started immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // It has been applied from the moment of loading
                .WithExpirationDate(DateTime.Now.AddDays(1)) // Expires in 1 day
                .AddChain<PurchaseCalculateEventDto>() // The chain for discount calculation event processing is added
                .WithCardStateFilter(CardState.Activated) // for activated cards only
                .WithDirectDiscount(ActionDiscountType.Percent, 0.1) // 10% direct discount
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
