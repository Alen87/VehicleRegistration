using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Project.Model;
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
    public class VehicleMakeServiceTest
    {
        private readonly Mock<IVehicleMakeRepository> _mockRepository;
        private readonly VehicleMakeService _service;

        public VehicleMakeServiceTest()
        {
            _mockRepository = new Mock<IVehicleMakeRepository>();
            _service = new VehicleMakeService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllMakes_ShouldReturnAllMakes()
        {
            
            var makesList = new List<IVehicleMake>
            {
                Mock.Of<IVehicleMake>(m => m.Id == 1 && m.Name == "BMW" && m.Abrv == "BMW"),
                Mock.Of<IVehicleMake>(m => m.Id == 2 && m.Name == "Audi" && m.Abrv == "AUDI"),
                Mock.Of<IVehicleMake>(m => m.Id == 3 && m.Name == "Mercedes" && m.Abrv == "MERC")
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(makesList);

            var result = await _service.GetAllMakes();

           
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(makesList);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPagedMakes_WithValidOptions_ShouldReturnPagedResult()
        {
           
            var makesList = new List<IVehicleMake>
            {
                Mock.Of<IVehicleMake>(m => m.Id == 1 && m.Name == "BMW" && m.Abrv == "BMW"),
                Mock.Of<IVehicleMake>(m => m.Id == 2 && m.Name == "Audi" && m.Abrv == "AUDI")
            };

            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = 2 },
                Sorting = new SortOptions { SortBy = "Name", SortAscending = true },
                Filtering = new FilterOptions { SearchText = "A" }
            };

            var pagedResult = new PagedResult<IVehicleMake>(makesList, 2, 1, 2);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetPagedMakes(queryOptions);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(2);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedMakes_WithNullOptions_ShouldUseDefaultOptions()
        {
            // Arrange
            var makesList = new List<IVehicleMake>
            {
                Mock.Of<IVehicleMake>(m => m.Id == 1 && m.Name == "BMW" && m.Abrv == "BMW"),
                Mock.Of<IVehicleMake>(m => m.Id == 2 && m.Name == "Audi" && m.Abrv == "AUDI")
            };

            var pagedResult = new PagedResult<IVehicleMake>(makesList, 2, 1, 10);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            
            var result = await _service.GetPagedMakes(null);

           
            result.Should().NotBeNull();
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.Is<QueryOptions>(
                opt => opt.Paging.PageNumber == 1 && opt.Paging.PageSize == 10 &&
                       opt.Sorting.SortBy == "Name" && opt.Sorting.SortAscending == true)),
                Times.Once);
        }

        [Fact]
        public async Task GetMakeById_WithValidId_ShouldReturnMake()
        {
           
            var make = Mock.Of<IVehicleMake>(m => m.Id == 1 && m.Name == "BMW" && m.Abrv == "BMW");

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(make);

           
            var result = await _service.GetMakeById(1);

            
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("BMW");
            result.Abrv.Should().Be("BMW");
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddMake_WithValidMake()
        {
           
            var make = Mock.Of<IVehicleMake>(m => m.Name == "Toyota" && m.Abrv == "TOY");

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(repo => repo.AddAsync(make))
                .ReturnsAsync(Mock.Of<IVehicleMake>(m => m.Id == 4 && m.Name == "Toyota" && m.Abrv == "TOY"));

            var result = await _service.AddMake(make);

           
            result.Should().NotBeNull();
            result.Id.Should().Be(4);
            result.Name.Should().Be("Toyota");
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(make), Times.Once);
        }

        [Fact]
        public async Task AddMake_WithExistingName_ShouldThrowException()
        {
            
            var make = Mock.Of<IVehicleMake>(m => m.Name == "BMW" && m.Abrv == "BMW");

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()))
                .ReturnsAsync(true);

            await FluentActions.Invoking(() => _service.AddMake(make))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Proizvođač vozila s imenom 'BMW' već postoji");

            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<IVehicleMake>()), Times.Never);
        }

        [Fact]
        public async Task GetFirstMakeAsync_WhenMakeExists()
        {
            var expectedModel = new VehicleMake { Id = 1, Name = "Model 1" };
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()))
                .ReturnsAsync(expectedModel);

            var result = await _service.GetFirstMakeAsync(m => m.Name == "Model 1");

            result.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task GetFirstMakeAsync_WhenMakeDoesNotExist()
        {
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()))
                .ReturnsAsync((IVehicleMake)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetFirstMakeAsync(m => m.Name == "NonExistentModel"));
        }
    }


}
}