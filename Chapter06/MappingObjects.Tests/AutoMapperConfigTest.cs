using AutoMapper; // To use MapperConfiguration.
using MappingObjects.Mappers; // To use CartToSummaryMapper.

namespace MappingObjects.Tests;
public class TestAutoMapperConfig
{
    [Fact]
    public void TestSummaryMapping()
    {
        MapperConfiguration config = CartToSummaryMapper.GetMapperConfiguration();

        config.AssertConfigurationIsValid();
    }
}