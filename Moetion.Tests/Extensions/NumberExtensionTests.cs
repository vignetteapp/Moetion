// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using Moetion.Extensions;
using NUnit.Framework;

namespace Moetion.Tests.Extensions
{
    public class NumberExtensionTests
    {
        [Test]
        public void RemapTest0()
        {
            //get random float between 0 and 1
            float number = Random.Shared.NextSingle();

            //remap it between 0 and 1
            float remapped = number.Remap(0f, 1f);

            //it should be the same
            Assert.AreEqual(number, remapped);
        }

        [Test]
        public void RemapTest1()
        {
            //get random float between -10 and 10
            float number = 10f / Random.Shared.Next(-1000000, 1000000);

            //remap it between -1 and 1
            float remapped = number.Remap(-1f, 1f);

            //it should be >= -1 and <= 1
            Assert.GreaterOrEqual(remapped, -1f);
            Assert.LessOrEqual(remapped, 1f);
        }

        [Test]
        public void NormalizeAngleTest()
        {
            //get random float between -1 and 1
            float angle = 1f / Random.Shared.Next(-100000, 100000);

            //normalize it
            float normalized = angle.NormalizeAngle();

            //multiply it by PI because trigonometry
            normalized *= MathF.PI;

            //since it was already normalized nothing should happen (account for precision loss)
            Assert.Less(Math.Abs(angle - normalized), 0.0001);
        }
    }
}
