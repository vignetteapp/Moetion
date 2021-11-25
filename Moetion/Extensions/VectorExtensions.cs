// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;
using System.Numerics;

namespace Moetion.Extensions
{
    public static class VectorExtensions
    {
        public static double Find2DAngle(double cx, double cy, double ex, double ey)
        {
            var dy = ey - cy;
            var dx = ex - cx;
            return Math.Atan2(dy, dx);
        }

        public static Vector2 Unit(this Vector2 vector) => vector / vector.Length();
        public static Vector3 Unit(this Vector3 vector) => vector / vector.Length();

        public static Vector3 RollPitchYaw(Vector3 a, Vector3 b, Vector3? c)
        {
            if (c == null)
            {
                return new Vector3
                {
                    X = (float)Find2DAngle(a.Z, a.Y, b.Z, b.Y).NormalizeAngle(),
                    Y = (float)Find2DAngle(a.Z, a.X, b.Z, b.X).NormalizeAngle(),
                    Z = (float)Find2DAngle(a.X, a.Y, b.X, b.Y).NormalizeAngle(),
                };
            }

            var qb = b - a;
            var qc = (Vector3)(c - a);
            var n = Vector3.Cross(qb, qc);

            var unitZ = n.Unit();
            var unitX = qb.Unit();
            var unitY = Vector3.Cross(unitZ, unitX);

            var beta = Math.Asin(unitZ.X);
            var alpha = Math.Atan2(-unitZ.Y, unitZ.Z);
            var gamma = Math.Atan2(-unitY.X, unitX.X);

            return new Vector3
            {
                X = (float)alpha.NormalizeAngle(),
                Y = (float)beta.NormalizeAngle(),
                Z = (float)gamma.NormalizeAngle(),
            };
        }
    }
}
