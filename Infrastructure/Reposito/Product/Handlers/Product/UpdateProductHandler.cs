using Application.DTO.Response;
using Application.Services.Product.Commands.Products;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public UpdateProductHandler(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServicesResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.productmodel.Id, cancellationToken: cancellationToken);

                if (product == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.productmodel.Name);
                }
                var adapt = request.productmodel.Adapt(new Domain.Entities.Product.Product());

                _context.Products.Update(adapt);

                await _context.SaveChangesAsync();

                return GeneralDbResponses.ItemUpdated(request.productmodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(true, ex.Message);
            }
        }
    }
}