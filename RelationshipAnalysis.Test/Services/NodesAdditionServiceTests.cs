using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices;

namespace RelationshipAnalysis.Test.Services;

public class NodesAdditionServiceTests
{
    private INodesAdditionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public NodesAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Add(new NodeCategory()
        {
            NodeCategoryName = "Account",
            NodeCategoryId = 1
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task AddNodes_ShouldReturnBadRequest_WhenUniqueHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SomeHeader"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "Account",
            UniqueKeyHeaderName = "SomeHeaderThatDoesntExist"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddNodes_ShouldReturnBadRequest_WhenNodeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SomeHeader"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "SomeNodeCategoryThatDoesntExist",
            UniqueKeyHeaderName = "AccountID"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddNodes_ShouldReturnSuccess_WhenNodeDtoIsValid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "AccountID").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);

        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "Account",
            UniqueKeyHeaderName = "AccountID"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

//     // TODO
//     [Fact]
//     public async Task AddNodes_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
//     {
//         // Arrange
//         var expected = new ActionResponse<MessageDto>()
//         {
//             Data = new MessageDto(Resources.FailedAddRecordsMessage),
//             StatusCode = StatusCodeType.BadRequest
//         };
//         var csvContent = @"""AccountID"",""CardID"",""IBAN""
// ""6534454617"",""6104335000000190"",""IR120778801496000000198""
// ""6534454617"",""6104335000000190"",""IR120778801496000000198""
// ""4000000028"",""6037699000000020"",""IR033880987114000000028""
// ";
//         var fileToBeSend = CreateFileMock(csvContent);
//
//         var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
//         validatorMock.Validate(fileToBeSend, "AccountID").Returns(expected);
//         var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
//         processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
//         var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
//         _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
//
//         // Act
//         var result = await _sut.AddNodes(new UploadNodeDto()
//         {
//             File = fileToBeSend,
//             NodeCategoryName = "Account",
//             UniqueKeyHeaderName = "AccountID"
//         });
//         // Assert
//         Assert.Equivalent(expected, result);
//         using var scope = _serviceProvider.CreateScope();
//         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//         Assert.Equal(0, context.Nodes.Count());
//     }
    
    
    private IFormFile CreateFileMock(string csvContent)
    {
        var csvFileName = "test.csv";
        var fileMock = Substitute.For<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvContent);
        writer.Flush();
        stream.Position = 0;

        fileMock.OpenReadStream().Returns(stream);
        fileMock.FileName.Returns(csvFileName);
        fileMock.Length.Returns(stream.Length);
        return fileMock;
    }
}