using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loymax.SystemApi.SDK;
using Newtonsoft.Json;

namespace Loymax.Api.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client()
            {
                BaseUrl = "http://improve-44322.iis.local/systemapi/"
                ////BaseUrl = "http://localhost/systemapi/"
            };

            // учётные данные запросите у специалистов Loymax
            await client.Authorization("admin", "111111");

            var partnerId = "9957c76a-fe1a-499e-d5ca-1ab605a6a166";

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

            {
                var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

                var offer = OfferImportBuilder.Create("Sample1. Скидка на группу товаров.") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 13, 36, 27), OfferChangesState.Approved) // Применяется с установленной даты
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

            {
                var offer = OfferImportBuilder.Create("Sample2. Скидка для ЦА") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 13, 42, 50), OfferChangesState.Approved) // Применяется с установленной даты
                    .WithExpirationDate(new DateTime(2020, 02, 29) ) // Устанавливаем дату окончания действия акции
                    .WithPriority(25) // Устанавливаем приоритет акции
                    .WithMerchantFilter(new string[] { "75e67dae-130a-4030-192d-18b02d907950", "55403257-1663-4bd1-9725-20b3bfe51392" }) // Задаем GUID-ы магазинов, в которых будет действовать акция
                    .WithTargetGroup(new string[] {"1", "2"}) // Задаем целевые аудитории
                    .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                    .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                        dicsountType: ActionDiscountType.Percent, // Задаем способ расчета: Процент
                        value: 15.0, // Устанавливаем размер скидки
                        calculationExclusionTypes: new List<CalculationExclusionDiscountType> { CalculationExclusionDiscountType.Discount }) // Исключаем из расчета сумму: Прямая скидка
                    .Build();

                offer.Offers[0].Description =
                    "Скидка для ЦА, в определенном магазине, на конечный срок действия."; // Добавляем описание для акции

                var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }

            {
                var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

                var offerBuilder = OfferImportBuilder.Create("Sample3. Фиксированная цена на единицу товара.") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 13, 46, 33), OfferChangesState.Approved) // Применяется с установленной даты
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
                offer.Offers[0].Description =
                    "Несколько цепочек с заданным фиксированном значением."; // Добавляем описание для акции

                var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }

            {
                var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

                var offer = OfferImportBuilder.Create("Sapmle4. N+M") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 13, 52, 34), OfferChangesState.Approved) // Применяется с установленной даты
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

            {
                var goodsGroup = await client.GoodsGroup_GetGoodsGroupsAsync(filter_name: "Товары"); // Находим группу товаров с именем "Товары"

                var offer = OfferImportBuilder.Create("Sample5. Спец.цена при покупке трех единиц товара") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 13, 57, 56), OfferChangesState.Approved) // Применяется с установленной даты
                    .WithPriority(40) // Устанавливаем приоритет акции
                    .AddChain<PurchaseCalculateEventDto>() // Добавляем цепочку для обработки события расчёта скидки
                    .WithChequePositionGoodsFilter( // Устанавливаем фильтр "Товар"
                        type: ChequeGoodsFilterType.GoodsQuantity, // Устанавливаем дополнительное условие, при котором будет срабатывать фильтр: Количество
                        goodsGroupsIds: new List<Guid> { goodsGroup.Data.First().ExternalId.Value }, // Задаем найденную ранее группу товаров
                        firstValue: 3.0, // Фильтр сработает только в том случае, если количество единиц товара в чеке равно 3.0
                        @operator: ComparisonOperator.Equals)
                    .WithDirectDiscount( // Устанавливаем действие "Прямая скидка"
                        dicsountType: ActionDiscountType.PricePerUnit, // Задаем способ расчета: Фиксированная цена на единицу товара
                        value: 100.0) // Устанавливаем размер скидки
                    .Build();

                offer.Offers[0].Description = 
                    "При покупке по цене трех единиц акционного товара в рамках одного чека Участник приобретает эти три товара по фиксированной цене N."; // Добавляем описание для акции

                var result = await client.OfferImportExport_PostOffersAsync(offer); // Импортируем акцию в систему
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }

            {
                var offer = OfferImportBuilder.Create("Sample6. Сообщение на чек определенной ЦА") // Установливаем название акции
                    .WithPartner(partnerId) // Указываем id партнёра
                    .WithState(OfferWorkingState.Running) // Акция будет сразу запущена
                    .WithChanges(new DateTime(2020, 02, 15, 14, 03, 04), OfferChangesState.Approved) // Применяется с установленной даты
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
}
