// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using Akihabara.Framework.Protobuf;
using Moetion.Extensions;
using static Moetion.Extensions.VectorExtensions;

namespace Moetion.Pose
{
    public static class PoseSolver
    {
        public static Pose Solve(NormalizedLandmarkList list)
        {
            var (leftArm, rightArm) = calcArms(list);

            return new Pose();
        }

        private static (Arm, Arm) calcArms(NormalizedLandmarkList list)
        {
            var landmarks = list.Landmark;

            var rightArm = new Arm();
            var leftArm = new Arm();

            rightArm.Upper = landmarks[11].ToVector().FindRotation(landmarks[13].ToVector());
            leftArm.Upper = landmarks[12].ToVector().FindRotation(landmarks[14].ToVector());
            rightArm.Upper.Y = AngleBetween3DCoords(landmarks[12].ToVector(), landmarks[11].ToVector(), landmarks[13].ToVector());
            leftArm.Upper.Y = AngleBetween3DCoords(landmarks[11].ToVector(), landmarks[12].ToVector(), landmarks[14].ToVector());

            rightArm.Lower = landmarks[13].ToVector().FindRotation(landmarks[15].ToVector());
            leftArm.Lower = landmarks[14].ToVector().FindRotation(landmarks[16].ToVector());
            rightArm.Lower.Y = AngleBetween3DCoords(landmarks[11].ToVector(), landmarks[13].ToVector(), landmarks[15].ToVector());
            leftArm.Lower.Y = AngleBetween3DCoords(landmarks[12].ToVector(), landmarks[14].ToVector(), landmarks[16].ToVector());
            rightArm.Lower.Z = Math.Clamp(rightArm.Lower.Z, -2.14f, 0f);
            leftArm.Lower.Z = Math.Clamp(leftArm.Lower.Z, -2.14f, 0f);

            rightArm.Hand = landmarks[15].ToVector().FindRotation(
                Vector3.Lerp(landmarks[17].ToVector(), landmarks[19].ToVector(), .5f)
            );
            leftArm.Hand = landmarks[16].ToVector().FindRotation(
                Vector3.Lerp(landmarks[18].ToVector(), landmarks[20].ToVector(), .5f)
            );

            // Modify rotations slightly for more natural movement
            rigArm(ref rightArm, Side.Right);
            rigArm(ref leftArm, Side.Left);

            return (leftArm, rightArm);
        }

        private static void rigArm(ref Arm arm, Side side)
        {
            float invert = side == Side.Right ? 1f : -1f;

            arm.Upper.Z *= -2.3f * invert;

            // Modify upper arm's Y rotation by lower arm's X and Z rotations
            arm.Upper.Y *= MathF.PI * invert;
            arm.Upper.Y -= Math.Max(arm.Lower.X, 0);
            arm.Upper.Y -= -invert * Math.Max(arm.Lower.Z, 0);
            arm.Upper.X -= .3f * invert;

            arm.Lower.Z *= -2.14f * invert;
            arm.Lower.Y *= 2.14f * invert;
            arm.Lower.X *= 2.14f * invert;

            // Clamp values to human limits
            arm.Upper.X = Math.Clamp(arm.Upper.X, -.5f, MathF.PI);
            arm.Lower.X = Math.Clamp(arm.Lower.X, -.3f, .3f);

            arm.Hand.Y = Math.Clamp(arm.Hand.Z * 2, -.6f, .6f); // side to side
            arm.Hand.Z = arm.Hand.Z * -2.3f * invert; // up down
        }
    }
}
