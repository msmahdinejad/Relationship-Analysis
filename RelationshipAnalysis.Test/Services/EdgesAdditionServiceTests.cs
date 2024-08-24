using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Test;

public class EdgesAdditionServiceTests
{
    private IEdgesAdditionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public EdgesAdditionServiceTests()
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
        context.Add(new EdgeCategory()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist", "SourceAcount", "DestiantionAccount")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SomeHeaderThatDoesntExist", "DestiantionAccount")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SourceAcount", "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidEdgeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidSourceNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidTargetNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulEdgeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SourceAcount", "DestiantionAccount").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
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

    // TODO
    [Fact]
    public async Task AddEdges_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulEdgeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "TransactionID", "SourceAcount", "DestiantionAccount").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>() { new Dictionary<string, object>()});
        var additionServiceMock = new Mock<ISingleEdgeAdditionService>();

        // Setup the mock to throw an exception for any inputs
        additionServiceMock
            .Setup(service => service.AddSingleEdge(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ))
            .Throws(new Exception("Custom exception message"));
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock.Object);

        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
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
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(0, context.Nodes.Count());
        Assert.Equal("Custom exception message", result.Data.Message);
    }

}