using SourceGenerator.MediatR.Proxy.Tests.Support;
using System.Threading.Tasks;
using Xunit;
using static SourceGenerator.MediatR.Proxy.Tests.Support.TestResources;

namespace SourceGenerator.MediatR.Proxy.Tests
{
    public class MediatrGeneratorTests
    {
        [Fact]
        public async Task MediatrGenerator_query()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceSource = await GetTestOutputs("InterfaceMyQuery.cs");
            var expectedInterfaceImplSource = await GetTestOutputs("InterfaceImplMyQuery.cs");

            var attributeUsage = @"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GetSourceByFileName("IMyService.cs").Source;
            var interfaceImplSource = result.GetSourceByFileName("MyService.cs").Source;
            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_query_only_interface()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceSource = await GetTestOutputs("InterfaceMyQuery.cs");

            var attributeUsage = @"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GetSourceByFileName("IMyService.cs").Source;
            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }

        [Fact(Skip = "Need a way to make interface available for compilation.")]
        public async Task MediatrGenerator_query_only_implementation()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceImplSource = await GetTestOutputs("InterfaceImplMyQuery.cs");


            var attributeUsage = @"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceImplSource = result.GetSourceByFileName("MyService.cs").Source;
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_command()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceSource = await GetTestOutputs("InterfaceMyQuery.cs");
            var expectedInterfaceImplSource = await GetTestOutputs("InterfaceImplMyQuery.cs");

            var attributeUsage = @$"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GetSourceByFileName("IMyService.cs").Source;
            var interfaceImplSource = result.GetSourceByFileName("MyService.cs").Source;
            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }

        [Fact]
        public async Task MediatrGenerator_command_only_interface()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceSource = await GetTestOutputs("InterfaceMyQuery.cs");

            var attributeUsage = @"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyContractAttribute(""IMyService"", ""Project.Shared"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GetSourceByFileName("IMyService.cs").Source;
            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }

        [Fact(Skip = "Need a way to make interface available for compilation.")]
        public async Task MediatrGenerator_command_only_implementation()
        {
            var myQuerySource = await GetTestInput("MyQuery.cs");
            var expectedInterfaceImplSource = await GetTestOutputs("InterfaceImplMyQuery.cs");

            var attributeUsage = @$"
[assembly: SourceGenerator.MediatR.Proxy.MediatrProxyImplementationAttribute(""IMyService"", ""Project.Shared"", ""Project.Application.Services"")]
";
            var result = GeneratorRunner.Run(myQuerySource, attributeUsage, new MediatrProxyGenerator());

            var interfaceImplSource = result.GetSourceByFileName("MyService.cs").Source;
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }
    }
}