using Application.DTO.Response;
using Application.Services.Product.Commands.Categories;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDeleted(category.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
