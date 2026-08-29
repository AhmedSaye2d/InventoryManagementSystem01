using Application.DTO.Response;
using Application.Services.Product.Commands.Products;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public DeleteProductHandler(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServicesResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken: cancellationToken);
                if (product == null)
                {
                    return GeneralDbResponses.ItemNotFound("Product");
                }
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return GeneralDbResponses.ItemDeleted(product.Name);

            }
            catch (Exception ex)
            {
                return new ServicesResponse(true, ex.Message);
            }
        }
    }
}