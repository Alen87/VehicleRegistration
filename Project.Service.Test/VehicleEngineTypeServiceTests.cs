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
    public class VehicleEngineTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehicleEngineTypeRepository> _mockRepository;
        private readonly VehicleEngineTypeService _service;

        public VehicleEngineTypeServiceTests()
        {
            _mockRepository = new Mock<IVehicleEngineTypeRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.VehicleEngineTypeRepository).Returns(_mockRepository.Object);
            _service = new VehicleEngineTypeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllEngineTypes_ShouldReturnAllEngineTypes()
        {
            // Arrange
            var engineTypesList = new List<IVehicleEngineType>
            {
                Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel" && et.Abrv == "DI"),
                Mock.Of<IVehicleEngineType>(et => et.Id == 2 && et.Type == "Petrol" && et.Abrv == "PE"),
                Mock.Of<IVehicleEngineType>(et => et.Id == 3 && et.Type == "Hybrid" && et.Abrv == "HYB")
            };

            var pagedResult = new PagedResult<IVehicleEngineType>(engineTypesList, 3, 1, int.MaxValue);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllEngineTypes(new QueryOptions());

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().BeEquivalentTo(engineTypesList);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedEngineTypes_WithValidOptions_ShouldReturnPagedResult()
        {
            // Arrange
            var engineTypesList = new List<IVehicleEngineType>
            {
                Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel" && et.Abrv == "DI"),
                Mock.Of<IVehicleEngineType>(et => et.Id == 2 && et.Type == "Petrol" && et.Abrv == "PE")
            };

            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = 2 },
                Sorting = new SortOptions { SortBy = "Type", SortAscending = true },
                Filtering = new FilterOptions { SearchText = "P" }
            };

            var pagedResult = new PagedResult<IVehicleEngineType>(engineTypesList, 2, 1, 2);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetPagedEngineTypes(queryOptions);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(2);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedEngineTypes_WithNullOptions_ShouldUseDefaultOptions()
        {
            // Arrange
            var engineTypesList = new List<IVehicleEngineType>
            {
                Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel" && et.Abrv == "DI"),
                Mock.Of<IVehicleEngineType>(et => et.Id == 2 && et.Type == "Petrol" && et.Abrv == "PE")
            };

            var pagedResult = new PagedResult<IVehicleEngineType>(engineTypesList, 2, 1, 10);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetPagedEngineTypes(null);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.Is<QueryOptions>(
                opt => opt.Paging.PageNumber == 1 && opt.Paging.PageSize == 10 &&
                       opt.Sorting.SortBy == "Type" && opt.Sorting.SortAscending == true)),
                Times.Once);
        }

        [Fact]
        public async Task GetEngineTypeById_WithValidId_ShouldReturnEngineType()
        {
            // Arrange
            var engineType = Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel" && et.Abrv == "DI");

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(engineType);

            // Act
            var result = await _service.GetEngineTypeById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Type.Should().Be("Diesel");
            result.Abrv.Should().Be("DI");
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetEngineTypeById_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((IVehicleEngineType)null);

            // Act & Assert
            await FluentActions.Invoking(() => _service.GetEngineTypeById(999))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Tip motora s ID-om 999 nije pronađen.");
        }

        [Fact]
        public async Task GetFirstEngineTypeAsync_WithValidPredicate_ShouldReturnEngineType()
        {
            // Arrange
            var engineType = Mock.Of<IVehicleEngineType>(et => et.Id == 1 && et.Type == "Diesel" && et.Abrv == "DI");
            Expression<Func<IVehicleEngineType, bool>> predicate = et => et.Type == "Diesel";

            _mockRepository.Setup(repo => repo.GetFirstAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()))
                .ReturnsAsync(engineType);

            // Act
            var result = await _service.GetFirstEngineTypeAsync(predicate);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Type.Should().Be("Diesel");
            _mockRepository.Verify(repo => repo.GetFirstAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetFirstEngineTypeAsync_WithNullPredicate_ShouldThrowException()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _service.GetFirstEngineTypeAsync(null))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*Predicate ne može biti null*");
        }

        [Fact]
        public async Task EngineTypeExistsByName_WhenExists_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.EngineTypeExistsByName("Diesel");

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task EngineTypeExistsByName_WhenNotExists_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.EngineTypeExistsByName("InvalidType");

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task EngineTypeExistsByName_WithEmptyName_ShouldReturnFalse()
        {
            // Act
            var result = await _service.EngineTypeExistsByName("");

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleEngineType, bool>>>()), Times.Never);
        }
    }
} 