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

            // учётные данные запросите у специалистов Loymax
            client.Authorization("login-here", "password-here");


            var offers = OfferImportBuilder.Create("SampleTitle") // Установить название акции
                .WithPartner("43168568-a7f8-4582-9d45-49857634251d") // указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // применяется с момента загрузки
                .WithExpirationDate(DateTime.Now.AddDays(1)) // Истекает срок действия через 1 день
                .AddChain<PurchaseCalculateEventDto>() // добавляем цепочку для обработки события расчёта скидки
                .WithCardStateFilter(CardState.Activated) // только для активированных карт
                .WithDirectDiscount(ActionDiscountType.Percent, 0.1) // 10% прямой скидки
                .Build();

            var result = client.OfferImportExport_PostOffersAsync(offers).Result; // импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
