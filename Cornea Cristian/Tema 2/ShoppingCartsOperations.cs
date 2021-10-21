using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSSC.Domain.ShoppingCarts;

namespace Laborator2_PSSC.Domain
{
    public static class ShoppingCartsOperations
    {
        public static IShoppingCarts ValidateShoppingCarts(Func<ProductCode, bool> checkProductExists, EmptyShoppingCarts shoppingCarts)
        {
            List<ValidatedShoppingCart> validatedShoppingCarts = new();
            bool isValidList = true;
            string invalidReson = string.Empty;
            foreach (var emptyShoppingCart in shoppingCarts.ShoppingCartList)
            {
                if (!ProductCode.TryParse(emptyShoppingCart.productCode, out ProductCode productCode))
                {
                    invalidReson = $"Invalid product code ({emptyShoppingCart.productCode})";
                    isValidList = false;
                    break;
                }
                if (!Quantity.TryParse(emptyShoppingCart.quantity, out Quantity quantity))
                {
                    invalidReson = $"Invalid quantity ({emptyShoppingCart.productCode}, {emptyShoppingCart.quantity})";
                    isValidList = false;
                    break;
                }
                if (!Address.TryParse(emptyShoppingCart.address, out Address address))
                {
                    invalidReson = $"Invalid address ({emptyShoppingCart.productCode}, {emptyShoppingCart.address})";
                    isValidList = false;
                    break;
                }
                if (!Price.TryParse(emptyShoppingCart.price, out Price price))
                {
                    invalidReson = $"Invalid price ({emptyShoppingCart.productCode}, {emptyShoppingCart.price})";
                    isValidList = false;
                    break;
                }
                ValidatedShoppingCart validShoppingCart = new(productCode, quantity, address, price);
                validatedShoppingCarts.Add(validShoppingCart);
            }

            if (isValidList)
            {
                return new ValidatedShoppingCarts(validatedShoppingCarts);
            }
            else
            {
                return new UnvalidatedShoppingCarts(shoppingCarts.ShoppingCartList, invalidReson);
            }

        }

        public static IShoppingCarts CalculateFinalPrice(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenCalculatedShoppingCarts: calculatedShoppingCart => calculatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: validShoppingCarts =>
            {
                var calculatedGrade = validShoppingCarts.ShoppingCartList.Select(validShoppingCart =>
                                            new CalculatedShoppingCart(validShoppingCart.productCode,
                                                                      validShoppingCart.quantity,
                                                                      validShoppingCart.address,
                                                                      validShoppingCart.price,
                                                                      validShoppingCart.price * validShoppingCart.quantity));
                return new CalculatedShoppingCarts(calculatedGrade.ToList().AsReadOnly());
            }
        );

        public static IShoppingCarts PayShoppingCart(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: validShoppingCarts => validShoppingCarts,
            whenCalculatedShoppingCarts: calculatedShoppingCart =>
            {
                StringBuilder csv = new();
                calculatedShoppingCart.ShoppingCartList.Aggregate(csv, (export, shoppingCart) => export.AppendLine($"{shoppingCart.productCode}, {shoppingCart.quantity}, {shoppingCart.address}, , {shoppingCart.finalPrice}"));

                PaidShoppingCarts paidShoppingCarts = new(calculatedShoppingCart.ShoppingCartList, csv.ToString(), DateTime.Now);

                return paidShoppingCarts;
            });

    }
}
