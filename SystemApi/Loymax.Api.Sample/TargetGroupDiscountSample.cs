using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class TargetGroupDiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId) 
        { 
            var offer = OfferImportBuilder.Create("Sample2. Discount for target audience") // Set the name of the offer
                .WithDescription("Discount for target audience in a specific POS on the expiration date.") // Add a description for the offer
                .WithPartner(partnerId) // Specify Partner's ID
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithExpirationDate(DateTime.Now.AddDays(7)) // Set the expiration date of the offer
                .WithPriority(25) // Set the priority of the offer
                .WithMerchantFilter(new string[] { "75e67dae-130a-4030-192d-18b02d907950", "55403257-1663-4bd1-9725-20b3bfe51392" }) // Set GUIDs of points of sale in which the offer will be valid
                .WithTargetGroup(new string[] { "1", "2" }) // Set the identifiers of the target audiences, which must be previously created in the Target audiences section
                .AddChain<PurchaseCalculateEventDto>() // Add a chain to process discount calculation event
                .WithDirectDiscount( // Set the Direct discount action
                    dicsountType: ActionDiscountType.Percent, // Set the calculation method: Percentage
                    value: 15.0, // Set the discount amount
                    calculationExclusionTypes: new List<CalculationExclusionDiscountType> { CalculationExclusionDiscountType.Discount }) // Exclude amount from the calculation: Direct discount
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
