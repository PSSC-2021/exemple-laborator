using Laborator1.Domain;
using System;
using System.Collections.Generic;
using static Laborator1.Domain.ShoppingCarts;

namespace Laborator1
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var listOfShoppingCarts = ReadListOfShoppingCarts().ToArray();
            EmptyShoppingCarts emptyShoppingCarts = new(listOfShoppingCarts);
            IShoppingCarts result = ValidateShoppingCarts(emptyShoppingCarts);
            result.Match(
                whenEmptyShoppingCarts: emptyResult => emptyShoppingCarts,
                whenUnvalidatedShoppingCarts: unvalidatedResult => unvalidatedResult,
                whenPaidShoppingCarts: paidResult => paidResult,
                whenValidatedShoppingCarts: validatedResult => PayShoppingCart(validatedResult)
            );
        }

        private static List<EmptyShoppingCart> ReadListOfShoppingCarts()
        {
            List<EmptyShoppingCart> listOfShoppingCarts = new();
            do
            {
                var quantity = ReadValue("Cantitatea produsului comandat: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                var product_code = ReadValue("Codul produsului: ");
                if (string.IsNullOrEmpty(product_code))
                {
                    break;
                }

                var address = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }

                listOfShoppingCarts.Add(new(quantity, product_code, address));
            } while (true);
            return listOfShoppingCarts;
        }

        private static IShoppingCarts ValidateShoppingCarts(EmptyShoppingCarts emptyShoppingCarts) =>
            random.Next(100) > 50 ?
            new UnvalidatedShoppingCarts(new List<UnvalidatedShoppingCart>(), "Random errror")
            : new ValidatedShoppingCarts(new List<ValidatedShoppingCart>());

        private static IShoppingCarts PayShoppingCart(ValidatedShoppingCarts validExamGrades) =>
            new PaidShoppingCarts(new List<ValidatedShoppingCart>(), DateTime.Now);

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
