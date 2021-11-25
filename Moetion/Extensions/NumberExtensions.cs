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

        /// <summary>
        /// Gets a normalized angle.
        /// </summary>
        /// <param name="radians">Angle in radians to normalize.</param>
        /// <returns>Normalized values to -1, 1.</returns>
        public static double NormalizeAngle(this double radians)
        {
            var twoPi = Math.PI * 2;
            var angle = radians % twoPi;
            angle = angle > Math.PI ? angle - twoPi : angle < -Math.PI ? twoPi + angle : angle;
            return angle / Math.PI;
        }
    }
}
