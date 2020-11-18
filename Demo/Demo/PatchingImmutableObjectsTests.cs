using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Demo
{
    public class PatchingImmutableObjectsTests
    {
        [Fact]
        public void Change_Single_Prop_In_ValueObject_While_Copying_The_Rest_With_Named_Method()
        {
            // Arrange
            var address = new Address(
                new City("city"), 
                new Street("street"), 
                new Country("country"),
                new ZipCode("zipcode"));
            
            // Act
            var newAddress = address.ChangeCity(new City("cityChanged"));
            
            // Assert
            using (new AssertionScope())
            {
                newAddress.City.Value.Should().Be("cityChanged");
                newAddress.Street.Value.Should().Be("street");
                newAddress.Country.Value.Should().Be("country");
                newAddress.ZipCode.Value.Should().Be("zipcode");
            }
        }

        [Fact]
        public void Change_Single_Prop_In_ValueObject_While_Copying_The_Rest_With_Single_Method()
        {
            // Arrange
            var address = new Address(
                new City("city"), 
                new Street("street"), 
                new Country("country"),
                new ZipCode("zipcode"));
            
            // Act
            var newAddress1 = address.Change(new City("cityChanged"));
            var newAddress2 = address.Change(new Street("streetChanged"));
            var newAddress3 = address.Change(new Country("countryChanged"));
            
            // Assert
            using (new AssertionScope())
            {
                newAddress1.City.Value.Should().Be("cityChanged");
                newAddress1.Street.Value.Should().Be("street");
                newAddress1.Country.Value.Should().Be("country");
                newAddress1.ZipCode.Value.Should().Be("zipcode");
                
                newAddress2.City.Value.Should().Be("city");
                newAddress2.Street.Value.Should().Be("streetChanged");
                newAddress2.Country.Value.Should().Be("country");
                newAddress2.ZipCode.Value.Should().Be("zipcode");
                
                newAddress3.City.Value.Should().Be("city");
                newAddress3.Street.Value.Should().Be("street");
                newAddress3.Country.Value.Should().Be("countryChanged");
                newAddress3.ZipCode.Value.Should().Be("zipcode");

                // ensure that original address has not changed
                address.City.Value.Should().Be("city");
                address.Street.Value.Should().Be("street");
                address.Country.Value.Should().Be("country");
                address.ZipCode.Value.Should().Be("zipcode");
            }
        }
        
        [Fact]
        public void Change_Single_Prop_In_ValueObject_While_Copying_The_Rest_Throws_When_Not_Implemented()
        {
            // Arrange
            var address = new Address(
                new City("city"), 
                new Street("street"), 
                new Country("country"),
                new ZipCode("zipcode"));
            
            // Act
            Action action = () => address.Change(new ZipCode("zipcodeChanged"));
            
            // Assert
            action.Should().Throw<Exception>().WithMessage("ups");
        }
    }

    public class Address : ValueObject
    {
        public Address(City city, Street street, Country country, ZipCode zipCode)
        {
            City = city;
            Street = street;
            Country = country;
            ZipCode = zipCode;
        }

        public Street Street { get; }
        public City City { get; }
        public Country Country { get; }
        public ZipCode ZipCode { get; }
        
        
        public Address ChangeCity(City city) => new Address(city, Street, Country, ZipCode);

        public Address Change(ValueObject valueObject)
        {
            return Apply((dynamic) valueObject);
        }

        private Address Apply(Street street) => new Address(City, street, Country, ZipCode);

        private Address Apply(City city) => new Address(city, Street, Country, ZipCode);

        private Address Apply(Country country) => new Address(City, Street, country, ZipCode);
        
        // Catch all
        private Address Apply(ValueObject _) => throw new NotImplementedException("ups");
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return City;
            yield return Street;
            yield return Country;
            yield return ZipCode;
        }
    }

    public class Street : ValueObject
    {
        public string Value { get; }

        public Street(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
    
    public class City : ValueObject
    {
        public string Value { get; }

        public City(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
    
    public class Country : ValueObject
    {
        public string Value { get; }

        public Country(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
    
    public class ZipCode : ValueObject
    {
        public string Value { get; }

        public ZipCode(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}