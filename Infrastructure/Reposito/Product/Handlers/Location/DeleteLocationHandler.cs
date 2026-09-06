using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Locations;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLocationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(request.Id);

                if (location == null)
                {
                    return GeneralDbResponses.ItemNotFound("location");
                }

                _unitOfWork.Locations.Delete(location);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDeleted(location.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
