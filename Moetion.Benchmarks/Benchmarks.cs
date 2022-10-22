// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Moetion.Benchmarks;

public class Benchmarks
{
    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Benchmarks).Assembly).Run(args, DefaultConfig.Instance
        // .WithSummaryStyle(new SummaryStyle(CultureInfo.InvariantCulture, printUnitsInHeader: false, SizeUnit.B, TimeUnit.Microsecond))
        );
}
