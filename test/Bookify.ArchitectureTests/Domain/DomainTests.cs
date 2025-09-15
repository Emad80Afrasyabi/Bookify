using Bookify.ArchitectureTests.Infrastructure;
using Bookify.Domain.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace Bookify.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvent_ShouldHave_DomainEventPostfix()
    {
        TestResult? result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_ShouldHave_PrivateParameterlessConstructor()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        List<Type> failingTypes = (from entityType in entityTypes let constructors = entityType.GetConstructors(bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance) where !constructors.Any(constructorInfo => constructorInfo.IsPrivate && constructorInfo.GetParameters().Length == 0) select entityType).ToList();

        failingTypes.Should().BeEmpty();
    }
}