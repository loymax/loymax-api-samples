using System;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class ChequeMessageSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var offer = OfferImportBuilder
                .Create("Sample6. Message to the cheque of a specific target audience") // Set the name of the offer
                .WithDescription("Summable offer providing specific messages for a specific target audience") // Add a description for the offer
                .WithPartner(partnerId) // Specify Partner's ID 
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithPriority(45) // Set the priority for the offer
                .IsSum() // Summable offer
                .WithTargetGroup("1") // Set the identifier for target audience, which must be previously created in the Target audiences section
                .AddChain<PurchaseCalculateEventDto>() // Add a chain to process discount calculation event
                .WithChequeMessage("С любовью. ваш МегаМакс") // Set the message on the cheque
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
