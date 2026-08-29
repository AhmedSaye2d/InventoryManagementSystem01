using Application.DTO.Response;
using Application.Services.Product.Commands.Locations;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public UpdateLocationHandler(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServicesResponse> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id.Equals(request.LocationModel.Id), cancellationToken);
                if (location == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.LocationModel.Name);

                }
                var data = request.LocationModel.Adapt(new Domain.Entities.Product.Location());

                _context.Locations.Update(data);

                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemUpdated(request.LocationModel.Name);
            }
            catch (Exception ex) 
            {
                return new ServicesResponse(true,ex.Message);
            }
        }
    }
}
