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
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

            var offer = OfferImportBuilder.Create("Sample1. Скидка на группу товаров.") // Установливаем название акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithPriority(20) // Устанавливаем приоритет акции
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType.OnlyGoods, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Отсутствует
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }) // Задаем найденную ранее группу товаров
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType.Percent, // Задаем способ расчета: Процент
                    value: 10.0, // Устанавливаем размер скидки
                    calculationExclusionTypes: new List<CalculationExclusionDiscountType> { CalculationExclusionDiscountType.Discount }) // Исключаем из расчета сумму: Прямая скидка
                .Build();

            offer.Offers[0].Description =
                "Скидка 10% на группу товаров с исключающим набором."; // Добавляем описание для акции

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
