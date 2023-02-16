using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using RetSettlementDates.Domain.Abstractions;
using RetSettlementDates.Domain.Binders;
using RetSettlementDates.Domain.DataObjects;
using RetSettlementDates.Unit.Fixtures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RetSettlementDates.Unit.Domain.Binders
{
    /* Notes:
     * 
     * I Should test more cases here, but it could took some time
     * And considering as a test I think it's okay for now
     */

    public class CsvFileBinderUnit
    {
        public static string GetFilePath(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);

            return Path.Combine(directory, "resources", fileName);
        }

        [AutoArrange]
        [Theory(DisplayName = "When valid file should bind all settlements")]
        public async Task Bind_WhenValidFile_ShouldBindAll([Frozen]Mock<IDataService> dsMock, IDataService ds)
        {
            // Arrange
            var list = new List<Settlement>();
            dsMock.Setup(s => s.ProcessSettlement(It.IsAny<Settlement>(), It.IsAny<CancellationToken>()))
                  .Callback<Settlement, CancellationToken>((settlement, _) =>
                  {
                      list.Add(settlement);
                  })
                  .Returns(Task.CompletedTask);

            var input = new CsvFileBinder.Input(GetFilePath("settlement_test.csv"), ';', CultureInfo.GetCultureInfo("pt-br"));
            var binder = new CsvFileBinder(input, new List<IDataService> { ds });

            // Act
            await binder.Bind(CancellationToken.None);

            // Assert
            list.Should().HaveCount(1, "has just one item in the file");

            AssertSettlement(list[0]);
        }

        public void AssertSettlement(Settlement item)
        {
            item.SettlementLocationID.Should().Be(1);
            item.SettlementLocationName.Should().Be("Houston");
            item.DateOfService.Should().Be(new DateTime(2022, 5, 1));
            item.PricePerMWh.Should().Be(33.79F);
            item.VolumeMWh.Should().Be(28.2225644F);
        }
    }

    public class CsvReaderUnit
    {
        [Fact(DisplayName = "Error when not found file should throws an exception")]
        public void Open_WhenFileNotFound_ShouldThrowAnException()
        {
            // Arrange
            var reader = new CsvReader("whatever", ',');

            // Act
            Action act = () => reader.Open();

            // Assert
            act.Should().Throw<FileNotFoundException>("the file do not exists");
        }

        [Fact(DisplayName = "When valid file but not open should throw exception")]
        public void ReadLines_WhenValidFileButNotOpened_ShouldThrowsException()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);
            var path = Path.Combine(directory, "resources", "test.csv");

            using var reader = new CsvReader(path, ',');

            // Act && Assert
            Func<Task> act = async () =>
            {
                await foreach (var line in reader.ReadLinesAsync())
                {
                    // whatever
                }
            };

            act.Should().ThrowAsync<Exception>("not open readers should throw exception");
        }

        [Fact(DisplayName = "When valid file should read all")]
        public async Task ReadLines_WhenValidFile_ShouldReadAll()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);
            var path = Path.Combine(directory, "resources", "test.csv");

            var count = 0;
            using var reader = new CsvReader(path, ',');

            reader.Open();

            // Act && Assert
            await foreach(var line in reader.ReadLinesAsync())
            {
                line.Length.Should().Be(2, "the file has only two cols");

                for (int i = 0; i < line.Length; i++)
                    line[i].Should().Be($"line{count}_f{i}", "the cols follow this pattern");

                count++;
            }
        }
    }
}
