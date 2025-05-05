using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Sorting;
using Project.Model;
using Project.Model.Common;
using Project.Repository.Common;
using Project.Service;
using Xunit;

namespace Project.Service.Test
{
    public class VehicleModelServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehicleModelRepository> _mockRepository;
        private readonly VehicleModelService _service;

        public VehicleModelServiceTests()
        {
            _mockRepository = new Mock<IVehicleModelRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.VehicleModelRepository).Returns(_mockRepository.Object);
            _service = new VehicleModelService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllModels_ShouldReturnAllModels()
        {
            var expectedModels = new List<IVehicleModel>
            {
                new VehicleModel { Id = 1, Name = "Model 1", MakeId = 1 },
                new VehicleModel { Id = 2, Name = "Model 2", MakeId = 1 }
            };
            
            var pagedResult = new PagedResult<IVehicleModel>(expectedModels, 2, 1, int.MaxValue);
            
            _mockRepository.Setup(x => x.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllModels(new QueryOptions());

            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(expectedModels);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetPagedModels_ShouldReturnPagedModels()
        {
            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = 10 },
                Sorting = new SortOptions { SortBy = "Name", SortAscending = true }
            };
            var expectedResult = new PagedResult<IVehicleModel>(
                new List<IVehicleModel> { new VehicleModel { Id = 1, Name = "Model 1" } },
                1, 1, 10);
            _mockRepository.Setup(x => x.GetPagedAsync(queryOptions))
                .ReturnsAsync(expectedResult);

            var result = await _service.GetPagedModels(queryOptions);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetModelById_WhenModelExists_ShouldReturnModel()
        {
            var expectedModel = new VehicleModel { Id = 1, Name = "Model 1" };
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedModel);

            var result = await _service.GetModelById(1);

            result.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task GetModelById_WhenModelDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((IVehicleModel)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetModelById(1));
        }

        [Fact]
        public async Task AddModel_WithValidModel_ShouldAddModel()
        {
            var model = new VehicleModel { Name = "New Model", MakeId = 1 };
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleModel, bool>>>()))
                .ReturnsAsync(false);
            _mockRepository.Setup(x => x.AddAsync(model))
                .ReturnsAsync(model);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.AddModel(model);

            result.Should().BeEquivalentTo(model);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddModel_WithExistingName_ShouldThrowInvalidOperationException()
        {
            var model = new VehicleModel { Name = "Existing Model", MakeId = 1 };
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleModel, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddModel(model));
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateModel_WithValidModel_ShouldUpdateModel()
        {
            var model = new VehicleModel { Id = 1, Name = "Updated Model", MakeId = 1 };
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(model);
            _mockRepository.Setup(x => x.UpdateAsync(model))
                .ReturnsAsync(model);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.UpdateModel(model);

            result.Should().BeEquivalentTo(model);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteModel_WhenModelExists_ShouldReturnTrue()
        {
            _mockRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.DeleteModel(1);

            result.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ModelExistsByName_WhenModelExists_ShouldReturnTrue()
        {
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleModel, bool>>>()))
                .ReturnsAsync(true);

            var result = await _service.ModelExistsByName("Existing Model");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetFirstModelAsync_WhenModelExists()
        {
            var expectedModel = new VehicleModel { Id = 1, Name = "Model 1", MakeId = 1 };
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleModel, bool>>>()))
                .ReturnsAsync(expectedModel);

            var result = await _service.GetFirstModelAsync(m => m.Name == "Model 1");

            result.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task GetFirstModelAsync_WhenModelDoesNotExist()
        {
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleModel, bool>>>()))
                .ReturnsAsync((IVehicleModel)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetFirstModelAsync(m => m.Name == "NonExistentModel"));
        }
    }
}