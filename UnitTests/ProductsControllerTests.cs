using System;
using FakeItEasy;
using KingICT.Controllers;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
	public class ProductsControllerTests
	{
        private readonly IProductRepository _fakeRepo;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
		{
            _fakeRepo = A.Fake<IProductRepository>();
            _productsController = new ProductsController(_fakeRepo);
        }

        [Fact]
        public async Task GetProducts_WhenProductsAreNull_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetProducts()).Returns(Task.FromResult<List<Products>>(null));

            // Act
            var result = await _productsController.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetProducts_WhenProductsAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
            };
            A.CallTo(() => _fakeRepo.GetProducts()).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetProducts_WhenProductsExist_ReturnsProducts()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
                new Products {Id = 1, Title = "product1", Description = "prod1", Category = "prod", Price = 1.2, Thumbnail = "sample.jpeg"},
                new Products {Id = 2, Title = "product2", Description = "prod2", Category = "prod", Price = 4, Thumbnail = "sample2.jpeg"},
            };
            A.CallTo(() => _fakeRepo.GetProducts()).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Equal(fakeProducts.Count, returnValue.Count);
            for (int i = 0; i < fakeProducts.Count; i++)
            {
                Assert.Equal(fakeProducts[i].Id, returnValue[i].Id);
                Assert.Equal(fakeProducts[i].Title, returnValue[i].Title);
            }
        }

        [Fact]
        public async Task GetProductById_WhenProductIsNull_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetProductById(1)).Returns(Task.FromResult<Products>(null));

            // Act
            var result = await _productsController.GetProductById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Products>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetProductById_WhenProductIsEmpty_ReturnsEmptyProduct()
        {
            // Arrange
            var fakeProducts = new Products
            {
            };
            A.CallTo(() => _fakeRepo.GetProductById(1)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProductById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Products>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<Products>(okResult.Value);

            Assert.Equal(fakeProducts, returnValue);
        }

        [Fact]
        public async Task GetProductById_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            var fakeProducts = new Products
            {
                Id = 1, Title = "prod1", Description = "prod1", Category = "prod", Price = 1.2, Thumbnail = "sample.jpg"
            };
            A.CallTo(() => _fakeRepo.GetProductById(1)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProductById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Products>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<Products>(okResult.Value);

            Assert.Equal(fakeProducts, returnValue);
        }

        [Fact]
        public async Task GetProductsByName_WhenProductIsNull_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetProductsByName("name")).Returns(Task.FromResult<List<Products>>(null));

            // Act
            var result = await _productsController.GetProductsByName("name");

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetProductsByName_WhenProductsDoNotExist_ReturnsEmptyList()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
            };
            A.CallTo(() => _fakeRepo.GetProductsByName("name")).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProductsByName("name");

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetProductsByName_WhenProductExist_ReturnsProducts()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
                new Products {Id = 1, Title = "product1", Description = "prod1", Category = "prod", Price = 1.2, Thumbnail = "sample.jpeg"},
            };
            A.CallTo(() => _fakeRepo.GetProductsByName("name")).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.GetProductsByName("name");

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Equal(fakeProducts.Count, returnValue.Count);
            for (int i = 0; i < fakeProducts.Count; i++)
            {
                Assert.Equal(fakeProducts[i].Id, returnValue[i].Id);
                Assert.Equal(fakeProducts[i].Title, returnValue[i].Title);
            }
        }

        [Fact]
        public async Task FilterProducts_WhenProductsAreNull_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.FilterProducts("", null)).Returns(Task.FromResult<List<Products>>(null));

            // Act
            var result = await _productsController.FilterProducts("", null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


        [Fact]
        public async Task FilterProducts_WhenCategoryAndPriceAreNotProvided_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.FilterProducts("", null)).Returns(Task.FromResult<List<Products>>(null));

            // Act
            var result = await _productsController.FilterProducts("", null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


        [Fact]
        public async Task FilterProducts_WhenCategoryIsProvidedButProductsAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
            };
            string category = "beauty";
            A.CallTo(() => _fakeRepo.FilterProducts(category, null)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts(category, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task FilterProducts_WhenCategoryIsProvidedAndProductsExist_ReturnsProducts()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
                new Products {Id = 1, Title = "product1", Description = "prod1", Category = "beauty", Price = 1.2, Thumbnail = "sample.jpeg"},
            };
            string category = "beauty";
            A.CallTo(() => _fakeRepo.FilterProducts(category, null)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts(category, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Equal(fakeProducts, returnValue);
        }

        [Fact]
        public async Task FilterProducts_WhenPriceIsProvidedButProductsAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
            };
            double price = 1.2;
            A.CallTo(() => _fakeRepo.FilterProducts("", price)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts("", price);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task FilterProducts_WhenPriceIsProvidedAndProductsExist_ReturnsProducts()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
                new Products {Id = 1, Title = "product1", Description = "prod1", Category = "beauty", Price = 1.2, Thumbnail = "sample.jpeg"},
            };
            double price = 1.2;
            A.CallTo(() => _fakeRepo.FilterProducts("", price)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts("", price);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Equal(fakeProducts, returnValue);
        }

        [Fact]
        public async Task FilterProducts_WhenCategoryAndPriceAreProvidedButProductsAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
            };
            string category = "beauty";
            double price = 1.2;
            A.CallTo(() => _fakeRepo.FilterProducts(category, price)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts(category, price);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task FilterProducts_WhenCategoryAndPriceAreProvidedAndProductsExist_ReturnsProducts()
        {
            // Arrange
            var fakeProducts = new List<Products>
            {
                new Products {Id = 1, Title = "product1", Description = "prod1", Category = "beauty", Price = 1.2, Thumbnail = "sample.jpeg"},
            };
            string category = "beauty";
            double price = 1.2;
            A.CallTo(() => _fakeRepo.FilterProducts(category, price)).Returns(Task.FromResult(fakeProducts));

            // Act
            var result = await _productsController.FilterProducts(category, price);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Products>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Products>>(okResult.Value);

            Assert.Equal(fakeProducts, returnValue);
        }
    }
}

