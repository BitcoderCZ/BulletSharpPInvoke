﻿// For some reason PolySharp doesn't generate all of the types it should, reports "Unsupported C# language version" even though it's set to 13
#pragma warning disable
#nullable enable annotations

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// An attribute that allows parameters to receive the expression of other parameters.
    /// </summary>
    [global::System.AttributeUsage(global::System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class CallerArgumentExpressionAttribute : global::System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="global::System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The condition parameter value.</param>
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the parameter name the expression is retrieved from.
        /// </summary>
        public string ParameterName { get; }
    }
}
#endif