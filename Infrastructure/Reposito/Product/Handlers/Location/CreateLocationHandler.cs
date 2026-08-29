using Application.DTO.Response;
using Application.Services.Product.Commands.Locations;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public CreateLocationHandler(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServicesResponse> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _context.Locations.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(request.locationmodel.Name), cancellationToken: cancellationToken);

                if (location != null)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.locationmodel.Name);
                }
                var data = request.locationmodel.Adapt(new Domain.Entities.Product.Location());

                _context.Locations.Add(data);   

               await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemCreated(request.locationmodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(true,ex.Message);
            }
        }
    }
}
