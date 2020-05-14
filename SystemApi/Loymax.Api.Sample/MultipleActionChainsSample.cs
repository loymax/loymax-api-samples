using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class MultipleActionChainsSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Products"); // Find a product group with the Products name, which must be previously created in the Product groups section

            var offerBuilder = OfferImportBuilder.Create("Sample3. Fixed price for the product unit.") // Set the name of the offer
                .WithDescription("Several chains with a given fixed value.") // Add a description for the offer
                .WithPartner(partnerId) // Specify Partner's ID
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithPriority(30); // Set the priority for the offer

            // Add the 1st chain to process the discount calculation event.
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType.OnlyGoods, // Set an additional condition under which the filter will trigger: None
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }) // Set the product group found earlier
                .WithDirectDiscount( // Set the Direct Discount action
                    dicsountType: ActionDiscountType.PricePerUnit, // Set the calculation method: Fixed unit price
                    value: 450.0); // Set the discount amount

            // Add the 2nd chain to process the discount calculation event.
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType.OnlyGoods, // Set an additional condition under which the filter will trigger: None
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}) // Set the product group found earlier
                .WithDirectDiscount( // Set the Direct Discount action
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Set the calculation method: Fixed unit price
                    value: 350.0); // Set the discount amount

            // Add the 3rd chain to process the discount calculation event.
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType.OnlyGoods, // Set an additional condition under which the filter will trigger: None
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}) // Set the product group found earlier
                .WithDirectDiscount( // Set the Direct Discount action
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Set the calculation method: Fixed unit price
                    value: 250.0); // Set the discount amount

            var offer = offerBuilder.Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
