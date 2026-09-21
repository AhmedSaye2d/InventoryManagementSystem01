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
        private readonly Application.Interfaces.ICacheService _cacheService;

        public GetAllLocationHandle(ILocationRepository locationRepository, Application.Interfaces.ICacheService cacheService)
        {
            _locationRepository = locationRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<GetLocationResponseDto>> Handle(GetAllLocationQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "locations:all";
            var cachedLocations = await _cacheService.GetAsync<IEnumerable<GetLocationResponseDto>>(cacheKey);
            if (cachedLocations != null)
            {
                return cachedLocations;
            }

            var data = await _locationRepository.GetAllAsync();
            var response = data.Adapt<List<GetLocationResponseDto>>();
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }
    }
}
