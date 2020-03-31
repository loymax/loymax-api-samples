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
                .Create("Sample6. Сообщение на чек определенной ЦА") // Установливаем название акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithPriority(45) // Устанавливаем приоритет акции
                .IsSum() // Акция суммируемая
                .WithTargetGroup("1") // Задаем целевую аудиторию
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithChequeMessage("С любовью. ваш МегаМакс") // Устанавливаем сообщение на чек
                .Build();

            offer.Offers[0].Description =
                "Суммируемая акция, предоставляющая определенные сообщения для конкретной ЦА"; // Добавляем описание для акции

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
