// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under LGPL v3. See LICENSE.md for details.

using System.Numerics;
using Akihabara.Framework.Protobuf;
using Google.Protobuf.Collections;
using Moetion.Extensions;

namespace Moetion.Hands
{
    public static class HandSolver
    {
        public static Hand Solve(NormalizedLandmarkList list, Handedness side = Handedness.Left)
        {
            var landmarks = list.Landmark;

            var hand = new Hand
            {
                Palm = new[]
                {
                    landmarks[0].ToVector(),
                    landmarks[side == Handedness.Right ? 17 : 5].ToVector(),
                    landmarks[side == Handedness.Right ? 5 : 17].ToVector(),
                }
            };

            hand.HandRotation = getRotation(hand.Palm[0], hand.Palm[1], hand.Palm[2]);
            hand.HandRotation.Y = hand.HandRotation.Z;
            hand.HandRotation.Y -= side == Handedness.Left ? 0.4f : 0.4f;

            hand.Wrist = new Vector3(hand.HandRotation.X, hand.HandRotation.Y, hand.HandRotation.Z);

            hand.RingProximal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 0, 13, 14));
            hand.RingIntermediate = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 13, 14, 15));
            hand.RingDistal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 14, 15, 16));

            hand.IndexProximal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 0, 5, 6));
            hand.IndexIntermediate = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 5, 6, 7));
            hand.IndexDistal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 6, 7, 8));

            hand.MiddleProximal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 0, 9, 10));
            hand.MiddleIntermediate = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 9, 10, 11));
            hand.MiddleDistal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 10, 11, 12));

            hand.ThumbProximal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 0, 1, 2));
            hand.ThumbIntermediate = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 1, 2, 3));
            hand.ThumbDistal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 2, 3, 4));

            hand.LittleProximal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 0, 17, 18));
            hand.LittleIntermediate = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 17, 18, 19));
            hand.LittleDistal = new Vector3(0, 0, getAngleBetween3DCoords(landmarks, 18, 19, 20));

            rigFingers(ref hand, side);

            return hand;
        }

        private static void rigFingers(ref Hand hand, Handedness side)
        {
            int direction = side == Handedness.Right ? 1 : -1;

            hand.Wrist.X = Math.Clamp(hand.Wrist.X * 2 * direction, -0.3f, 0.3f);
            hand.Wrist.Y = Math.Clamp(
                hand.Wrist.Y * 2.3f,
                side == Handedness.Right ? -1.2f : -0.6f,
                side == Handedness.Right ? 0.6f : 1.6f);
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

        private static void rigOtherFinger(ref Vector3 tracked, Handedness side, int direction)
        {
            tracked.Z = Math.Clamp(
                tracked.Z * -MathF.PI * direction,
                side == Handedness.Right ? -MathF.PI : 0f,
                side == Handedness.Right ? 0f : MathF.PI
            );
        }

        private static void rigThumbFinger(ref Vector3 tracked, Handedness side, HandSegment segment, int direction)
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
                    side == Handedness.Right ? -1f : -0.3f,
                    side == Handedness.Right ? 0.3f : 1f
                );
                thumb.X = Math.Clamp(
                    start.X * tracked.Z * -MathF.PI * damp.X, -0.6f, 0.3f
                );
                thumb.Y = Math.Clamp(
                    start.Y * tracked.Z * -MathF.PI * damp.Y * direction,
                    side == Handedness.Right ? -1f : -0.3f,
                    side == Handedness.Right ? 0.3f : 1f
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

            float pitch = normalizeAngle(MathF.Asin(unitZ.X));
            float roll = normalizeAngle(MathF.Atan2(-unitZ.Y, unitZ.Z));
            float yaw = normalizeAngle(MathF.Atan2(-unitY.X, unitX.X));

            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            static float normalizeAngle(float radians)
            {
                float angle = radians % MathF.Tau;
                angle = angle > MathF.PI ? angle - MathF.Tau : angle < -MathF.PI ? MathF.Tau + angle : angle;
                return angle / MathF.PI;
            }
        }

        private static float getAngleBetween3DCoords(RepeatedField<NormalizedLandmark> lm, int a, int b, int c)
            => getAngleBetween3DCoords(lm[a].ToVector(), lm[b].ToVector(), lm[c].ToVector());

        private static float getAngleBetween3DCoords(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = Vector3.Subtract(a, b);
            var v2 = Vector3.Subtract(c, b);
            var v1norm = Vector3.Divide(v1, MathF.Sqrt(Vector3.Dot(v1, v1)));
            var v2norm = Vector3.Divide(v2, MathF.Sqrt(Vector3.Dot(v2, v2)));
            var dotProducts = Vector3.Dot(v1norm, v2norm);
            var angle = MathF.Acos(dotProducts);

            return normalizeRadians(angle);

            static float normalizeRadians(float radians)
            {
                if (radians >= MathF.PI / 2)
                    radians -= 2 * MathF.PI;

                if (radians <= -MathF.PI / 2)
                {
                    radians += 2 * MathF.PI;
                    radians = MathF.PI - radians;
                }

                return radians / MathF.PI;
            }
        }

        private enum HandSegment
        {
            Proximal,
            Intermediate,
            Distral,
        }
    }

    public enum Handedness
    {
        Left,
        Right,
    }
}
