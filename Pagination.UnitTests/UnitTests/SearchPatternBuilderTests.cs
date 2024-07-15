using Pagination.Enums;

namespace Pagination.UnitTests.UnitTests
{
    public class PageableExtensionsTests
    {
        [Fact]
        public void GetSearchPatternFormat__StartsWith__ReturnsCorrectPattern()
        {
            // Arrange
            var matchType = ExpressionMatchTypeEnum.StartsWith;

            // Act
            var result = PageableExtensions.GetSearchPatternFormat(matchType);

            // Assert
            Assert.Equal("{0}%", result);
        }

        [Fact]
        public void GetSearchPatternFormat__EndsWith__ReturnsCorrectPattern()
        {
            // Arrange
            var matchType = ExpressionMatchTypeEnum.EndsWith;

            // Act
            var result = PageableExtensions.GetSearchPatternFormat(matchType);

            // Assert
            Assert.Equal("%{0}", result);
        }

        [Fact]
        public void GetSearchPatternFormat__Contains__ReturnsCorrectPattern()
        {
            // Arrange
            var matchType = ExpressionMatchTypeEnum.Contains;

            // Act
            var result = PageableExtensions.GetSearchPatternFormat(matchType);

            // Assert
            Assert.Equal("%{0}%", result);
        }

        [Fact]
        public void GetSearchPatternFormat__Equals__ReturnsCorrectPattern()
        {
            // Arrange
            var matchType = ExpressionMatchTypeEnum.Equals;

            // Act
            var result = PageableExtensions.GetSearchPatternFormat(matchType);

            // Assert
            Assert.Equal("{0}", result);
        }

        [Fact]
        public void GetSearchPattern__StartsWith__TakesInStringAndEnum__ReturnsCorrectPattern()
        {
            // Arrange
            var term = "term";
            var matchType = ExpressionMatchTypeEnum.StartsWith;

            // Act
            var result = term.BuildSearchPattern(matchType);

            // Assert
            Assert.Equal("term%", result);
        }

        [Fact]
        public void GetSearchPattern__EndsWith__TakesInStringAndEnum__ReturnsCorrectPattern()
        {
            // Arrange
            var term = "term";
            var matchType = ExpressionMatchTypeEnum.EndsWith;

            // Act
            var result = term.BuildSearchPattern(matchType);

            // Assert
            Assert.Equal("%term", result);
        }

        [Fact]
        public void GetSearchPattern__Contains__TakesInStringAndEnum__ReturnsCorrectPattern()
        {
            // Arrange
            var term = "term";
            var matchType = ExpressionMatchTypeEnum.Contains;

            // Act
            var result = term.BuildSearchPattern(matchType);

            // Assert
            Assert.Equal("%term%", result);
        }

        [Fact]
        public void GetSearchPattern__Equals__TakesInStringAndEnum__ReturnsCorrectPattern()
        {
            // Arrange
            var term = "term";
            var matchType = ExpressionMatchTypeEnum.Equals;

            // Act
            var result = term.BuildSearchPattern(matchType);

            // Assert
            Assert.Equal("term", result);
        }

        [Fact]
        public void GetSearchPattern__StartsWith__TakesInStrings__ReturnsCorrectPattern()
        {
            // Arrange
            var patternFormat = "{0}%";
            var term = "term";

            // Act
            var result = term.BuildSearchPattern(patternFormat);

            // Assert
            Assert.Equal("term%", result);
        }

        [Fact]
        public void GetSearchPattern__EndsWith__TakesInStrings__ReturnsCorrectPattern()
        {
            // Arrange
            var patternFormat = "%{0}";
            var term = "term";

            // Act
            var result = term.BuildSearchPattern(patternFormat);

            // Assert
            Assert.Equal("%term", result);
        }

        [Fact]
        public void GetSearchPattern__Contains__TakesInStrings__ReturnsCorrectPattern()
        {
            // Arrange
            var patternFormat = "%{0}%";
            var term = "term";

            // Act
            var result = term.BuildSearchPattern(patternFormat);

            // Assert
            Assert.Equal("%term%", result);
        }

        [Fact]
        public void GetSearchPattern__Equals__TakesInStrings__ReturnsCorrectPattern()
        {
            // Arrange
            var patternFormat = "{0}";
            var term = "term";

            // Act
            var result = term.BuildSearchPattern(patternFormat);

            // Assert
            Assert.Equal("term", result);
        }
    }
}
