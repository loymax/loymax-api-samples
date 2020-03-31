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
            var offer = OfferImportBuilder.Create("SampleTitle") // Установливаем название акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с момента загрузки
                .WithExpirationDate(DateTime.Now.AddDays(1)) // Истекает срок действия через 1 день
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithCardStateFilter(CardState.Activated) // Только для активированных карт
                .WithDirectDiscount(ActionDiscountType.Percent, 0.1) // 10% прямой скидки
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
