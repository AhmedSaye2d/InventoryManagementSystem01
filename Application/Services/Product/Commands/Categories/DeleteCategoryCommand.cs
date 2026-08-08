using Application.DTO.Response;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Product.Commands.Categories
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<ServicesResponse>;
    
}
