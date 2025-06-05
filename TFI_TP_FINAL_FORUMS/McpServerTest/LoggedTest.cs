using McpServerTest.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace McpServerTest
{
    public class LoggedTest : IDisposable
    {
        private readonly DelegatingTestOutputHelper _delegatingTestOutputHelper;

        public LoggedTest(ITestOutputHelper testOutputHelper)
        {
            _delegatingTestOutputHelper = new()
            {
                CurrentTestOutputHelper = testOutputHelper,
            };
            XunitLoggerProvider = new XunitLoggerProvider(_delegatingTestOutputHelper);
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddProvider(XunitLoggerProvider);
            });
        }

        public ITestOutputHelper TestOutputHelper => _delegatingTestOutputHelper;
        public ILoggerFactory LoggerFactory { get; set; }
        public ILoggerProvider XunitLoggerProvider { get; }

        public virtual void Dispose()
        {
            _delegatingTestOutputHelper.CurrentTestOutputHelper = null;
        }
    }
}
