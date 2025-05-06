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
    public class VehicleMakeServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehicleMakeRepository> _mockRepository;
        private readonly VehicleMakeService _service;

        public VehicleMakeServiceTest()
        {
            _mockRepository = new Mock<IVehicleMakeRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.VehicleMakeRepository).Returns(_mockRepository.Object);
            _service = new VehicleMakeService(_mockUnitOfWork.Object);
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

            var pagedResult = new PagedResult<IVehicleMake>(makesList, 3, 1, int.MaxValue);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllMakes(new QueryOptions());

           
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().BeEquivalentTo(makesList);
            _mockRepository.Verify(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()), Times.Once);
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
                Filtering = new FilteringOptions { SearchText = "A" }
            };

            var pagedResult = new PagedResult<IVehicleMake>(makesList, 2, 1, 2);

            _mockRepository.Setup(repo => repo.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            
            var result = await _service.GetPagedMakes(queryOptions);

           
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
        public async Task AddMake_WithValidMake_ShouldAddAndReturnMake()
        {
           
            var make = Mock.Of<IVehicleMake>(m => m.Name == "Toyota" && m.Abrv == "TOY");

            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(repo => repo.AddAsync(make))
                .ReturnsAsync(Mock.Of<IVehicleMake>(m => m.Id == 4 && m.Name == "Toyota" && m.Abrv == "TOY"));
                
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

          
            var result = await _service.AddMake(make);

           
            result.Should().NotBeNull();
            result.Id.Should().Be(4);
            result.Name.Should().Be("Toyota");
            _mockRepository.Verify(repo => repo.ExistsAsync(It.IsAny<Expression<Func<IVehicleMake, bool>>>()), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(make), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
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
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}