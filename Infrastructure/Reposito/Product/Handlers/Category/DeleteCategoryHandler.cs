using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Categories;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDeleted(category.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
