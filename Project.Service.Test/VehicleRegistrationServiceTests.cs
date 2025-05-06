using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Sorting;
using Project.Common.Filtering;
using Project.Model.Common;
using Project.Repository.Common;
using Project.Service.Common;
using Project.Service;
using Xunit;

namespace Project.Service.Test
{
    public class VehicleRegistrationServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehicleRegistrationRepository> _mockRepository;
        private readonly Mock<IVehicleModelRepository> _mockModelRepository;
        private readonly Mock<IVehicleOwnerRepository> _mockOwnerRepository;
        private readonly Mock<IVehicleEngineTypeRepository> _mockEngineTypeRepository;
        private readonly VehicleRegistrationService _service;

        public VehicleRegistrationServiceTests()
        {
            _mockRepository = new Mock<IVehicleRegistrationRepository>();
            _mockModelRepository = new Mock<IVehicleModelRepository>();
            _mockOwnerRepository = new Mock<IVehicleOwnerRepository>();
            _mockEngineTypeRepository = new Mock<IVehicleEngineTypeRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            _mockUnitOfWork.Setup(u => u.VehicleRegistrationRepository).Returns(_mockRepository.Object);
            _mockUnitOfWork.Setup(u => u.VehicleModelRepository).Returns(_mockModelRepository.Object);
            _mockUnitOfWork.Setup(u => u.VehicleOwnerRepository).Returns(_mockOwnerRepository.Object);
            _mockUnitOfWork.Setup(u => u.VehicleEngineTypeRepository).Returns(_mockEngineTypeRepository.Object);
            
            _service = new VehicleRegistrationService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllRegistrations_ShouldReturnAllRegistrations()
        {
            // Arrange
            var registrationsList = new List<IVehicleRegistration>
            {
                Mock.Of<IVehicleRegistration>(r => r.Id == 1 && r.RegistrationNumber == "ZG-123-AB"),
                Mock.Of<IVehicleRegistration>(r => r.Id == 2 && r.RegistrationNumber == "ZG-456-CD"),
                Mock.Of<IVehicleRegistration>(r => r.Id == 3 && r.RegistrationNumber == "ZG-789-EF")
            };

            var pagedResult = new PagedResult<IVehicleRegistration>(registrationsList, 3, 1, int.MaxValue);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllRegistrations(new QueryOptions());

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().BeEquivalentTo(registrationsList);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedRegistrations_WithValidOptions_ShouldReturnPagedResult()
        {
            // Arrange
            var registrationsList = new List<IVehicleRegistration>
            {
                Mock.Of<IVehicleRegistration>(r => r.Id == 1 && r.RegistrationNumber == "ZG-123-AB"),
                Mock.Of<IVehicleRegistration>(r => r.Id == 2 && r.RegistrationNumber == "ZG-456-CD")
            };

            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = 2 },
                Sorting = new SortOptions { SortBy = "RegistrationNumber", SortAscending = true },
                Filtering = new FilteringOptions { SearchText = "ZG" }
            };

            var pagedResult = new PagedResult<IVehicleRegistration>(registrationsList, 2, 1, 2);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetPagedRegistrations(queryOptions);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(2);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetRegistrationById_WithValidId_ShouldReturnRegistration()
        {
            // Arrange
            var registration = Mock.Of<IVehicleRegistration>(r => 
                r.Id == 1 && 
                r.RegistrationNumber == "ZG-123-AB" && 
                r.ModelId == 1 && 
                r.OwnerId == 1 &&
                r.ModelEngineTypeId == 1);

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(registration);

            // Act
            var result = await _service.GetRegistrationById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.RegistrationNumber.Should().Be("ZG-123-AB");
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetRegistrationById_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((IVehicleRegistration)null);

            // Act & Assert
            await FluentActions.Invoking(() => _service.GetRegistrationById(999))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Registracija vozila s ID-om 999 nije pronađena.");
        }

        [Fact]
        public async Task AddRegistration_WithValidRegistration_ShouldAddAndReturnRegistration()
        {
            // Arrange
            var model = Mock.Of<IVehicleModel>(m => m.Id == 1 && m.Name == "Golf");
            var owner = Mock.Of<IVehicleOwner>(o => o.Id == 1 && o.FirstName == "Pero" && o.LastName == "Perić");
            var engineType = Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel");
            
            var registration = Mock.Of<IVehicleRegistration>(r => 
                r.RegistrationNumber == "ZG-123-AB" && 
                r.ModelId == 1 && 
                r.OwnerId == 1 &&
                r.ModelEngineTypeId == 1);

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync(false);
                
            _mockModelRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(model);
                
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(owner);
                
            _mockEngineTypeRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(engineType);

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<IVehicleRegistration>()))
                .ReturnsAsync(Mock.Of<IVehicleRegistration>(r => 
                    r.Id == 1 && 
                    r.RegistrationNumber == "ZG-123-AB" && 
                    r.ModelId == 1 && 
                    r.ModelName == "Golf" &&
                    r.OwnerId == 1 && 
                    r.OwnerName == "Pero Perić" &&
                    r.ModelEngineTypeId == 1));
                
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddRegistration(registration);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.RegistrationNumber.Should().Be("ZG-123-AB");
            result.ModelName.Should().Be("Golf");
            result.OwnerName.Should().Be("Pero Perić");
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<IVehicleRegistration>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddRegistration_WithExistingRegistrationNumber_ShouldThrowException()
        {
            // Arrange
            var registration = Mock.Of<IVehicleRegistration>(r => r.RegistrationNumber == "ZG-123-AB");

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            await FluentActions.Invoking(() => _service.AddRegistration(registration))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Registracija s brojem 'ZG-123-AB' već postoji");

            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<IVehicleRegistration>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddRegistration_WithInvalidModelId_ShouldThrowException()
        {
            // Arrange
            var registration = Mock.Of<IVehicleRegistration>(r => 
                r.RegistrationNumber == "ZG-123-AB" && 
                r.ModelId == 999);

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync(false);
                
            _mockModelRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((IVehicleModel)null);

            // Act & Assert
            await FluentActions.Invoking(() => _service.AddRegistration(registration))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Model vozila s ID-om 999 ne postoji*");

            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<IVehicleRegistration>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateRegistration_WithValidRegistration_ShouldUpdateAndReturnRegistration()
        {
            // Arrange
            var model = Mock.Of<IVehicleModel>(m => m.Id == 1 && m.Name == "Golf");
            var owner = Mock.Of<IVehicleOwner>(o => o.Id == 1 && o.FirstName == "Pero" && o.LastName == "Perić");
            var engineType = Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel");
            
            var registration = Mock.Of<IVehicleRegistration>(r => 
                r.Id == 1 &&
                r.RegistrationNumber == "ZG-123-AB" && 
                r.ModelId == 1 && 
                r.OwnerId == 1 &&
                r.ModelEngineTypeId == 1);

            _mockRepository.Setup(repo => repo.GetFirstAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync((IVehicleRegistration)null);
                
            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(registration);
                
            _mockModelRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(model);
                
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(owner);
                
            _mockEngineTypeRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(engineType);

            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<IVehicleRegistration>()))
                .ReturnsAsync(Mock.Of<IVehicleRegistration>(r => 
                    r.Id == 1 && 
                    r.RegistrationNumber == "ZG-123-AB" && 
                    r.ModelId == 1 && 
                    r.ModelName == "Golf" &&
                    r.OwnerId == 1 && 
                    r.OwnerName == "Pero Perić" &&
                    r.ModelEngineTypeId == 1));
                
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateRegistration(registration);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.RegistrationNumber.Should().Be("ZG-123-AB");
            result.ModelName.Should().Be("Golf");
            result.OwnerName.Should().Be("Pero Perić");
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<IVehicleRegistration>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteRegistration_WithValidId_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(true);
                
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteRegistration(1);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteRegistration_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteRegistration(999);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.DeleteAsync(999), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegistrationExistsByNumber_WhenExists_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.RegistrationExistsByNumber("ZG-123-AB");

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RegistrationExistsByNumber_WhenNotExists_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.RegistrationExistsByNumber("ZG-999-ZZ");

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RegistrationExistsByNumber_WithEmptyNumber_ShouldReturnFalse()
        {
            // Act
            var result = await _service.RegistrationExistsByNumber("");

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleRegistration, bool>>>()), Times.Never);
        }
    }
} 