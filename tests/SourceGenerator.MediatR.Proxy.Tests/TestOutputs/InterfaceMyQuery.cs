﻿//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SourceGenerator.MediatR.Proxy.Tests.TestInputs;

namespace Project.Shared
{
    [System.ServiceModel.ServiceContract]
    public interface IMyService
    {
        [System.ServiceModel.OperationContract]
        System.Threading.Tasks.Task<MyQueryResultType> My(SourceGenerator.MediatR.Proxy.Tests.TestInputs.MyQuery query, CancellationToken cancellationToken = default);
    }
}