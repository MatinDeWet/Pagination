using Pagination.Enums;

namespace Pagination.UnitTests.UnitTests
{
    public class PageableExtensionsTests
    {
        public static IEnumerable<object[]> GetSearchPatternFormatData =>
            new List<object[]>
            {
                new object[] { ExpressionMatchTypeEnum.StartsWith, "{0}%" },
                new object[] { ExpressionMatchTypeEnum.EndsWith, "%{0}" },
                new object[] { ExpressionMatchTypeEnum.Contains, "%{0}%" },
                new object[] { ExpressionMatchTypeEnum.Equals, "{0}" }
            };

        [Theory]
        [MemberData(nameof(GetSearchPatternFormatData))]
        public void GetSearchPatternFormat__TakesInEnum__ReturnsCorrectPattern(ExpressionMatchTypeEnum matchType, string expected)
        {
            // Act
            var result = PageableExtensions.GetSearchPatternFormat(matchType);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetSearchPatternData =>
            new List<object[]>
            {
                new object[] { "term", ExpressionMatchTypeEnum.StartsWith, "term%" },
                new object[] { "term", ExpressionMatchTypeEnum.EndsWith, "%term" },
                new object[] { "term", ExpressionMatchTypeEnum.Contains, "%term%" },
                new object[] { "term", ExpressionMatchTypeEnum.Equals, "term" }
            };

        [Theory]
        [MemberData(nameof(GetSearchPatternData))]
        public void GetSearchPattern__TakesInStringAndEnum__ReturnsCorrectPattern(string term, ExpressionMatchTypeEnum matchType, string expected)
        {
            // Act
            var result = term.BuildSearchPattern(matchType);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetBuildSearchPatternData =>
            new List<object[]>
            {
                new object[] { "{0}%", "term", "term%" },
                new object[] { "%{0}", "term", "%term" },
                new object[] { "%{0}%", "term", "%term%" },
                new object[] { "{0}", "term", "term" }
            };

        [Theory]
        [MemberData(nameof(GetBuildSearchPatternData))]
        public void GetSearchPattern__TakesInStrings__ReturnsCorrectPattern(string patternFormat, string term, string expected)
        {
            // Act
            var result = term.BuildSearchPattern(patternFormat);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
