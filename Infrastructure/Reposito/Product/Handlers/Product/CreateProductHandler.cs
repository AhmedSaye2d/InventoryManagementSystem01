using Application.DTO.Response;
using Application.Services.Product.Commands.Products;
using Product = Domain.Entities.Product.Product;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Products
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public CreateProductHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == request.productmodel.Name, cancellationToken);
                if (product != null)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.productmodel.Name);
                }
                var data = request.productmodel.Adapt(new Domain.Entities.Product.Product());

                _context.Products.Add(data);

                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemCreated(request.productmodel.Name);

            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
