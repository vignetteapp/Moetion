// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;
using System.Numerics;

namespace Moetion.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Calculates the euclidean distance between two vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double Distance(this Vector2 vector, Vector2 other)
        {
            //The Math.Pow get replaced by the compiler during inlining, so no slowdown because of looping
            return Math.Sqrt(Math.Pow(vector.X - other.X, 2) + Math.Pow(vector.Y - other.Y, 2));
        }

        /// <summary>
        /// Calculates the euclidean distance between two vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double Distance(this Vector3 vector, Vector3 other)
        {
            //The Math.Pow get replaced by the compiler during inlining, so no slowdown because of looping
            return Math.Sqrt(Math.Pow(vector.X - other.X, 2) + Math.Pow(vector.Y - other.Y, 2) + Math.Pow(vector.Z - other.Z, 2));
        }
    }
}
