using AzuriteTestContainer.Fixture;

namespace AzuriteTestContainer;

[CollectionDefinition("SharedTestCollection")]
public sealed class SharedTestCollection : ICollectionFixture<TestContainersFixture> { }