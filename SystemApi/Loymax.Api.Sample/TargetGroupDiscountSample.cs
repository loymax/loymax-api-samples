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
            var offer = OfferImportBuilder.Create("Sample2. Скидка для ЦА") // Установливаем название акции
                .WithDescription("Скидка для ЦА, в определенном магазине, на конечный срок действия.") // Добавляем описание для акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithExpirationDate(DateTime.Now.AddDays(7)) // Устанавливаем дату окончания действия акции
                .WithPriority(25) // Устанавливаем приоритет акции
                .WithMerchantFilter(new string[] { "75e67dae-130a-4030-192d-18b02d907950", "55403257-1663-4bd1-9725-20b3bfe51392" }) // Задаем GUID-ы магазинов, в которых будет действовать акция
                .WithTargetGroup(new string[] { "1", "2" }) // Задаем идентификаторы целевых аудиторий, которые должны быть предварительно созданы в разделе "Целевые аудитории"
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType.Percent, // Задаем способ расчета: Процент
                    value: 15.0, // Устанавливаем размер скидки
                    calculationExclusionTypes: new List<CalculationExclusionDiscountType> { CalculationExclusionDiscountType.Discount }) // Исключаем из расчета сумму: Прямая скидка
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
