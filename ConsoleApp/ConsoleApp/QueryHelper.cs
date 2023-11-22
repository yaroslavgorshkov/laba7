using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp
{
    public class QueryHelper : IQueryHelper
    {
        // Завдання 1
        public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries) => deliveries.Where(d => !string.IsNullOrEmpty(d.PaymentId));

        // Завдання 2
        public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries) => deliveries.Where(d => d.Status != DeliveryStatus.Cancelled && d.Status != DeliveryStatus.Done);

        // Завдання 3
        public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId) =>
            deliveries.Where(d => d.ClientId == clientId)
                      .Select(d => new DeliveryShortInfo
                      {
                          Id = d.Id,
                          StartCity = d.Direction.Origin.City,
                          EndCity = d.Direction.Destination.City,
                          ClientId = d.ClientId,
                          Type = d.Type,
                          LoadingPeriod = d.LoadingPeriod,
                          ArrivalPeriod = d.ArrivalPeriod,
                          Status = d.Status,
                          CargoType = d.CargoType
                      });

        // Завдання 4
        public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type) =>
            deliveries
                .Where(d => d.Direction.Origin.City == cityName && d.Type == type)
                /*.Take(10)*/;

        // Завдання 5
        public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries) =>
            deliveries.OrderBy(d => d.Status).ThenBy(d => d.LoadingPeriod.Start);

        // Завдання 6
        public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries) =>
            deliveries.Select(d => d.CargoType).Distinct().Count();

        // Завдання 7
        public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries) =>
            deliveries.GroupBy(d => d.Status)
                      .ToDictionary(g => g.Key, g => g.Count());

        // Завдання 8
        public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries) =>
            deliveries.GroupBy(d => new { StartCity = d.Direction.Origin.City, EndCity = d.Direction.Destination.City })
                .Select(group => new AverageGapsInfo
                {
                    StartCity = group.Key.StartCity,
                    EndCity = group.Key.EndCity,
                    AverageGap = group.Average(delivery => (delivery.ArrivalPeriod.Start.Value - delivery.LoadingPeriod.End.Value).Minutes)
                });

        // Завдання 9
        public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
            Func<TElement, TOrderingKey> ordering,
            Func<TElement, bool>? filter = null,
            int countOnPage = 100,
            int pageNumber = 1)
        {
            var query = elements;

            if (filter != null)
                query = query.Where(filter);

            return query.OrderBy(ordering)
                        .Skip((pageNumber - 1) * countOnPage)
                        .Take(countOnPage);
        }
    }
}
