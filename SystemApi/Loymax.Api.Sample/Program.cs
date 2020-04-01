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

            // Скидка 10% 
            await DiscountSample.ImportOfferAsync(client, partnerId);

            // Скидка 10% на группу товаров с исключающим набором
            await GoodsGroupDiscountSample.ImportOfferAsync(client, partnerId);

            // Скидка для ЦА, в определенном магазине, на конечный срок действия
            await TargetGroupDiscountSample.ImportOfferAsync(client, partnerId);

            // Несколько цепочек с заданным фиксированном значением
            await MultipleActionChainsSample.ImportOfferAsync(client, partnerId);

            // При покупке 5ти товаров, 6ой в подарок
            await GoodsSetDiscountSample.ImportOfferAsync(client, partnerId);

            // При покупке по цене трех единиц акционного товара в рамках одного чека Участник приобретает эти три товара по фиксированной цене N
            await GoodsQuantityDiscountSample.ImportOfferAsync(client, partnerId);

            // Суммируемая акция, предоставляющая определенные сообщения для конкретной ЦА
            await ChequeMessageSample.ImportOfferAsync(client, partnerId);
        }
    }
}
