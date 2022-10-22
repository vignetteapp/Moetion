// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using Google.Protobuf.Collections;
using Mediapipe.Net.Framework.Protobuf;
using Moetion.Extensions;
using static Moetion.Extensions.MediapipeExtensions;
using static Moetion.Extensions.VectorExtensions;

namespace Moetion.Pose;

public static class PoseSolver
{
    public static Pose Solve(LandmarkList worldLandmarks, NormalizedLandmarkList normLandmarks, bool enableLegs = true)
    {
        (Arm leftArm, Arm rightArm) = calcArms(worldLandmarks);
        (Hips hips, Vector3 spine) = calcHips(worldLandmarks, normLandmarks);
        RepeatedField<Landmark> wlm = worldLandmarks.Landmark;
        RepeatedField<NormalizedLandmark> nlm = normLandmarks.Landmark;

        // Detect offscreen and reset values to defaults
        bool rightHandOffscreen = nlm[15].ToVector().Y > -.1f
                                  || nlm[15].Visibility < .23f
                                  || .995f < nlm[15].ToVector2().Y;
        bool leftHandOffscreen = nlm[16].ToVector().Y > -.1f
                                 || nlm[16].Visibility < .23f
                                 || .995f < nlm[16].ToVector2().Y;
        bool leftFootOffscreen = nlm[23].Visibility < .63f || hips.Position.Z > -.4f;
        bool rightFootOffscreen = nlm[24].Visibility < .63f || hips.Position.Z > -.4f;

        if (leftHandOffscreen)
        {
            leftArm.Upper *= 0;
            leftArm.Upper.Z = RestingDefaults.Pose.LeftArm.Upper.Z;
            leftArm.Hand *= 0;
        }
        if (rightHandOffscreen)
        {
            rightArm.Upper *= 0;
            rightArm.Upper.Z = RestingDefaults.Pose.RightArm.Upper.Z;
            rightArm.Hand *= 0;
        }
        if (leftFootOffscreen)
            leftArm.Lower *= 0;
        if (rightFootOffscreen)
            rightArm.Lower *= 0;

        Pose pose = new Pose
        {
            Hips = hips,
            LeftArm = leftArm,
            RightArm = rightArm,
            Spine = spine,
            LeftLeg = RestingDefaults.Pose.LeftLeg,
            RightLeg = RestingDefaults.Pose.RightLeg,
        };

        // Skip calculations if disable legs
        if (enableLegs)
        {
            (Leg leftLeg, Leg rightLeg) = calcLegs(worldLandmarks);
            leftLeg.Upper *= rightFootOffscreen ? 0 : 1;
            leftLeg.Lower *= rightFootOffscreen ? 0 : 1;
            rightLeg.Upper *= leftFootOffscreen ? 0 : 1;
            rightLeg.Lower *= leftFootOffscreen ? 0 : 1;
            pose.LeftLeg = leftLeg;
            pose.RightLeg = rightLeg;
        }

        return pose;
    }

    private static (Arm, Arm) calcArms(LandmarkList worldLandmarks)
    {
        RepeatedField<Landmark> wlm = worldLandmarks.Landmark;

        Arm rightArm = new Arm();
        Arm leftArm = new Arm();

        rightArm.Upper = wlm[11].FindRotation(wlm[13]);
        leftArm.Upper = wlm[12].FindRotation(wlm[14]);
        rightArm.Upper.Y = wlm.AngleBetweenLandmarks(12, 11, 13);
        leftArm.Upper.Y = wlm.AngleBetweenLandmarks(11, 12, 14);

        rightArm.Lower = wlm[13].FindRotation(wlm[15]);
        leftArm.Lower = wlm[14].FindRotation(wlm[16]);
        rightArm.Lower.Y = wlm.AngleBetweenLandmarks(11, 13, 15);
        leftArm.Lower.Y = wlm.AngleBetweenLandmarks(12, 14, 16);
        rightArm.Lower.Z = Math.Clamp(rightArm.Lower.Z, -2.14f, 0f);
        leftArm.Lower.Z = Math.Clamp(leftArm.Lower.Z, -2.14f, 0f);

        rightArm.Hand = wlm[15].ToVector().FindRotation(
            Vector3.Lerp(wlm[17].ToVector(), wlm[19].ToVector(), .5f)
        );
        leftArm.Hand = wlm[16].ToVector().FindRotation(
            Vector3.Lerp(wlm[18].ToVector(), wlm[20].ToVector(), .5f)
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

    private static (Hips, Vector3) calcHips(LandmarkList worldLandmarks, NormalizedLandmarkList normLandmarks)
    {
        RepeatedField<Landmark> wlm = worldLandmarks.Landmark;
        RepeatedField<NormalizedLandmark> nlm = normLandmarks.Landmark;

        // Find 2D normalized hip and shoulder joint positions / distances
        Vector2 hipLeft2d = nlm[23].ToVector2();
        Vector2 hipRight2d = nlm[24].ToVector2();
        Vector2 shoulderLeft2d = nlm[11].ToVector2();
        Vector2 shoulderRight2d = nlm[12].ToVector2();

        // ...Why is it 1 and not .5f?   ',:v
        Vector2 hipCenter2d = Vector2.Lerp(hipLeft2d, hipRight2d, 1);
        Vector2 shoulderCenter2d = Vector2.Lerp(shoulderLeft2d, shoulderRight2d, 1);
        float spineLength = Vector2.Distance(hipCenter2d, shoulderCenter2d);

        Hips hips = new Hips
        {
            Position = new Vector3
            {
                // subtract .65 to bring closer to 0,0 center
                X = Math.Clamp(-1 * (hipCenter2d.X - .65f), -1, 1),
                Y = 0,
                Z = Math.Clamp(spineLength - 1, -2, 0),
            },
            Rotation = wlm.RollPitchYaw(23, 24),
        };

        // Fix -PI, PI jumping
        if (hips.Rotation.Y > .5f)
            hips.Rotation.Y -= 2;

        hips.Rotation.Y += .5f;

        // Stop jumping between left and right shoulder tilt
        if (hips.Rotation.Z > 0)
            hips.Rotation.Z = 1 - hips.Rotation.Z;

        if (hips.Rotation.Z < 0)
            hips.Rotation.Z = -1 - hips.Rotation.Z;

        float turnAroundAmountHips = Math.Abs(hips.Rotation.Y).Remap(.2f, .4f);
        hips.Rotation.Z *= 1 - turnAroundAmountHips;
        hips.Rotation.X = 0; // Temp fix for inaccurate X axis

        Vector3 spine = wlm.RollPitchYaw(11, 12);

        // fix -PI, PI jumping
        if (spine.Y > .5f)
            spine.Y -= 2;

        spine.Y += .5f;

        // Stop jumping between left and right shoulder tilt
        if (spine.Z > 0)
            spine.Z = 1 - spine.Z;

        if (spine.Z < 0)
            spine.Z = -1 - spine.Z;

        // Fix weird large numbers when 2 shoulder points get too close
        float turnAroundAmount = Math.Abs(spine.Y).Remap(.2f, .4f);
        spine.Z *= 1 - turnAroundAmount;
        spine.X = 0; // Temp fix for inaccurate X axis

        rigHips(ref hips, ref spine);
        return (hips, spine);
    }

    private static void rigHips(ref Hips hips, ref Vector3 spine)
    {
        // Convert normalized values to radians
        hips.Rotation *= MathF.PI;

        hips.WorldPosition = new Vector3
        {
            X = hips.Position.X * (.5f + 1.8f * -hips.Position.Z),
            Y = 0,
            Z = hips.Position.Z * (.1f + hips.Position.Z * -2),
        };

        spine *= MathF.PI;
    }

    // NOTE: Legs are a WIP on the Kalidokit side.
    private static (Leg, Leg) calcLegs(LandmarkList worldLandmarks)
    {
        RepeatedField<Landmark> wlm = worldLandmarks.Landmark;

        Leg leftLeg = new Leg
        {
            Upper = wlm[24].FindRotation(wlm[26]),
            Lower = wlm[26].FindRotation(wlm[28]),
        };
        Leg rightLeg = new Leg
        {
            Upper = wlm[23].FindRotation(wlm[25]),
            Lower = wlm[25].FindRotation(wlm[27]),
        };

        // Recenter
        leftLeg.Upper.Z = Math.Clamp(leftLeg.Upper.Z - .5f, -.5f, 0);
        leftLeg.Upper.Y = 0; // Y axis is not correct
        leftLeg.Lower.X = wlm.AngleBetweenLandmarks(24, 26, 28);
        leftLeg.Lower.Y = 0; // Y axis not correct
        leftLeg.Lower.Z = 0; // Z axis not correct

        rightLeg.Upper.Z = Math.Clamp(rightLeg.Upper.Z - .5f, -.5f, 0);
        rightLeg.Upper.Y = 0; // Y axis is not correct
        rightLeg.Lower.X = wlm.AngleBetweenLandmarks(23, 25, 27);
        rightLeg.Lower.Y = 0; // Y axis not correct
        rightLeg.Lower.Z = 0; // Z axis not correct

        // Modify Rotations slightly for more natural movement
        rigLeg(ref leftLeg, Side.Left);
        rigLeg(ref rightLeg, Side.Right);

        return (leftLeg, rightLeg);
    }

    private static void rigLeg(ref Leg leg, Side side)
    {
        float invert = side == Side.Right ? 1 : -1;
        leg.Upper.Z = leg.Upper.Z * -2.3f * invert;
        leg.Upper.X = Math.Clamp(leg.Upper.Z * .1f * invert, -.5f, MathF.PI);
        leg.Lower.X = leg.Lower.X * -2.14f * 1.3f;
    }
}
