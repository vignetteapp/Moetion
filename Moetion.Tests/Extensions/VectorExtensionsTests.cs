// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System.Numerics;
using NUnit.Framework;
using Moetion.Extensions;

namespace Moetion.Tests.Extensions
{
    public class VectorExtensionsTests
    {

        [Test]
        public void Vector2DistanceTest()
        {
            Vector2 vector = new Vector2(2, 2);
            Vector2 other = new Vector2(4, 2);
            Assert.AreEqual(2, vector.Distance(other));
        }

        [Test]
        public void Vector3DistanceTest()
        {
            Vector3 vector = new Vector3(2, 2, 2);
            Vector3 other = new Vector3(4, 2, 2);
            Assert.AreEqual(2, vector.Distance(other));
        }
    }
}
