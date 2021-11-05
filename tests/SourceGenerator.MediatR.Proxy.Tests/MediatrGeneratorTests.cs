using System;
using SourceGenerator.MediatR.Proxy.Tests.Support;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SourceGenerator.MediatR.Proxy.Tests
{
    public class MediatrGeneratorTests
    {
        [Fact]
        public async Task MediatrGenerator_query()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyQuery.cs"));
            var expectedInterfaceSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceMyQuery.cs"));
            var expectedInterfaceImplSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceImplMyQuery.cs"));

            var attributeUsage = @"
[assembly: MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
[assembly: MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IMyService.cs", StringComparison.Ordinal)).Source;
            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("MyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_query_only_interface()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyQuery.cs"));
            var expectedInterfaceSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceMyQuery.cs"));


            var attributeUsage = @"
[assembly: MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IMyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }

        [Fact(Skip = "Need a way to make interface available for compilation.")]
        public async Task MediatrGenerator_query_only_implementation()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyQuery.cs"));
            var expectedInterfaceImplSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceImplMyQuery.cs"));


            var attributeUsage = @"
[assembly: MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("MyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_command()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyCommand.cs"));
            var expectedInterfaceSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceMyCommand.cs"));
            var expectedInterfaceImplSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceImplMyCommand.cs"));


            var attributeUsage = @$"
[assembly: MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
[assembly: MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IMyService.cs", StringComparison.Ordinal)).Source;
            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("MyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_command_only_interface()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyCommand.cs"));
            var expectedInterfaceSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceMyCommand.cs"));

            var attributeUsage = @"
[assembly: MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IMyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }
        
        [Fact(Skip ="Need a way to make interface available for compilation.")]
        public async Task MediatrGenerator_command_only_implementation()
        {
            var userSource = await File.ReadAllTextAsync(TestInputFile("MyCommand.cs"));
            var expectedInterfaceImplSource = await File.ReadAllTextAsync(TestOutputFile("InterfaceImplMyCommand.cs"));

            var attributeUsage = @$"
[assembly: MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage, new MediatrProxyGenerator());

            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("MyService.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        private static string TestInputFile(string filename) => Path.Combine("..", "..", "..", "TestInputs", filename);
        private static string TestOutputFile(string filename) => Path.Combine("..", "..", "..", "TestOutputs", filename);
    }
}