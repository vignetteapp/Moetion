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
        private Vector2 v21 = new Vector2(Random.Shared.Next(), Random.Shared.Next());
        private Vector2 v22 = new Vector2(Random.Shared.Next(), Random.Shared.Next());
        
        private Vector3 v31 = new Vector3(Random.Shared.Next(), Random.Shared.Next(), Random.Shared.Next());
        private Vector3 v32 = new Vector3(Random.Shared.Next(), Random.Shared.Next(), Random.Shared.Next());

       [Benchmark]
        public void Vector2Distance()
        {
            v21.Distance(v22);
        }
        
        [Benchmark]
        public void Vector3Distance()
        {
            v31.Distance(v32);
        }
    }
}