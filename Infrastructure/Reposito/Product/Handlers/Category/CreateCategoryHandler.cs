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
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public CreateCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Name.ToLower().Equals(request.Categorymodel.Name.ToLower()), cancellationToken: cancellationToken);

                if (category != null)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.Categorymodel.Name);
                }

                var data = request.Categorymodel.Adapt<Domain.Entities.Product.Category>();

                _context.Categories.Add(data);
                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemCreated(request.Categorymodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
