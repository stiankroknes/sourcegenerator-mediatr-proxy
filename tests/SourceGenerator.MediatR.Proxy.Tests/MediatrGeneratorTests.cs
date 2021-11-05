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
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyQuery.cs");
            var expectedInterfaceSource = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceMyQuery.cs");
            var expectedInterfaceImplSource  = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceImplMyQuery.cs");
            
            var attributeUsage = @"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyContractAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"")]
[assembly: MediatrProxyImplementationAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"", ""Migration.Operations.Server.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage,new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IOperationsServiceProxy.cs", StringComparison.Ordinal)).Source;
            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("OperationsServiceProxy.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }
        
        [Fact]
        public async Task MediatrGenerator_query_only_interface()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyQuery.cs");
            var expectedInterfaceSource = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceMyQuery.cs");

            var attributeUsage = @"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyContractAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage,new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IOperationsServiceProxy.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }
        
        [Fact]
        public async Task MediatrGenerator_query_only_implementation()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyQuery.cs");
            var expectedInterfaceImplSource  = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceImplMyQuery.cs");

            var attributeUsage = @"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyImplementationAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"", ""Migration.Operations.Server.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage,new MediatrProxyGenerator());

            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("OperationsServiceProxy.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }
        
        [Fact]
        public async Task MediatrGenerator_command()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyCommand.cs");
            var expectedInterfaceSource = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceMyCommand.cs");
            var expectedInterfaceImplSource  = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceImplMyCommand.cs");

            var attributeUsage = @$"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyContractAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"")]
[assembly: MediatrProxyImplementationAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"", ""Migration.Operations.Server.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource,attributeUsage, new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IOperationsServiceProxy.cs", StringComparison.Ordinal)).Source;
            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("OperationsServiceProxy.cs", StringComparison.Ordinal)).Source;
            
            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }
        
        [Fact]
        public async Task MediatrGenerator_command_only_interface()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyCommand.cs");
            var expectedInterfaceSource = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceMyCommand.cs");

            var attributeUsage = @"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyContractAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"")]
";
            var result = GeneratorRunner.Run(userSource, attributeUsage,new MediatrProxyGenerator());

            var interfaceSource = result.GeneratedCode.Single(g => g.FileName.Equals("IOperationsServiceProxy.cs", StringComparison.Ordinal)).Source;

            expectedInterfaceSource.AssertSourceCodesEquals(interfaceSource);
        }
        [Fact]
        public async Task MediatrGenerator_command_only_implementation()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\MyCommand.cs");
            var expectedInterfaceImplSource  = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\InterfaceImplMyCommand.cs");

            var attributeUsage = @$"
[assembly: System.Reflection.AssemblyMetadataAttribute(""Foo"", ""Bar"")]
[assembly: MediatrProxyImplementationAttribute(""IOperationsServiceProxy"", ""Migration.Operations.Shared"", ""Migration.Operations.Server.Application.Services"")]
";
            var result = GeneratorRunner.Run(userSource,attributeUsage, new MediatrProxyGenerator());

            var interfaceImplSource = result.GeneratedCode.Single(g => g.FileName.Equals("OperationsServiceProxy.cs", StringComparison.Ordinal)).Source;
            
            expectedInterfaceImplSource.AssertSourceCodesEquals(interfaceImplSource);
        }
    }
}