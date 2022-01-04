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

            hand.Ring.Proximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 13, 14));
            hand.Ring.Intermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(13, 14, 15));
            hand.Ring.Distal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(14, 15, 16));

            hand.Index.Proximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 5, 6));
            hand.Index.Intermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(5, 6, 7));
            hand.Index.Distal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(6, 7, 8));

            hand.Middle.Proximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 9, 10));
            hand.Middle.Intermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(9, 10, 11));
            hand.Middle.Distal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(10, 11, 12));

            hand.Thumb.Proximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 1, 2));
            hand.Thumb.Intermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(1, 2, 3));
            hand.Thumb.Distal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(2, 3, 4));

            hand.Little.Proximal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(0, 17, 18));
            hand.Little.Intermediate = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(17, 18, 19));
            hand.Little.Distal = new Vector3(0, 0, landmarks.AngleBetweenLandmarks(18, 19, 20));

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
                        startPos.X + hand.Thumb.Proximal.Z * -MathF.PI * dampener.X,
                        -.6f, .3f
                    ),
                    Y = Math.Clamp(
                        startPos.Y + hand.Thumb.Proximal.Z * -MathF.PI * dampener.Y * sideFactor,
                        isRight ? -1 : -.3f, isRight ? .3f : 1
                    ),
                    Z = Math.Clamp(
                        startPos.Z + hand.Thumb.Proximal.Z * -MathF.PI * dampener.Z * sideFactor,
                        isRight ? -.6f : -.3f, isRight ? .3f : .6f
                    ),
                };

                hand.Thumb.Proximal = newThumbProximal;
                #endregion
            }
            {
                #region Intermediate
                var dampener = new Vector3(0, .7f, .5f);
                var startPos = new Vector3(-.2f, .1f * sideFactor, .2f * sideFactor);

                var newThumbIntermediate = new Vector3
                {
                    X = Math.Clamp(startPos.X + hand.Thumb.Intermediate.Z * MathF.PI * dampener.X, -2, 2),
                    Y = Math.Clamp(startPos.Y + hand.Thumb.Intermediate.Z * MathF.PI * dampener.Y * sideFactor, -2, 2),
                    Z = Math.Clamp(startPos.Z + hand.Thumb.Intermediate.Z * MathF.PI * dampener.Z * sideFactor, -2, 2),
                };

                hand.Thumb.Intermediate = newThumbIntermediate;
                #endregion
            }
            {
                #region Distal
                var dampener = new Vector3(0, 1, .5f);
                var startPos = new Vector3(-.2f, .1f * sideFactor, .2f * sideFactor);

                var newThumbDistal = new Vector3
                {
                    X = Math.Clamp(startPos.X + hand.Thumb.Distal.Z * MathF.PI * dampener.X, -2, 2),
                    Y = Math.Clamp(startPos.Y + hand.Thumb.Distal.Z * MathF.PI * dampener.Y * sideFactor, -2, 2),
                    Z = Math.Clamp(startPos.Z + hand.Thumb.Distal.Z * MathF.PI * dampener.Z * sideFactor, -2, 2),
                };

                hand.Thumb.Distal = newThumbDistal;
                #endregion
            }
            #endregion

            rigOtherFingerSegment(ref hand.Index.Proximal, side);
            rigOtherFingerSegment(ref hand.Index.Intermediate, side);
            rigOtherFingerSegment(ref hand.Index.Distal, side);
            rigOtherFingerSegment(ref hand.Middle.Proximal, side);
            rigOtherFingerSegment(ref hand.Middle.Intermediate, side);
            rigOtherFingerSegment(ref hand.Middle.Distal, side);
            rigOtherFingerSegment(ref hand.Ring.Proximal, side);
            rigOtherFingerSegment(ref hand.Ring.Intermediate, side);
            rigOtherFingerSegment(ref hand.Ring.Distal, side);
            rigOtherFingerSegment(ref hand.Little.Proximal, side);
            rigOtherFingerSegment(ref hand.Little.Intermediate, side);
            rigOtherFingerSegment(ref hand.Little.Distal, side);
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
    }
}
