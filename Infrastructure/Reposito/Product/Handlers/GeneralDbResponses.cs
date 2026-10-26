using Application.DTO.Response;
using System;
namespace Infrastructure.Reposito.Product.Handlers
{
    public class GeneralDbResponses
    {
        public static ServicesResponse ItemAlreadyExist(string itemName)
             => new(false, $"{itemName} already exists");
        public static ServicesResponse ItemNotFound(string itemName)
            => new(false, $"{itemName} not found");
        public static ServicesResponse ItemCreated(string itemName)
             => new(true, $"{itemName} successfully created");
        public static ServicesResponse ItemUpdated(string itemName)
            => new(true, $"{itemName} successfully updated");
            public static ServicesResponse ItemDeleted(string itemName)
                => new(true, $"{itemName} successfully deleted");

    }
}
