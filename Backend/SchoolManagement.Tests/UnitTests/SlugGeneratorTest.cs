using FluentAssertions;
using Moq;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Common.Utils;
using Xunit;

namespace SchoolManagement.Tests.UnitTests
{
    public class SlugGeneratorTest
    {
        private readonly Mock<IStudentQueryService> _studentQueryMock;

        public SlugGeneratorTest()
        {
            _studentQueryMock = new Mock<IStudentQueryService>();
        }

        [Fact]
        public async Task Slug_WhenNoCollision_ReturnsOriginalSlugUnchanged()
        {
            string baseSlug = "john-doe";
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(baseSlug)).ReturnsAsync(false);

            var result = await CustomSluger.Slug(
                slug => _studentQueryMock.Object.IsExistsBySlugAsync(slug),
                baseSlug);

            result.Should().Be(baseSlug);
            _studentQueryMock.Verify(q => q.IsExistsBySlugAsync(baseSlug), Times.Once);
        }

        [Fact]
        public async Task Slug_WhenNoCollision_OnlyChecksTheBaseSlugOnce()
        {
            string baseSlug = "jane-smith";
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>())).ReturnsAsync(false);

            await CustomSluger.Slug(
                slug => _studentQueryMock.Object.IsExistsBySlugAsync(slug),
                baseSlug);

            _studentQueryMock.Verify(q => q.IsExistsBySlugAsync(baseSlug), Times.Once);
            _studentQueryMock.Verify(q => q.IsExistsBySlugAsync(It.Is<string>(s => s != baseSlug)), Times.Never);
        }

        [Fact]
        public async Task Slug_OneCollision_ReturnsSlugWithSixCharHexSuffix()
        {
            string baseSlug = "reda-elm";
            var checkedSlugs = new List<string>();
            IsRecordExists exists = async slug =>
            {
                checkedSlugs.Add(slug);
                return checkedSlugs.Count == 1;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            result.Should().StartWith($"{baseSlug}-");
            string suffix = result.Substring(baseSlug.Length + 1);
            suffix.Should().HaveLength(6).And.MatchRegex("^[a-f0-9]{6}$");
            checkedSlugs.Count.Should().Be(2);
            checkedSlugs[0].Should().Be(baseSlug);
            checkedSlugs[1].Should().Be(result);
        }

        [Fact]
        public async Task Slug_ThreeCollisions_AttemptsFourTimesAndSettlesOnShortSuffix()
        {
            string baseSlug = "jane-smith";
            var checkedSlugs = new List<string>();
            IsRecordExists exists = async slug =>
            {
                checkedSlugs.Add(slug);
                return checkedSlugs.Count <= 3;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            checkedSlugs.Count.Should().Be(4);
            checkedSlugs[0].Should().Be(baseSlug);
            checkedSlugs.Skip(1).Should().AllSatisfy(s =>
            {
                s.Should().StartWith($"{baseSlug}-");
                s.Substring(baseSlug.Length + 1).Length.Should().Be(6);
            });
            result.Should().Be(checkedSlugs[3]);
        }

        [Fact]
        public async Task Slug_SixCollisions_UsesFullGuidFallbackAfterFiveShortSuffixes()
        {
            string baseSlug = "reda-elm";
            var checkedSlugs = new List<string>();
            IsRecordExists exists = async slug =>
            {
                checkedSlugs.Add(slug);
                return checkedSlugs.Count <= 6;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            checkedSlugs.Count.Should().Be(6);
            checkedSlugs[0].Should().Be(baseSlug);
            checkedSlugs.Skip(1).Should().AllSatisfy(s =>
            {
                s.Should().StartWith($"{baseSlug}-");
                s.Substring(baseSlug.Length + 1).Should().HaveLength(6);
            });
            result.Should().StartWith($"{baseSlug}-");
            string suffix = result.Substring(baseSlug.Length + 1);
            suffix.Should().MatchRegex("^[a-f0-9]{32}$").And.HaveLength(32);
            checkedSlugs.Should().NotContain(result, "full-guid fallback is not re-checked against the delegate");
        }

        [Fact]
        public async Task Slug_FullGuidFallback_DoesNotInvokeExistenceCheckOnFinalGuidSlug()
        {
            string baseSlug = "max-mustermann";
            var checkedSlugs = new List<string>();
            int callIdx = 0;
            IsRecordExists exists = async slug =>
            {
                checkedSlugs.Add(slug);
                callIdx++;
                return callIdx <= 6;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            checkedSlugs.Count.Should().Be(6, "the final full-guid candidate is never checked");
            result.Should().MatchRegex(@"^max-mustermann-[a-f0-9]{32}$");
        }

        [Fact]
        public async Task Slug_DefaultSuffixGenerator_EveryAttemptUsesSixLowercaseHexChars()
        {
            string baseSlug = "collision-prone";
            var suffixes = new List<string>();
            int callCount = 0;
            IsRecordExists exists = async slug =>
            {
                callCount++;
                if (callCount > 1) suffixes.Add(slug.Substring(baseSlug.Length + 1));
                return callCount <= 3;
            };

            await CustomSluger.Slug(exists, baseSlug);

            suffixes.Should().AllSatisfy(s =>
            {
                s.Should().HaveLength(6);
                s.Should().MatchRegex("^[a-f0-9]{6}$");
            });
            suffixes.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public async Task Slug_EmptyBaseSlug_StillFormatsGuidFallbackAsDashPlusGuid()
        {
            string baseSlug = "";
            int iteration = 0;
            IsRecordExists exists = async slug =>
            {
                iteration++;
                return iteration <= 6;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            result.Should().MatchRegex(@"^-[a-f0-9]{32}$");
        }

        [Fact]
        public async Task Slug_UnicodeBaseSlug_NotNormalizedWhenNoCollision()
        {
            string baseSlug = "Marie-Thérèse 杜";
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(baseSlug)).ReturnsAsync(false);

            var result = await CustomSluger.Slug(
                slug => _studentQueryMock.Object.IsExistsBySlugAsync(slug),
                baseSlug);

            result.Should().Be(baseSlug);
        }


        [Fact]
        public async Task Slug_FiveSuffixedAttemptsExist_ReturnsFullGuidOnSixthCollision()
        {
            string baseSlug = "sally-user";
            var checkedSlugs = new List<string>();
            int callIdx = 0;
            IsRecordExists exists = async slug =>
            {
                checkedSlugs.Add(slug);
                callIdx++;
                return true;
            };

            var result = await CustomSluger.Slug(exists, baseSlug);

            checkedSlugs.Count.Should().Be(6);
            checkedSlugs[0].Should().Be(baseSlug);
            checkedSlugs.Skip(1).Should().AllSatisfy(s =>
                s.Substring(baseSlug.Length + 1).Should().HaveLength(6));
            result.Should().MatchRegex(@"^sally-user-[a-f0-9]{32}$");
        }
    }
}
