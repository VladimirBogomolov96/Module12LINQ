using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(customer => customer.Orders.Sum(order => order.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select(customer => (customer, suppliers.Where(supplier => supplier.Country == customer.Country && supplier.City == customer.City)));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.GroupBy(customer => customer).Select(group => (group.Key, suppliers.Where(supplier => group.Key.Country == supplier.Country && group.Key.City == supplier.City)));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(customer => customer.Orders.Length !=0 && customer.Orders.Sum(order => order.Total) > limit);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers.Where(customer => customer.Orders.Length > 0).Select(customer => (customer, customer.Orders.Min(order => order.OrderDate)));                
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return customers.Where(customer => customer.Orders.Length > 0)
                .Select(customer => (customer, customer.Orders.Min(order => order.OrderDate)))
                .OrderBy(tuple => tuple.Item2.Year)
                .ThenBy(tuple => tuple.Item2.Month)
                .ThenBy(tuple => tuple.Item2.Day)
                .ThenByDescending(tuple => tuple.Item1.Orders.Sum(order => order.Total))
                .ThenBy(tuple => tuple.Item1.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return customers.Where(customer => customer.Region is null || !customer.Phone.Contains('(') || Regex.IsMatch(customer.PostalCode, @"\D"));
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            return products.GroupBy(product => product.Category)
                .Select(group => new Linq7CategoryGroup
                {
                    Category = group.Key,
                    UnitsInStockGroup = group.GroupBy(product => product.UnitsInStock)
                     .Select(innerGroup => new Linq7UnitsInStockGroup
                     {
                         UnitsInStock = innerGroup.Key,
                         Prices = products.Where(product => product.Category == group.Key && product.UnitsInStock == innerGroup.Key).Select(product => product.UnitPrice).OrderBy(price => price)
                     })
                });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products.GroupBy(product => product.GetCategory(cheap, middle, expensive)).Select(group => (group.Key, products.Where(product => product.GetCategory(cheap, middle, expensive) == group.Key)));
        }

        public static decimal GetCategory(this Product product, decimal cheap, decimal middle, decimal expensive)
        {
            decimal category = -1;
            if (product.UnitPrice <= cheap && product.UnitPrice >= 0)
            {
                category = cheap;
            }
            else if (product.UnitPrice > cheap && product.UnitPrice <= middle)
            {
                category = middle;
            }
            else if (product.UnitPrice > middle && product.UnitPrice <= expensive)
            {
                category = expensive;
            }

            return category;
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            IEnumerable<string> cities = customers.Select(customer => customer.City).Distinct();
            var result = cities.Select(city => (city, Convert.ToInt32(customers.Where(customer => customer.City == city).Select(customer => customer.Orders).Average(orders => orders.Sum(order => order.Total))), Convert.ToInt32(customers.Where(customer => customer.City == city).Select(customer => customer.Orders.Length).Average())));
            return result;
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            return string.Join(string.Empty, suppliers.Select(supplier => supplier.Country).Distinct().OrderBy(country => country.Length).ThenBy(country => country));
        }
    }
}