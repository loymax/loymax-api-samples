using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class GoodsQuantityDiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var goodsGroup =
                await client.GoodsGroup_GetGoodsGroupsAsync(
                    filter_name: "Товары"); // Find a product group with the Products name, which must be previously created in the Product groups section

            var offer = OfferImportBuilder
                .Create("Sample5. Special price when buying three units of product") // Set the name of the offer
                .WithDescription("When buing at the price of three units of a promotional product within the framework of one cheque, the Member purchases these three items at a fixed price N.") // Add a description for the offer
                .WithPartner(partnerId) // Specify Partner's ID
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithPriority(40) // Set the priority for the offer
                .AddChain<PurchaseCalculateEventDto>() // Add a chain to process discount calculation event
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType
                        .GoodsQuantity, // Set an additional condition under which the filter will trigger: Quantity
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}, // Set the product group found earlier
                    firstValue: 3.0, // The filter only triggers if the number of units in the cheque is 3.0
                    @operator: ComparisonOperator.Equals)
                .WithDirectDiscount( // Set the Direct discount action
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Set the calculation method: Fixed unit price
                    value: 100.0) // Set the discount amount
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
