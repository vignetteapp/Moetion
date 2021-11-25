// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;

namespace Moetion.Extensions
{
    public static class NumberExtensions
    {
        public static float Remap(this float val, float min, float max) => (Math.Clamp(val, min, max) - min) / (max - min);

        /// <summary>
        /// Gets a normalized angle.
        /// </summary>
        /// <param name="radians">Angle in radians to normalize.</param>
        /// <returns>Normalized values to -1, 1.</returns>
        public static float NormalizeAngle(this float radians)
        {
            var twoPi = MathF.PI * 2;
            var angle = radians % twoPi;
            angle = angle > MathF.PI ? angle - twoPi : angle < -MathF.PI ? twoPi + angle : angle;
            return angle / MathF.PI;
        }
    }
}
