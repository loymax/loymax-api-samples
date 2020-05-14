using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class GoodsGroupDiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Products"); // Find a product group with the Products name, which must be previously created in the Product groups section

            var offer = OfferImportBuilder.Create("Sample1. Discount on product group.") // Set the name of the offer
                .WithDescription("10% discount on product group with exclusive set.") // Add a description for the offer
                .WithPartner(partnerId) // Specify Partner's ID
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithPriority(20) // Set the priority for the offer
                .AddChain<PurchaseCalculateEventDto>() // Add a chain to process discount calculation event
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType.OnlyGoods, // Set an additional condition under which the filter will trigger: None
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }) // Set the product group found earlier
                .WithDirectDiscount( // Set the Direct discount action
                    dicsountType: ActionDiscountType.Percent, // Set the calculation method: Percentage
                    value: 10.0, // Set the discount amount
                    calculationExclusionTypes: new List<CalculationExclusionDiscountType> { CalculationExclusionDiscountType.Discount }) // Exclude the amount from the calculation: Direct discount
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
