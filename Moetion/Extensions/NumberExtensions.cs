// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;

namespace Moetion.Extensions
{
    public static class NumberExtensions
    {
        public static double Remap(this double val, double min, double max)
        {
            return (Math.Clamp(val, min, max) - min) / (max - min);
        }
    }
}
