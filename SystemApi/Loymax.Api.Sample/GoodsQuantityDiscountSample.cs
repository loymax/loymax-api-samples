using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class GoodsQuantityDiscountSample
    {
        public static async Task ImportOfferAsync(Client client, string partnerId)
        {
            var goodsGroup =
                await client.GoodsGroup_GetGoodsGroupsAsync(
                    filter_name: "Товары"); // Находим группу товаров с именем "Товары", которая должна быть предварительно создана в разделе "Группы товаров"

            var offer = OfferImportBuilder
                .Create("Sample5. Спец.цена при покупке трех единиц товара") // Установливаем название акции
                .WithDescription("При покупке по цене трех единиц акционного товара в рамках одного чека Участник приобретает эти три товара по фиксированной цене N.") // Добавляем описание для акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithPriority(40) // Устанавливаем приоритет акции
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType
                        .GoodsQuantity, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Количество
                    goodsGroupsIds: new List<Guid>
                        {goodsGroup.Data.First().ExternalId.Value}, // Задаем найденную ранее группу товаров
                    firstValue: 3.0, // Фильтр сработает только в том случае, если количество единиц товара в чеке равно 3.0
                    @operator: ComparisonOperator.Equals)
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType
                        .PricePerUnit, // Задаем способ расчета: Фиксированная цена на единицу товара
                    value: 100.0) // Устанавливаем размер скидки
                .Build();

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
