using Application.DTO.Response;
using Application.Services.Product.Commands.Locations;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public DeleteLocationHandler(AppDbContext context)
        {
            this._context = context;    
        }
        public async Task<ServicesResponse> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken: cancellationToken);

                if (location == null)
                {
                    return GeneralDbResponses.ItemNotFound("location");
                }
                _context.Locations.Remove(location);

                await _context.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDeleted(location.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(true, ex.Message);
            }
        }
    }
}
