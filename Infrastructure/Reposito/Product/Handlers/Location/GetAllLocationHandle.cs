using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Locations;
using Mapster;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class GetAllLocationHandle : IRequestHandler<GetAllLocationQuery, IEnumerable<GetLocationResponseDto>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetAllLocationHandle(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<GetLocationResponseDto>> Handle(GetAllLocationQuery request, CancellationToken cancellationToken)
        {
            var data = await _locationRepository.GetAllAsync();
            return data.Adapt<List<GetLocationResponseDto>>();
        }
    }
}
