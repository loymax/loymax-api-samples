using System.Threading.Tasks;
using Loymax.SystemApi.SDK;

namespace Loymax.Api.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client()
            {
                BaseUrl = "https://dev.loymax.tech/systemapi/"
            };

            // ask Loymax specialists for credentials
            await client.Authorization("login-here", "password-here");

            var partnerId = "9957c76a-fe1a-499e-d5ca-1ab605a6a166";

            // 10% discount
            await DiscountSample.ImportOfferAsync(client, partnerId);

            // 10% discount on product group with excluding set
            await GoodsGroupDiscountSample.ImportOfferAsync(client, partnerId);

            // Discount for target audience in a specific point of sale for the expiration date
            await TargetGroupDiscountSample.ImportOfferAsync(client, partnerId);

            // Several chains with a given fixed value
            await MultipleActionChainsSample.ImportOfferAsync(client, partnerId);

            // When buying 5 products, 6th as a gift
            await GoodsSetDiscountSample.ImportOfferAsync(client, partnerId);

            // When purchasing at the price of three units of a promotional product within one cheque, the Member purchases these three products at a fixed price N
            await GoodsQuantityDiscountSample.ImportOfferAsync(client, partnerId);

            // Summable offer providing specific messages for a specific target audience
            await ChequeMessageSample.ImportOfferAsync(client, partnerId);
        }
    }
}
