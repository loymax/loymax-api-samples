using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class GoodsSetDiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Find a product group with the Products name, which must be previously created in the Product groups section

            var offer = OfferImportBuilder.Create("Sapmle4. N+M") // Set the name of the offer
                .WithPartner(partnerId) // Specify Partner's ID
                .WithDescription("При покупке 5ти товаров, 6ой в подарок.") // Add a description for the offer
                .WithState(OfferWorkingState.Running) // The offer will be run immediately
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Applied from specified date
                .WithPriority(35) // Set the priority of the offer
                .AddChain<PurchaseCalculateEventDto>() // Add a chain to process discount calculation event
                .WithGoodsSetDivide( // Set the Dividing into Sets action
                    new List<GoodsSetItemDto>
                    {
                            new GoodsSetItemDto // Create a new set
                            {
                                GoodsGroup = new GoodsGroupExDto // The item source for the set is the product group found.
                                {
                                    Id = goodsGroup.Data.First().ExternalId.Value.ToString(), // Set the product group ID 
                                },
                                Value = 6.000, // Set the number of products in the set
                            },
                    },
                    ChequeGoodsSortType.PriceAscending, // Set the type of product sorting: Ascending prices
                    ChequeGoodsSetType.Quantity) // Set the condition for dividing: Quantity of products
                .WithChequePositionGoodsFilter( // Set the Product filter
                    type: ChequeGoodsFilterType.GoodsQuantity, // Set an additional condition under which the filter will trigger: Quantity
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }, // Set the product group found earlier
                    firstValue: 1.0, // The filter will only trigger if the number of units in the cheque is less than or equal to 1.0
                    @operator: ComparisonOperator.LessOrEqual,
                    chequeGoodsSortType: ChequeGoodsSortType.PriceAscending,
                    dividePositions: true)
                .WithDirectDiscount( // Set the Direct Discount action
                    dicsountType: ActionDiscountType.Percent, // Set the calculation method: Percentage
                    value: 100) // Set the discount amount
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Import the offer into the system
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
