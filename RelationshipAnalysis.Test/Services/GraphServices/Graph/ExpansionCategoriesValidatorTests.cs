using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Test.Services.GraphServices.Graph
{
    public class ExpansionCategoriesValidatorTests
    {
        private readonly ICategoryNameValidator _nodeCategoryValidator;
        private readonly ICategoryNameValidator _edgeCategoryValidator;
        private readonly ExpansionCategoriesValidator _sut;

        public ExpansionCategoriesValidatorTests()
        {
            _nodeCategoryValidator = Substitute.For<ICategoryNameValidator>();
            _edgeCategoryValidator = Substitute.For<ICategoryNameValidator>();

            _sut = new ExpansionCategoriesValidator(_nodeCategoryValidator, _edgeCategoryValidator);
        }

        [Fact]
        public async Task ValidateCategories_ShouldReturnInvalidSourceCategory_WhenSourceCategoryIsInvalid()
        {
            // Arrange
            var sourceCategoryName = "InvalidSource";
            var targetCategoryName = "ValidTarget";
            var edgeCategoryName = "ValidEdge";

            _nodeCategoryValidator.Validate(sourceCategoryName).Returns(Task.FromResult(false));
            _nodeCategoryValidator.Validate(targetCategoryName).Returns(Task.FromResult(true));
            _edgeCategoryValidator.Validate(edgeCategoryName).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.ValidateCategories(sourceCategoryName, targetCategoryName, edgeCategoryName);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(Resources.InvalidSourceNodeCategory, result.graphDto.Message);
        }

        [Fact]
        public async Task ValidateCategories_ShouldReturnInvalidTargetCategory_WhenTargetCategoryIsInvalid()
        {
            // Arrange
            var sourceCategoryName = "ValidSource";
            var targetCategoryName = "InvalidTarget";
            var edgeCategoryName = "ValidEdge";

            _nodeCategoryValidator.Validate(sourceCategoryName).Returns(Task.FromResult(true));
            _nodeCategoryValidator.Validate(targetCategoryName).Returns(Task.FromResult(false));
            _edgeCategoryValidator.Validate(edgeCategoryName).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.ValidateCategories(sourceCategoryName, targetCategoryName, edgeCategoryName);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(Resources.InvalidTargetNodeCategory, result.graphDto.Message);
        }

        [Fact]
        public async Task ValidateCategories_ShouldReturnInvalidEdgeCategory_WhenEdgeCategoryIsInvalid()
        {
            // Arrange
            var sourceCategoryName = "ValidSource";
            var targetCategoryName = "ValidTarget";
            var edgeCategoryName = "InvalidEdge";

            _nodeCategoryValidator.Validate(sourceCategoryName).Returns(Task.FromResult(true));
            _nodeCategoryValidator.Validate(targetCategoryName).Returns(Task.FromResult(true));
            _edgeCategoryValidator.Validate(edgeCategoryName).Returns(Task.FromResult(false));

            // Act
            var result = await _sut.ValidateCategories(sourceCategoryName, targetCategoryName, edgeCategoryName);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(Resources.InvalidEdgeCategory, result.graphDto.Message);
        }

        [Fact]
        public async Task ValidateCategories_ShouldReturnValidResult_WhenAllCategoriesAreValid()
        {
            // Arrange
            var sourceCategoryName = "ValidSource";
            var targetCategoryName = "ValidTarget";
            var edgeCategoryName = "ValidEdge";

            _nodeCategoryValidator.Validate(sourceCategoryName).Returns(Task.FromResult(true));
            _nodeCategoryValidator.Validate(targetCategoryName).Returns(Task.FromResult(true));
            _edgeCategoryValidator.Validate(edgeCategoryName).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.ValidateCategories(sourceCategoryName, targetCategoryName, edgeCategoryName);

            // Assert
            Assert.True(result.isValid);
            Assert.Null(result.graphDto);
        }
    }
}
