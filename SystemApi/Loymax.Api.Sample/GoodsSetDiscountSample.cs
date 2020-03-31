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
            var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

            var offer = OfferImportBuilder.Create("Sapmle4. N+M") // Установливаем название акции
                .WithPartner(partnerId) // Указываем id партнёра
                .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                .WithChanges(DateTime.Now, OfferChangesState.Approved) // Применяется с установленной даты
                .WithPriority(35) // Устанавливаем приоритет акции
                .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                .WithGoodsSetDivide( // Устанавливаем действие " Разбиение на наборы"
                    new List<GoodsSetItemDto>
                    {
                            new GoodsSetItemDto // Заводим новый набор
                            {
                                GoodsGroup = new GoodsGroupExDto // Источник товаров для набора является найденная группа товаров
                                {
                                    Id = goodsGroup.Data.First().ExternalId.Value.ToString(), // Устанавливаем Id группы товаров 
                                },
                                Value = 6.000, // Задаем колиество товаров в наборе
                            },
                    },
                    ChequeGoodsSortType.PriceAscending, // Устанавливаем тип сортировки товаров: По возрастанию цены
                    ChequeGoodsSetType.Quantity) // Устанавливаем условие разбиения: Количество товара
                .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                    type: ChequeGoodsFilterType.GoodsQuantity, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Количество
                    goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }, // Задаем найденную ранее группу товаров
                    firstValue: 1.0, // Фильтр сработает только в том случае, если количество единиц товара в чеке меньше или равно 1.0
                    @operator: ComparisonOperator.LessOrEqual,
                    chequeGoodsSortType: ChequeGoodsSortType.PriceAscending,
                    dividePositions: true)
                .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                    dicsountType: ActionDiscountType.Percent, // Задаем способ расчета: Процент
                    value: 100) // Устанавливаем размер скидки
                .Build();

            offer.Offers[0].Description =
                "При покупке 5ти товаров, 6ой в подарок."; // Добавляем описание для акции

            var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
