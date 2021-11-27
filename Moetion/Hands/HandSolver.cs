// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using Akihabara.Framework.Protobuf;
using Google.Protobuf.Collections;
using Moetion.Extensions;
using static Moetion.Extensions.VectorExtensions;

namespace Moetion.Hands
{
    public static class HandSolver
    {
        public static Hand Solve(NormalizedLandmarkList list, Side side = Side.Left)
        {
            var landmarks = list.Landmark;

            var hand = new Hand
            {
                Palm = new[]
                {
                    landmarks[0].ToVector(),
                    landmarks[side == Side.Right ? 17 : 5].ToVector(),
                    landmarks[side == Side.Right ? 5 : 17].ToVector(),
                }
            };

            hand.Rotation = getRotation(hand.Palm[0], hand.Palm[1], hand.Palm[2]);
            hand.Rotation.Y = hand.Rotation.Z;
            hand.Rotation.Y -= side == Side.Left ? 0.4f : 0.4f;

            hand.Wrist = new Vector3(hand.Rotation.X, hand.Rotation.Y, hand.Rotation.Z);

            hand.RingProximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 13, 14));
            hand.RingIntermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(13, 14, 15));
            hand.RingDistal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(14, 15, 16));

            hand.IndexProximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 5, 6));
            hand.IndexIntermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(5, 6, 7));
            hand.IndexDistal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(6, 7, 8));

            hand.MiddleProximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 9, 10));
            hand.MiddleIntermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(9, 10, 11));
            hand.MiddleDistal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(10, 11, 12));

            hand.ThumbProximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 1, 2));
            hand.ThumbIntermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(1, 2, 3));
            hand.ThumbDistal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(2, 3, 4));

            hand.LittleProximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 17, 18));
            hand.LittleIntermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(17, 18, 19));
            hand.LittleDistal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(18, 19, 20));

            rigFingers(ref hand, side);

            return hand;
        }

        private static void rigFingers(ref Hand hand, Side side)
        {
            int direction = side == Side.Right ? 1 : -1;

            hand.Wrist.X = Math.Clamp(hand.Wrist.X * 2 * direction, -0.3f, 0.3f);
            hand.Wrist.Y = Math.Clamp(
                hand.Wrist.Y * 2.3f,
                side == Side.Right ? -1.2f : -0.6f,
                side == Side.Right ? 0.6f : 1.6f);
            hand.Wrist.Z = hand.Wrist.Z * -2.3f * direction;

            rigThumbFinger(ref hand.ThumbProximal, side, HandSegment.Proximal, direction);
            rigThumbFinger(ref hand.ThumbIntermediate, side, HandSegment.Intermediate, direction);
            rigThumbFinger(ref hand.ThumbDistal, side, HandSegment.Distral, direction);

            rigOtherFinger(ref hand.IndexDistal, side, direction);
            rigOtherFinger(ref hand.IndexIntermediate, side, direction);
            rigOtherFinger(ref hand.IndexProximal, side, direction);

            rigOtherFinger(ref hand.MiddleDistal, side, direction);
            rigOtherFinger(ref hand.MiddleIntermediate, side, direction);
            rigOtherFinger(ref hand.MiddleProximal, side, direction);

            rigOtherFinger(ref hand.RingDistal, side, direction);
            rigOtherFinger(ref hand.RingIntermediate, side, direction);
            rigOtherFinger(ref hand.RingProximal, side, direction);

            rigOtherFinger(ref hand.LittleDistal, side, direction);
            rigOtherFinger(ref hand.LittleIntermediate, side, direction);
            rigOtherFinger(ref hand.LittleProximal, side, direction);
        }

        private static void rigOtherFinger(ref Vector3 tracked, Side side, int direction)
        {
            tracked.Z = Math.Clamp(
                tracked.Z * -MathF.PI * direction,
                side == Side.Right ? -MathF.PI : 0f,
                side == Side.Right ? 0f : MathF.PI
            );
        }

        private static void rigThumbFinger(ref Vector3 tracked, Side side, HandSegment segment, int direction)
        {
            var damp = new Vector3(
                segment == HandSegment.Proximal ? 2.2f : segment == HandSegment.Intermediate ? 0 : 0,
                segment == HandSegment.Proximal ? 2.2f : segment == HandSegment.Intermediate ? 0.7f : 1f,
                segment == HandSegment.Proximal ? 0.5f : segment == HandSegment.Intermediate ? 0.5f : 0.5f
            );

            var start = new Vector3(
                segment == HandSegment.Proximal ? 1.2f : segment == HandSegment.Distral ? -0.2f : -0.2f,
                segment == HandSegment.Proximal ? 1.1f * direction : segment == HandSegment.Distral ? 0.1f * direction : 0.1f * direction,
                segment == HandSegment.Proximal ? 0.2f * direction : segment == HandSegment.Distral ? 0.2f * direction : 0.2f * direction
            );

            var thumb = Vector3.Zero;

            if (segment == HandSegment.Proximal)
            {
                thumb.Z = Math.Clamp(
                    start.Z * tracked.Z * -MathF.PI * damp.Z * direction,
                    side == Side.Right ? -1f : -0.3f,
                    side == Side.Right ? 0.3f : 1f
                );
                thumb.X = Math.Clamp(
                    start.X * tracked.Z * -MathF.PI * damp.X, -0.6f, 0.3f
                );
                thumb.Y = Math.Clamp(
                    start.Y * tracked.Z * -MathF.PI * damp.Y * direction,
                    side == Side.Right ? -1f : -0.3f,
                    side == Side.Right ? 0.3f : 1f
                );
            }
            else
            {
                thumb.Z = Math.Clamp(start.Z + tracked.Z * -MathF.PI * damp.Z * direction, -2f, 2f);
                thumb.X = Math.Clamp(start.X + tracked.Z * -MathF.PI * damp.X, -2f, 2f);
                thumb.Y = Math.Clamp(start.Y + tracked.Z * -MathF.PI * damp.Y * direction, -2f, 2f);
            }

            tracked = thumb;
        }

        private static Quaternion getRotation(Vector3 a, Vector3 b, Vector3 c)
        {
            var qb = Vector3.Subtract(b, a);
            var qc = Vector3.Subtract(c, a);
            var n = Vector3.Cross(qb, qc);

            var unitZ = Vector3.Divide(n, MathF.Sqrt(Vector3.Dot(n, n)));
            var unitX = Vector3.Divide(qb, MathF.Sqrt(Vector3.Dot(qb, qb)));
            var unitY = Vector3.Cross(unitZ, unitX);

            float pitch = MathF.Asin(unitZ.X).NormalizeAngle();
            float roll = MathF.Atan2(-unitZ.Y, unitZ.Z).NormalizeAngle();
            float yaw = MathF.Atan2(-unitY.X, unitX.X).NormalizeAngle();

            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        private enum HandSegment
        {
            Proximal,
            Intermediate,
            Distral,
        }
    }
}
