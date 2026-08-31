using Application.DTO.Response;
using Application.Services.Product.Commands.Categories;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public UpdateCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Id.Equals(request.Categorymodel.Id), cancellationToken);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                request.Categorymodel.Adapt(category);

                _context.Categories.Update(category);
                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemUpdated(request.Categorymodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
