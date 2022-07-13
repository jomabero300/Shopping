using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TSShopping.Common;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Enum;
using TSShopping.Models;

namespace TSShopping.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly ApplicationDbContext _context;

        public OrdersHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CancelOrderAsync(int id)
        {
            Sale sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            foreach (SaleDetail saleDetail in sale.SaleDetails)
            {
                Product product = await _context.Products.FindAsync(saleDetail.Product.Id);
                if (product != null)
                {
                    product.Stock += saleDetail.Quantity;
                }
            }

            sale.OrderStatus = OrderStatus.Cancelado;
            await _context.SaveChangesAsync();
            return new Response { Succeeded = true };
        }

        public async Task<Response> ProcessOrderAsync(ShowCartViewModel model)
        {
            Response response = await CheckInventoryAsync(model);
            if (!response.Succeeded)
            {
                return response;
            }

            Sale sale = new()
            {
                Date = DateTime.UtcNow,
                User = model.User,
                Remarks = model.Remarks,
                SaleDetails = new List<Data.Entities.SaleDetail>(),
                OrderStatus = OrderStatus.Nuevo
            };

            foreach (TemporalSale item in model.TemporalSales)
            {
                sale.SaleDetails.Add(new SaleDetail
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });

                Product product = await _context.Products.FindAsync(item.Product.Id);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);
                }

                _context.TemporalSales.Remove(item);
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return response;
        }
        private async Task<Response> CheckInventoryAsync(ShowCartViewModel model)
        {
            Response response = new() { Succeeded = true };
            foreach (TemporalSale item in model.TemporalSales)
            {
                Product product = await _context.Products.FindAsync(item.Product.Id);
                if (product == null)
                {
                    response.Succeeded = false;
                    response.Message = $"El producto {item.Product.Name}, ya no está disponible";
                    return response;
                }
                if (product.Stock < item.Quantity)
                {
                    response.Succeeded = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto {item.Product.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.";
                    return response;
                }
            }
            return response;
        }

    }
}