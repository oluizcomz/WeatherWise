using Moq;
using WeatherWise_api.Controllers;
using WeatherWise_api.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using WeatherWise_api.Modal;

namespace FavoriteTests.Controllers;
public class FavoriteControllerTests
{
    private readonly Mock<IFavoriteRepository> _favoriteRepositoryMock;
    private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
    private readonly FavoriteController _favoriteController;
    
    public FavoriteControllerTests()
    {
        _favoriteRepositoryMock = new Mock<IFavoriteRepository>();
        _loggerMock = new Mock<ILogger<WeatherForecastController>>();
        _favoriteController = new FavoriteController(_loggerMock.Object, _favoriteRepositoryMock.Object);
    }

    #region Get
    /// <summary>
    /// Vaidação para quando não é enviado valor para a requisição 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetFavorite_ReturnsBadRequest_WhenEmailIsNull()
    {
        var result = await _favoriteController.GetFavorite(null);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    /// <summary>
    /// Validação para quando eviado email e retorna countryID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetFavorite_ReturnsOkResult_WithFavorites()
    {
        var email = "test@example.com";
        var mockFavorites = new List<Favorite> { new Favorite { Email = email, CountryId = "xx" } };
        _favoriteRepositoryMock.Setup(repo => repo.GetFavorites(email)).ReturnsAsync(mockFavorites);

        var result = await _favoriteController.GetFavorite(email);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Favorite>>(okResult.Value);
        Assert.Single(returnValue);
    }
    /// <summary>
    /// validação para quando da erro na requicição com o dynamo DB
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetFavorite_Returns500_WhenExceptionIsThrown()
    {
        var email = "test@example.com";
        _favoriteRepositoryMock.Setup(repo => repo.GetFavorites(email)).ThrowsAsync(new Exception("Database error"));

        var result = await _favoriteController.GetFavorite(email);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    /// <summary>
    /// Validação para quando o email ou countryId não forem fornecidos.
    /// </summary>
    /// <returns></returns>
    #endregion
    
    #region Post
    [Fact]
    public async Task AddFavorite_ReturnsBadRequest_WhenEmailOrCountryIdIsNull()
    {
        var result = await _favoriteController.AddFavorite(null, "BR");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Validação para quando um favorito é adicionado com sucesso.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddFavorite_ReturnsOkResult_WhenFavoriteIsAdded()
    {
        var email = "test@example.com";
        var countryId = "BR";

        var result = await _favoriteController.AddFavorite(email, countryId);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Validação para quando há um erro na requisição ao DynamoDB.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddFavorite_Returns500_WhenExceptionIsThrown()
    {
        var email = "test@example.com";
        var countryId = "BR";
        _favoriteRepositoryMock.Setup(repo => repo.AddFavorite(email, countryId)).ThrowsAsync(new Exception("Database error"));

        var result = await _favoriteController.AddFavorite(email, countryId);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    #endregion
    
    #region Delete
    /// <summary>
    /// Validação para quando o email ou countryId não forem fornecidos.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteFavorite_ReturnsBadRequest_WhenEmailOrCountryIdIsNull()
    {
        var result = await _favoriteController.DeleteFavorite(null, "BR");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Validação para quando um favorito é deletado com sucesso.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteFavorite_ReturnsOkResult_WhenFavoriteIsDeleted()
    {
        var email = "test@example.com";
        var countryId = "BR";

        var result = await _favoriteController.DeleteFavorite(email, countryId);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Validação para quando há um erro na requisição ao DynamoDB.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteFavorite_Returns500_WhenExceptionIsThrown()
    {
        var email = "test@example.com";
        var countryId = "BR";
        _favoriteRepositoryMock.Setup(repo => repo.DeleteFavorite(email, countryId)).ThrowsAsync(new Exception("Database error"));

        var result = await _favoriteController.DeleteFavorite(email, countryId);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    #endregion

}
