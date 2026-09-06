using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Categories;
using Mapster;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.Categorymodel.Id);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                request.Categorymodel.Adapt(category);

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemUpdated(request.Categorymodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
