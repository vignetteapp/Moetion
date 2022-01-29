// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;

namespace Moetion.Extensions
{
    public static class VectorExtensions
    {
        public static float Find2DAngle(float cx, float cy, float ex, float ey)
        {
            var dy = ey - cy;
            var dx = ex - cx;
            return MathF.Atan2(dy, dx);
        }

        public static Vector3 FindRotation(this Vector3 vector, Vector3 other, bool normalize = true)
        {
            var result = new Vector3
            {
                X = Find2DAngle(vector.Z, vector.X, other.Z, other.X),
                Y = Find2DAngle(vector.Z, vector.Y, other.Z, other.Y),
                Z = Find2DAngle(vector.X, vector.Y, other.X, other.Y),
            };

            if (normalize)
            {
                return new Vector3
                {
                    X = result.X.NormalizeRadians(),
                    Y = result.Y.NormalizeRadians(),
                    Z = result.Z.NormalizeRadians(),
                };
            }

            return result;
        }

        public static Vector2 Unit(this Vector2 vector) => vector / vector.Length();
        public static Vector3 Unit(this Vector3 vector) => vector / vector.Length();

        public static Vector2 ToVector2(this Vector3 vector) => new Vector2 { X = vector.X, Y = vector.Y };

        public static Vector3 RollPitchYaw(Vector3 a, Vector3 b, Vector3? c)
        {
            if (c == null)
            {
                return new Vector3
                {
                    X = Find2DAngle(a.Z, a.Y, b.Z, b.Y).NormalizeAngle(),
                    Y = Find2DAngle(a.Z, a.X, b.Z, b.X).NormalizeAngle(),
                    Z = Find2DAngle(a.X, a.Y, b.X, b.Y).NormalizeAngle(),
                };
            }

            var qb = b - a;
            var qc = (Vector3)(c - a);
            var n = Vector3.Cross(qb, qc);

            var unitZ = n.Unit();
            var unitX = qb.Unit();
            var unitY = Vector3.Cross(unitZ, unitX);

            var beta = MathF.Asin(unitZ.X);
            var alpha = MathF.Atan2(-unitZ.Y, unitZ.Z);
            var gamma = MathF.Atan2(-unitY.X, unitX.X);

            return new Vector3
            {
                X = alpha.NormalizeAngle(),
                Y = beta.NormalizeAngle(),
                Z = gamma.NormalizeAngle(),
            };
        }

        /// <summary>
        /// Find 2D angle between 3 points in 3D space.
        /// </summary>
        /// <returns>Single angle normalized to 0, 1.</returns>
        public static float AngleBetween3DCoords(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = a - b;
            var v2 = c - b;

            var v1Norm = v1.Unit();
            var v2Norm = v2.Unit();

            var dotProducts = Vector3.Dot(v1Norm, v2Norm);
            var angle = MathF.Acos(dotProducts);

            return angle.NormalizeRadians();
        }
    }
}
