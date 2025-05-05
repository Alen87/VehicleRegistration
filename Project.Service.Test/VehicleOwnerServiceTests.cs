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
    public class VehicleOwnerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehicleOwnerRepository> _mockRepository;
        private readonly VehicleOwnerService _service;

        public VehicleOwnerServiceTests()
        {
            _mockRepository = new Mock<IVehicleOwnerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.VehicleOwnerRepository).Returns(_mockRepository.Object);
            _service = new VehicleOwnerService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllOwners_ShouldReturnAllOwners()
        {
            // Arrange
            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = int.MaxValue },
                Sorting = new SortOptions()
            };
            
            var expectedOwners = new List<IVehicleOwner>
            {
                new VehicleOwner { Id = 1, FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) },
                new VehicleOwner { Id = 2, FirstName = "Marko", LastName = "Maric", DateOfBirth = new DateTime(1992, 2, 2) }
            };
            
            var pagedResult = new PagedResult<IVehicleOwner>(expectedOwners, expectedOwners.Count, 1, int.MaxValue);
            _mockRepository.Setup(x => x.GetPagedAsync(It.IsAny<QueryOptions>()))
                .ReturnsAsync(pagedResult);

            
            var result = await _service.GetAllOwners(queryOptions);

           
            result.Should().BeEquivalentTo(pagedResult);
        }

        [Fact]
        public async Task GetPagedOwners_ShouldReturnPagedOwners()
        {
           
            var queryOptions = new QueryOptions
            {
                Paging = new PagingOptions { PageNumber = 1, PageSize = 10 },
                Sorting = new SortOptions { SortBy = "LastName", SortAscending = true }
            };
            var expectedResult = new PagedResult<IVehicleOwner>(
                new List<IVehicleOwner> { new VehicleOwner { Id = 1, FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) } },
                1, 1, 10);
            _mockRepository.Setup(x => x.GetPagedAsync(queryOptions))
                .ReturnsAsync(expectedResult);

            
            var result = await _service.GetPagedOwners(queryOptions);

           
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetOwnerById_WhenOwnerExists()
        {
            
            var expectedOwner = new VehicleOwner { Id = 1, FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) };
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedOwner);

            
            var result = await _service.GetOwnerById(1);

           
            result.Should().BeEquivalentTo(expectedOwner);
        }

        [Fact]
        public async Task GetOwnerById_WhenOwnerDoesNotExist()
        {
            
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((IVehicleOwner)null);

            
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetOwnerById(1));
        }

        [Fact]
        public async Task AddOwner_WithValidOwner()
        {
            
            var owner = new VehicleOwner { FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) };
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleOwner, bool>>>()))
                .ReturnsAsync(false);
            _mockRepository.Setup(x => x.AddAsync(owner))
                .ReturnsAsync(owner);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

          
            var result = await _service.AddOwner(owner);

            result.Should().BeEquivalentTo(owner);
        }

        [Fact]
        public async Task AddOwner_WithExistingName()
        {
           
            var owner = new VehicleOwner { FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) };
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleOwner, bool>>>()))
                .ReturnsAsync(true);

            
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddOwner(owner));
        }

        [Fact]
        public async Task UpdateOwner_WithValidOwner()
        {
           
            var owner = new VehicleOwner { Id = 1, FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) };
            _mockRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(owner);
            _mockRepository.Setup(x => x.UpdateAsync(owner))
                .ReturnsAsync(owner);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            
            var result = await _service.UpdateOwner(owner);

           
            result.Should().BeEquivalentTo(owner);
        }

        [Fact]
        public async Task DeleteOwner_WhenOwnerExists()
        {
            
            _mockRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            
            var result = await _service.DeleteOwner(1);

            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task OwnerExistsByFirstNameAndLastName_WhenOwnerExists()
        {
            
            _mockRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<IVehicleOwner, bool>>>()))
                .ReturnsAsync(true);

            
            var result = await _service.OwnerExistsByFirstNameAndLastName("Pero", "Peric");

            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetFirstOwnerAsync_WhenOwnerExists()
        {
            
            var expectedOwner = new VehicleOwner { Id = 1, FirstName = "Pero", LastName = "Peric", DateOfBirth = new DateTime(1990, 1, 1) };
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleOwner, bool>>>()))
                .ReturnsAsync(expectedOwner);

           
            var result = await _service.GetFirstOwnerAsync(o => o.FirstName == "Pero");

           
            result.Should().BeEquivalentTo(expectedOwner);
        }

        [Fact]
        public async Task GetFirstOwnerAsync_WhenOwnerDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            
            _mockRepository.Setup(x => x.GetFirstAsync(It.IsAny<Expression<Func<IVehicleOwner, bool>>>()))
                .ReturnsAsync((IVehicleOwner)null);

            
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetFirstOwnerAsync(o => o.FirstName == "NonExistentOwner"));
        }
    }
} 