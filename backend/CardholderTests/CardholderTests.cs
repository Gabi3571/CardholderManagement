using CardholderApi.Controllers;
using CardholderApi.DTOs;
using CardholderApi.Models;
using CardholderApi.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CardholderTests
{
    [TestClass]
    public class CardholdersControllerTests
    {
        private Mock<ICardholderRepository> _repositoryMock = new();
        private Mock<ILogger<CardholdersController>> _loggerMock = new();
        private CardholdersController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<ICardholderRepository>();
            _loggerMock = new Mock<ILogger<CardholdersController>>();

            _controller = new CardholdersController(_repositoryMock.Object, _loggerMock.Object);
        }

        #region GET

        [TestMethod]
        public async Task GetCardholders_ShouldReturnPagedResult()
        {
            var pagedResult = new PagedResult<CardholderDTO>
            {
                Items = [new() { Id = Guid.NewGuid() }],
                TotalCount = 1
            };
            _repositoryMock.Setup(r => r.GetPagedAsync(1, 10, "desc"))
                           .ReturnsAsync(pagedResult);

            var result = await _controller.GetCardholders();
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as PagedResult<CardholderDTO>;
            Assert.IsNotNull(returned, "Expected PagedResult<CardholderDTO>");
            Assert.AreEqual(1, returned.TotalCount);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCardholder_WhenFound()
        {
            var id = Guid.NewGuid();
            var cardholder = new CardholderDTO { Id = id };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(cardholder);

            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returned = okResult.Value as CardholderDTO;
            Assert.IsNotNull(returned, "Expected CardholderDTO");
            Assert.AreEqual(id, returned.Id);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _ = _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CardholderDTO)null);

            var result = await _controller.GetById(id);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        #endregion

        #region CREATE

        [TestMethod]
        public async Task Create_ShouldReturnCreatedCardholder()
        {
            var dto = new CreateCardholderDTO { FirstName = "Kate", LastName = "Katić", Address = "Katina adresa", PhoneNumber = "+38763214678", TransactionCount = 3 };
            var created = new CardholderDTO { Id = Guid.NewGuid() };
            _repositoryMock.Setup(r => r.AddAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            
            var returned = createdResult.Value as CardholderDTO;
            Assert.IsNotNull(returned, "Expected CardholderDTO");
            Assert.AreEqual(created.Id, returned.Id);
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_OnValidationException()
        {
            var dto = new CreateCardholderDTO { FirstName = "Ante", LastName = "Antić"};
            _repositoryMock.Setup(r => r.AddAsync(dto))
                           .ThrowsAsync(new ValidationException("Validation failed"));

            var result = await _controller.Create(dto);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Create_ShouldReturnInternalServerError_OnException()
        {
            var dto = new CreateCardholderDTO();
            _repositoryMock.Setup(r => r.AddAsync(dto))
                           .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Create(dto);
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        #endregion

        #region UPDATE

        [TestMethod]
        public async Task Update_ShouldModifyAllFieldsOfExistingCardholder()
        {
            var existingId = Guid.NewGuid();

            var cardholders = new List<CardholderDTO>
            {
                new() {
                    Id = existingId,
                    FirstName = "Matija",
                    LastName = "Matijić",
                    Address = "Matijina adresa",
                    PhoneNumber = "+3856214597",
                    TransactionCount = 1
                }
            };

            // Mock repository s refleksijom za automatski update svih polja
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateCardholderDTO>()))
                .Returns<Guid, UpdateCardholderDTO>((id, dto) =>
                {
                    var entity = cardholders.FirstOrDefault(c => c.Id == id)
                                 ?? throw new KeyNotFoundException();

                    var dtoProps = typeof(UpdateCardholderDTO).GetProperties();
                    var entityProps = typeof(CardholderDTO).GetProperties();

                    foreach (var prop in dtoProps)
                    {
                        var entityProp = entityProps.FirstOrDefault(p => p.Name == prop.Name);
                        if (entityProp != null && entityProp.CanWrite)
                        {
                            var value = prop.GetValue(dto);
                            entityProp.SetValue(entity, value);
                        }
                    }

                    return Task.CompletedTask;
                });

            var updateDto = new UpdateCardholderDTO
            {
                FirstName = "Matija",
                LastName = "Matijić",
                Address = "Matijina nova adresa",
                PhoneNumber = "+387649876",
                TransactionCount = 2
            };

            var result = await _controller.Update(existingId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var updated = cardholders.First(c => c.Id == existingId);
            var dtoPropsToCheck = typeof(UpdateCardholderDTO).GetProperties();

            foreach (var prop in dtoPropsToCheck)
            {
                var entityProp = typeof(CardholderDTO).GetProperty(prop.Name);
                Assert.AreEqual(prop.GetValue(updateDto), entityProp.GetValue(updated),
                    $"Property {prop.Name} was not updated correctly");
            }
        }


        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenValidationFails()
        {
            var existingId = Guid.NewGuid();
            var cardholders = new List<CardholderDTO>
            {
                new() {
                    Id = existingId,
                    FirstName = "Ante",
                    LastName = "Antić",
                    Address = "Antina adresa",
                    PhoneNumber = "+3876543234",
                    TransactionCount = 7
                }
            };

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateCardholderDTO>()))
                .Returns<Guid, UpdateCardholderDTO>((id, dto) =>
                {
                    var entity = cardholders.FirstOrDefault(c => c.Id == id)
                                 ?? throw new KeyNotFoundException();

                    // simuliramo validator: LastName ne smije biti null
                    if (string.IsNullOrWhiteSpace(dto.LastName))
                        throw new ValidationException("LastName is required");

                    var dtoProps = typeof(UpdateCardholderDTO).GetProperties();
                    var entityProps = typeof(CardholderDTO).GetProperties();
                    foreach (var prop in dtoProps)
                    {
                        var entityProp = entityProps.FirstOrDefault(p => p.Name == prop.Name);
                        if (entityProp != null && entityProp.CanWrite)
                            entityProp.SetValue(entity, prop.GetValue(dto));
                    }

                    return Task.CompletedTask;
                });

            var invalidDto = new UpdateCardholderDTO
            {
                FirstName = "Matija",
                Address = "Nova adresa",
                PhoneNumber = "+387649876",
                TransactionCount = 2
            };

            var result = await _controller.Update(existingId, invalidDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenEntityDoesNotExist()
        {
            var nonExistingId = Guid.NewGuid();
            var cardholders = new List<CardholderDTO>();

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateCardholderDTO>()))
                .Returns<Guid, UpdateCardholderDTO>((id, dto) =>
                {
                    var entity = cardholders.FirstOrDefault(c => c.Id == id)
                                 ?? throw new KeyNotFoundException($"Cardholder with id {id} not found");
                    return Task.CompletedTask;
                });

            var dto = new UpdateCardholderDTO
            {
                FirstName = "Jasna",
                LastName = "Jasnić",
                Address = "Jasnina adresa",
                PhoneNumber = "+486987245",
                TransactionCount = 5
            };

            var result = await _controller.Update(nonExistingId, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }


        [TestMethod]
        public async Task Update_ShouldReturnInternalServerError_OnException()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateCardholderDTO();
            _repositoryMock.Setup(r => r.UpdateAsync(id, dto))
                           .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Update(id, dto);
            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        #endregion

        #region DELETE

        [TestMethod]
        public async Task Delete_ShouldRemoveCardholder_WhenSuccessful()
        {
            var existingId = Guid.NewGuid();
            var cardholders = new List<CardholderDTO>
            {
                new() { Id = existingId, FirstName = "Sanja", LastName = "Sanjić", Address = "Sanjina adresa", PhoneNumber = "+368423352", TransactionCount = 16 }
            };

            _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
                .Returns<Guid>(id =>
                {
                    var entity = cardholders.FirstOrDefault(c => c.Id == id)
                                 ?? throw new KeyNotFoundException();
                    cardholders.Remove(entity);
                    return Task.CompletedTask;
                });

            var result = await _controller.Delete(existingId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.IsFalse(cardholders.Any(c => c.Id == existingId), "Cardholder was not removed");
        }


        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_OnKeyNotFoundException()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id))
                           .ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.Delete(id);
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnInternalServerError_OnException()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id))
                           .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Delete(id);
            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }

        #endregion
    }
}
