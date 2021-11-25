// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using Moetion.Extensions;

namespace Moetion.Benchmarks.Vectors
{
    [DisassemblyDiagnoser(maxDepth: 1)] // change to 0 for just the [Benchmark] method
    [MemoryDiagnoser(displayGenColumns: false)]
    public class VectorExtensions
    {
        private Vector3 v31 = new Vector3(Random.Shared.Next(), Random.Shared.Next(), Random.Shared.Next());
        private Vector3 v32 = new Vector3(Random.Shared.Next(), Random.Shared.Next(), Random.Shared.Next());
    }
}
