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
        public static Hand Solve(NormalizedLandmarkList list, Side side)
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

            hand.Rotation = RollPitchYaw(hand.Palm[0], hand.Palm[1], hand.Palm[2]);
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
            var isRight = side == Side.Right;
            var sideFactor = isRight ? 1 : -1;

            hand.Wrist.X = Math.Clamp(hand.Wrist.X * 2 * sideFactor, -.3f, .3f);
            hand.Wrist.Y = Math.Clamp(hand.Wrist.Y * 2.3f, isRight ? -1.2f : -0.6f, isRight ? .6f : 1.6f);
            hand.Wrist.Z *= -2.3f * sideFactor;

            #region Thumb
            {
                #region Proximal
                var dampener = new Vector3(2.2f, 2.2f, .5f);
                var startPos = new Vector3(1.2f, 1.1f * sideFactor, .2f * sideFactor);

                var newThumbProximal = new Vector3
                {
                    X = Math.Clamp(
                        startPos.X + hand.ThumbProximal.Z * -MathF.PI * dampener.X,
                        -.6f, .3f
                    ),
                    Y = Math.Clamp(
                        startPos.Y + hand.ThumbProximal.Z * -MathF.PI * dampener.Y * sideFactor,
                        isRight ? -1 : -.3f, isRight ? .3f : 1
                    ),
                    Z = Math.Clamp(
                        startPos.Z + hand.ThumbProximal.Z * -MathF.PI * dampener.Z * sideFactor,
                        isRight ? -.6f : -.3f, isRight ? .3f : .6f
                    ),
                };

                hand.ThumbProximal = newThumbProximal;
                #endregion
            }
            {
                #region Intermediate
                var dampener = new Vector3(0, .7f, .5f);
                var startPos = new Vector3(-.2f, .1f * sideFactor, .2f * sideFactor);

                var newThumbIntermediate = new Vector3
                {
                    X = Math.Clamp(startPos.X + hand.ThumbIntermediate.Z * MathF.PI * dampener.X, -2, 2),
                    Y = Math.Clamp(startPos.Y + hand.ThumbIntermediate.Z * MathF.PI * dampener.Y * sideFactor, -2, 2),
                    Z = Math.Clamp(startPos.Z + hand.ThumbIntermediate.Z * MathF.PI * dampener.Z * sideFactor, -2, 2),
                };

                hand.ThumbIntermediate = newThumbIntermediate;
                #endregion
            }
            {
                #region Distal
                var dampener = new Vector3(0, 1, .5f);
                var startPos = new Vector3(-.2f, .1f * sideFactor, .2f * sideFactor);

                var newThumbDistal = new Vector3
                {
                    X = Math.Clamp(startPos.X + hand.ThumbDistal.Z * MathF.PI * dampener.X, -2, 2),
                    Y = Math.Clamp(startPos.Y + hand.ThumbDistal.Z * MathF.PI * dampener.Y * sideFactor, -2, 2),
                    Z = Math.Clamp(startPos.Z + hand.ThumbDistal.Z * MathF.PI * dampener.Z * sideFactor, -2, 2),
                };

                hand.ThumbDistal = newThumbDistal;
                #endregion
            }
            #endregion

            rigOtherFingerSegment(ref hand.IndexProximal, side);
            rigOtherFingerSegment(ref hand.IndexIntermediate, side);
            rigOtherFingerSegment(ref hand.IndexDistal, side);
            rigOtherFingerSegment(ref hand.MiddleProximal, side);
            rigOtherFingerSegment(ref hand.MiddleIntermediate, side);
            rigOtherFingerSegment(ref hand.MiddleDistal, side);
            rigOtherFingerSegment(ref hand.RingProximal, side);
            rigOtherFingerSegment(ref hand.RingIntermediate, side);
            rigOtherFingerSegment(ref hand.RingDistal, side);
            rigOtherFingerSegment(ref hand.LittleProximal, side);
            rigOtherFingerSegment(ref hand.LittleIntermediate, side);
            rigOtherFingerSegment(ref hand.LittleDistal, side);
        }

        private static void rigOtherFingerSegment(ref Vector3 segment, Side side)
        {
            var isRight = side == Side.Right;
            var sideFactor = isRight ? 1 : -1;

            segment.Z = Math.Clamp(
                segment.Z * -MathF.PI * sideFactor,
                isRight ? -MathF.PI : 0,
                isRight ? 0 : MathF.PI
            );
        }

        private enum HandSegment
        {
            Proximal,
            Intermediate,
            Distral,
        }
    }
}
