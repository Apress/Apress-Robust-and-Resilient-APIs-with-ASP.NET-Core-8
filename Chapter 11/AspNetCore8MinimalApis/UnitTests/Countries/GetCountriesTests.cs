using AspNetCore8MinimalApis.Endpoints;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using AutoFixture;
using Domain.DTOs;
using Domain.Services;
using ExpectedObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Xunit;

namespace UnitTests.Countries;

public class GetCountriesTests
{
    private readonly ICountryMapper _countryMapper;
    private readonly ICountryService _countryService;
    private readonly Fixture _fixture;


    public GetCountriesTests()
    {
        _countryMapper = Substitute.For<ICountryMapper>();
        _countryService = Substitute.For<ICountryService>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task WhenGetCountriesReceivesNullPagingParametersAndGetAllAsyncMethodReturnsCountries_ShouldFillUpDefaultPagingParametersAndReturnCountries()
    {
        // Arrange
        int? pageIndex = null;
        int? pageSize = null;
        var expectedPaging = new PagingDto
        {
            PageIndex = 1,
            PageSize = 10
        }.ToExpectedObject();

        var countries = _fixture.CreateMany<CountryDto>(2).ToList();
        var expectedCountries = countries.ToExpectedObject();

        var mappedCountries = _fixture.CreateMany<Country>(2).ToList();
        var expectedMappedCountries = mappedCountries.ToExpectedObject();
        
        _countryService.GetAllAsync(Arg.Any<PagingDto>()).Returns(x => countries);
        _countryMapper.Map(Arg.Any<List<CountryDto>>()).Returns(x => mappedCountries);

        // Act
        var result = (await CountryEndpoints.GetCountries(pageIndex, pageSize, _countryMapper, _countryService)) as Ok<List<Country>>;

        // Assert
        expectedMappedCountries.ShouldEqual(result.Value);
        await _countryService.Received(1).GetAllAsync(Arg.Is<PagingDto>(x => expectedPaging.Matches(x)));
        _countryMapper.Received(1).Map(Arg.Is<List<CountryDto>>(x => expectedCountries.Matches(x)));
    }
}
