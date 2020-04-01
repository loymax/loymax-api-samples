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
                BaseUrl = "https://dev.loymax.tech/systemapi/"
            };

            // ask Loymax specialists for credentials
            client.Authorization("login-here", "password-here");


            var offers = OfferImportBuilder.Create("SampleTitle") // Set offer name
                .WithPartner("43168568-a7f8-4582-9d45-49857634251d") // Indicate partner id
                .WithState(OfferWorkingState.Running) // The offer will be started immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // It has been applied from the moment of loading
                .WithExpirationDate(DateTime.Now.AddDays(1)) // Expires in 1 day
                .AddChain<PurchaseCalculateEventDto>() // The chain for discount calculation event processing is added
                .WithCardStateFilter(CardState.Activated) // for activated cards only
                .WithDirectDiscount(ActionDiscountType.Percent, 0.1) // 10% direct discount
                .Build();

            var result = client.OfferImportExport_PostOffersAsync(offers).Result; // import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
