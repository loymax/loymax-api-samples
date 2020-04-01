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
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

            var offerBuilder = OfferImportBuilder.Create("Sample3. Фиксированная цена на единицу товара.") // Установливаем название акции
                .WithDescription("Несколько цепочек с заданным фиксированном значением.") // Добавляем описание для акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithPriority(30); // Устанавливаем приоритет акции

            // Добавляем 1-ю цепочку для обработки события расчёта скидки
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType.OnlyGoods, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Отсутствует
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }) // Задаем найденную ранее группу товаров
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType.PricePerUnit, // Задаем способ расчета: Фиксированная цена на единицу товара
                    value: 450.0); // Устанавливаем размер скидки

            // Добавляем 2-ю цепочку для обработки события расчёта скидки
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType.OnlyGoods, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Отсутствует
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}) // Задаем найденную ранее группу товаров
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Задаем способ расчета: Фиксированная цена на единицу товара
                    value: 350.0); // Устанавливаем размер скидки

            // Добавляем 3-ю цепочку для обработки события расчёта скидки
            offerBuilder.AddChain<PurchaseCalculateEventDto>()
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType.OnlyGoods, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Отсутствует
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}) // Задаем найденную ранее группу товаров
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Задаем способ расчета: Фиксированная цена на единицу товара
                    value: 250.0); // Устанавливаем размер скидки

            var offer = offerBuilder.Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
