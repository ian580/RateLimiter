using Xunit;
using RateLimiter;
using Moq;
using FluentAssertions;
using System;
using System.Threading;

namespace RateLimiterTest
{
    public class ProxyResourceTest
    {
        [Fact]
        public void CanInvokeRealResource()
        {
            // Arrange
            var resource = new Mock<IResource<string>>();
            var proxy = new ProxyResource<string>(1, TimeSpan.FromSeconds(1), resource.Object);

            // Act
            proxy.Invoke();

            // Assert
            resource.Verify(x => x.Invoke());
        }

        [Fact]
        public void ProxyLimitsCalls()
        {
            // Arrange
            var resource = new Mock<IResource<string>>();
            var proxy = new ProxyResource<string>(1, TimeSpan.FromDays(1), resource.Object);

            // Act
            proxy.Invoke();
            Action fail = () => proxy.Invoke();

            // Assert
            fail.Should().Throw<LimitedException>();
            resource.Verify(r => r.Invoke(), Times.Once);
        }

        [Fact]
        public void LimitWindowIsApplied()
        {
            // Arrange
            var resource = new Mock<IResource<string>>();
            var proxy = new ProxyResource<string>(1, TimeSpan.FromMilliseconds(100), resource.Object);

            // Act
            proxy.Invoke();
            Action fail = () => proxy.Invoke();
            Action pass = () => proxy.Invoke();

            // Assert
            fail.Should().Throw<LimitedException>();

            Thread.Sleep(100);

            pass.Should().NotThrow<LimitedException>();

            resource.Verify(r => r.Invoke(), Times.Exactly(2));
        }
    }
}
