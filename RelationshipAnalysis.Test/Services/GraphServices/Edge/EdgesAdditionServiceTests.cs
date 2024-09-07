using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Edge;

public class EdgesAdditionServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private IEdgesAdditionService _sut;

    public EdgesAdditionServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
        context.Add(new NodeCategory
        {
            NodeCategoryName = "Account",
            NodeCategoryId = 1
        });
        context.Add(new EdgeCategory
        {
            EdgeCategoryName = "Transaction",
            EdgeCategoryId = 1
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist", "SourceAcount", "DestiantionAccount")
            .Returns(expected);
        var processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "SomeHeaderThatDoesntExist",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueSourceHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SomeHeaderThatDoesntExist", "DestiantionAccount")
            .Returns(expected);
        var processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SomeHeaderThatDoesntExist",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueTargetHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SourceAcount", "SomeHeaderThatDoesntExist")
            .Returns(expected);
        ICsvProcessorService processorMock;
        processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "SomeHeaderThatDoesntExist"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }


    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenEdgeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidEdgeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        var processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "NotExistCategory",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenSourceNodeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidSourceNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        var processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "NotExistAccount",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenTargetNodeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidTargetNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        var processorMock = Substitute.For<ICsvProcessorService>();
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "NotExistAccount",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }


    [Fact]
    public async Task AddEdges_ShouldReturnSuccess_WhenNodeDtoIsValid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.SuccessfulEdgeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SourceAcount", "DestiantionAccount").Returns(expected);
        var processorMock = Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
        var additionServiceMock = Substitute.For<IContextEdgesAdditionService>();
        additionServiceMock.AddToContext(Arg.Any<ApplicationDbContext>(), Arg.Any<EdgeCategory>(),
                Arg.Any<NodeCategory>(), Arg.Any<NodeCategory>(), Arg.Any<List<dynamic>>(), Arg.Any<UploadEdgeDto>())
            .Returns(expected);
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock,
            new MessageResponseCreator());
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }


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