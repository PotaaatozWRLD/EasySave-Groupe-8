using Xunit;
using Moq;
using System.Collections.Generic;

namespace EasySave.Tests
{
    public interface IFoo
    {
        bool DoSomething(string value);
    }

    public class MoqTest
    {
        [Fact]
        public void TestMoq()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.DoSomething("test")).Returns(true);

            var result = mock.Object.DoSomething("test");

            Assert.True(result);
            mock.Verify(x => x.DoSomething("test"), Times.Once);
        }
    }
}
