// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using Google.Protobuf.Collections;
using Mediapipe.Net.Framework.Protobuf;
using Moetion.Extensions;
using static Moetion.Extensions.NumberExtensions;
using static Moetion.Extensions.VectorExtensions;

namespace Moetion.Face;

public static class FaceSolver
{
    public static readonly int[] EyeLeftPoints = new int[] { 130, 133, 160, 159, 158, 144, 145, 153 };
    public static readonly int[] EyeRightPoints = new int[] { 263, 362, 387, 386, 385, 373, 374, 380 };
    public static readonly int[] BrowLeftPoints = new int[] { 35, 244, 63, 105, 66, 229, 230, 231 };
    public static readonly int[] BrowRightPoints = new int[] { 265, 464, 293, 334, 296, 449, 450, 451 };
    public static readonly int[] PupilLeftPoints = new int[] { 468, 469, 470, 471, 472 };
    public static readonly int[] PupilRightPoints = new int[] { 473, 474, 475, 476, 477 };

    /// <summary>
    /// Combines head, eye, pupil, and eyebrow calcs into one method to solve for the entire face.
    /// </summary>
    /// <remarks>
    /// Here I chose the Mediapipe runtime values. We'll have to see if we want both runtimes.
    /// </remarks>
    public static Face Solve(
        NormalizedLandmarkList list,
        bool smoothBlink = false,
        float blinkHigh = .35f, /* .85f if runtime is Tensorflow */
        float blinkLow = .5f    /* .55f if runtime is Tensorflow */
    )
    {
        Head head = CalcHead(list);
        Mouth mouth = CalcMouth(list);

        Eyes eyes = CalcEyes(list, blinkHigh, blinkLow);

        if (smoothBlink)
            StabilizeBlink(ref eyes, head.Y);

        Vector2 pupils = CalcPupils(list);
        float brow = CalcBrow(list);

        return new Face
        {
            Head = head,
            Eyes = eyes,
            Brow = brow,
            Pupils = pupils,
            Mouth = mouth,
        };
    }

    #region Mouth Calculations
    public static Mouth CalcMouth(NormalizedLandmarkList list)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        // Eye keypoints
        Vector3 eyeInnerCornerL = landmarks[133].ToVector();
        Vector3 eyeInnerCornerR = landmarks[362].ToVector();
        Vector3 eyeOuterCornerL = landmarks[130].ToVector();
        Vector3 eyeOuterCornerR = landmarks[263].ToVector();

        // Eye keypoint distances
        float eyeInnerDistance = Vector3.Distance(eyeInnerCornerL, eyeInnerCornerR);
        float eyeOuterDistance = Vector3.Distance(eyeOuterCornerL, eyeOuterCornerR);

        // Mouth keypoints
        Vector3 upperInnerLip = landmarks[13].ToVector();
        Vector3 lowerInnerLip = landmarks[14].ToVector();
        Vector3 mouthCornerLeft = landmarks[61].ToVector();
        Vector3 mouthCornerRight = landmarks[291].ToVector();

        // Mouth keypoint distances
        float mouthOpen = Vector3.Distance(upperInnerLip, lowerInnerLip);
        float mouthWidth = Vector3.Distance(mouthCornerLeft, mouthCornerRight);

        // Mouth open and mouth shape ratios
        float ratioY = mouthOpen / eyeInnerDistance;
        float ratioX = mouthWidth / eyeOuterDistance;

        // Normalize and scale mouth open
        ratioY = ratioY.Remap(0.15f, 0.7f);

        // Normalize and scale mouth shape
        ratioX = ratioX.Remap(0.45f, 0.9f);
        ratioX = (ratioX - 0.3f) * 2;

        float mouthX = ratioX;
        float mouthY = (mouthOpen / eyeInnerDistance).Remap(0.17f, 0.5f);

        float ratioI = Math.Clamp(mouthX.Remap(0, 1) * 2 * mouthY.Remap(0.2f, 0.7f), 0, 1);
        float ratioA = mouthY * 0.4f + mouthY * (1 - ratioI) * 0.6f;
        float ratioU = mouthY * (1 - ratioI).Remap(0, 0.3f) * 0.1f;
        float ratioE = ratioU.Remap(0.2f, 1) * (1 - ratioI) * 0.3f;
        float ratioO = (1 - ratioI) * mouthY.Remap(0.3f, 1) * 0.4f;

        return new Mouth
        {
            X = ratioX,
            Y = ratioY,
            Shape = new Phoneme
            {
                A = ratioA,
                E = ratioE,
                I = ratioI,
                O = ratioO,
                U = ratioU,
            },
        };
    }
    #endregion

    #region Head Calculations
    public static Head CalcHead(NormalizedLandmarkList list)
    {
        // Find 3 vectors that form a plane to represent the head
        Vector3[] plane = FaceEulerPlane(list);
        Vector3 rotate = RollPitchYaw(plane[0], plane[1], plane[2]);
        // Find center of face detection box
        Vector3 midPoint = Vector3.Lerp(plane[0], plane[1], 0.5f);
        // Roughly find the dimensions of the face detection box
        float width = Vector3.Distance(plane[0], plane[1]);
        float height = Vector3.Distance(midPoint, plane[2]);

        // Flip
        rotate.X *= -1;
        rotate.Y *= -1;

        return new Head
        {
            X = rotate.X * MathF.PI,
            Y = rotate.Y * MathF.PI,
            Z = rotate.Z * MathF.PI,
            Width = width,
            Height = height,
            Position = Vector3.Lerp(midPoint, plane[2], 0.5f),
            NormalizedAngles = new Vector3
            {
                X = rotate.X,
                Y = rotate.Y,
                Z = rotate.Z,
            },
            Degrees = new Vector3
            {
                X = rotate.X * 180,
                Y = rotate.Y * 180,
                Z = rotate.Z * 180,
            },
        };
    }

    public static Vector3[] FaceEulerPlane(NormalizedLandmarkList list)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        // Create face detection square bounds
        Vector3 topLeft = landmarks[21].ToVector();
        Vector3 topRight = landmarks[251].ToVector();
        Vector3 bottomRight = landmarks[397].ToVector();
        Vector3 bottomLeft = landmarks[172].ToVector();
        Vector3 bottomMidpoint = Vector3.Lerp(bottomRight, bottomLeft, 0.5f);

        // TODO: idk, this array processing looks ugly.
        return new Vector3[] { topLeft, topRight, bottomMidpoint };
    }
    #endregion

    #region Eye Calculations
    public static float GetEyeOpen(NormalizedLandmarkList list, Side side, float high = .85f, float low = .55f)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        int[] eyePoints = side == Side.Right ? EyeRightPoints : EyeLeftPoints;
        float eyeDistance = EyeLidRatio(
            landmarks[eyePoints[0]],
            landmarks[eyePoints[1]],
            landmarks[eyePoints[2]],
            landmarks[eyePoints[3]],
            landmarks[eyePoints[4]],
            landmarks[eyePoints[5]],
            landmarks[eyePoints[6]],
            landmarks[eyePoints[7]]
        );

        // Human eye width to height ratio is roughly .3
        float maxRatio = 0.285f;
        // Compare ratio against max ratio
        float ratio = Math.Clamp(eyeDistance / maxRatio, 0, 2);
        // Remap eye open and close ratios to increase sensitivity
        float eyeOpenRatio = ratio.Remap(low, high);

        return eyeOpenRatio;
    }

    public static float EyeLidRatio(
        NormalizedLandmark outerCorner,
        NormalizedLandmark innerCorner,
        NormalizedLandmark outerUpperLid,
        NormalizedLandmark midUpperLid,
        NormalizedLandmark innerUpperLid,
        NormalizedLandmark outerLowerLid,
        NormalizedLandmark midLowerLid,
        NormalizedLandmark innerLowerLid)
    {
        Vector2 eyeOuterCorner = outerCorner.ToVector2();
        Vector2 eyeInnerCorner = innerCorner.ToVector2();

        Vector2 eyeOuterUpperLid = outerUpperLid.ToVector2();
        Vector2 eyeMidUpperLid = midUpperLid.ToVector2();
        Vector2 eyeInnerUpperLid = innerUpperLid.ToVector2();

        Vector2 eyeOuterLowerLid = outerLowerLid.ToVector2();
        Vector2 eyeMidLowerLid = midLowerLid.ToVector2();
        Vector2 eyeInnerLowerLid = innerLowerLid.ToVector2();

        // Use 2D Distances instead of 3D for less jitter
        float eyeWidth = Vector2.Distance(eyeOuterCorner, eyeInnerCorner);
        float eyeOuterLidDistance = Vector2.Distance(eyeOuterUpperLid, eyeOuterLowerLid);
        float eyeMidLidDistance = Vector2.Distance(eyeMidUpperLid, eyeMidLowerLid);
        float eyeInnerLidDistance = Vector2.Distance(eyeInnerUpperLid, eyeInnerLowerLid);
        float eyeLidAvg = (eyeOuterLidDistance + eyeMidLidDistance + eyeInnerLidDistance) / 3;
        float ratio = eyeLidAvg / eyeWidth;

        return ratio;
    }

    /// <summary>
    /// Calculates pupil position [-1, 1].
    /// </summary>
    public static Vector2 PupilPos(NormalizedLandmarkList list, Side side)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        int[] eyePoints = side == Side.Right ? EyeRightPoints : EyeLeftPoints;
        Vector3 eyeOuterCorner = landmarks[eyePoints[0]].ToVector();
        Vector3 eyeInnerCorner = landmarks[eyePoints[1]].ToVector();
        float eyeWidth = Vector2.Distance(eyeOuterCorner.ToVector2(), eyeInnerCorner.ToVector2());
        Vector3 midPoint = Vector3.Lerp(eyeOuterCorner, eyeInnerCorner, .5f);

        int[] pupilPoints = side == Side.Right ? PupilRightPoints : PupilLeftPoints;
        Vector3 pupil = landmarks[pupilPoints[0]].ToVector();
        float dx = midPoint.X - pupil.X;
        float dy = midPoint.Y - pupil.Y - eyeWidth * .075f;

        float ratioX = 4 * dx / (eyeWidth / 2);
        float ratioY = 4 * dy / (eyeWidth / 4);

        return new Vector2(ratioX, ratioY);
    }

    public static void StabilizeBlink(ref Eyes eyes, float headY, bool enableWink = true, float maxRotation = .5f)
    {
        eyes.Left = Math.Clamp(eyes.Left, 0, 1);
        eyes.Right = Math.Clamp(eyes.Right, 0, 1);

        // Difference between each eye
        float blinkDiff = MathF.Abs(eyes.Left - eyes.Right);
        // Threshold to which difference is considered a wink
        float blinkThresh = enableWink ? .8f : 1.2f;

        bool isClosing = eyes.Left < .3f && eyes.Right < .3f;
        bool isOpening = eyes.Left > .6f && eyes.Right > .6f;

        // Sets obstructed eye to the opposite eye value
        if (headY > maxRotation)
        {
            eyes.Left = eyes.Right;
            return;
        }
        if (headY < -maxRotation)
        {
            eyes.Right = eyes.Left;
            return;
        }

        // Wink of averaged blink values
        if (!(blinkDiff >= blinkThresh && !isClosing && !isOpening))
        {
            float value = Lerp(eyes.Right, eyes.Left, eyes.Right > eyes.Left ? .95f : .05f);
            eyes.Left = value;
            eyes.Right = value;
        }
    }

    /// <summary>
    /// Calculate eyes.
    /// </summary>
    public static Eyes CalcEyes(NormalizedLandmarkList list, float high = .85f, float low = .55f)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        // Return early if no iris tracking
        if (landmarks.Count != 478)
        {
            return new Eyes
            {
                Left = 1,
                Right = 1,
            };
        }

        // Open [0, 1]
        return new Eyes
        {
            Left = GetEyeOpen(list, Side.Left, high, low),
            Right = GetEyeOpen(list, Side.Right, high, low),
        };
    }

    /// <summary>
    /// Calculate pupil location normalized to eye bounds
    /// </summary>
    public static Vector2 CalcPupils(NormalizedLandmarkList list)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        // Pupil (x: [-1, 1], y: [-1, 1])
        if (landmarks.Count != 478)
        {
            return new Vector2(0, 0);
        }

        // Track pupils using left eye
        Vector2 pupilLeft = PupilPos(list, Side.Left);
        Vector2 pupilRight = PupilPos(list, Side.Right);

        return (pupilLeft + pupilRight) * .5f;
    }

    /// <summary>
    /// Calculate brow raise
    /// </summary>
    public static float GetBrowRaise(NormalizedLandmarkList list, Side side)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        int[] browPoints = side == Side.Right ? BrowRightPoints : BrowLeftPoints;
        float browDistance = EyeLidRatio(
            landmarks[browPoints[0]],
            landmarks[browPoints[1]],
            landmarks[browPoints[2]],
            landmarks[browPoints[3]],
            landmarks[browPoints[4]],
            landmarks[browPoints[5]],
            landmarks[browPoints[6]],
            landmarks[browPoints[7]]
        );

        float maxBrowRatio = 1.15f;
        float browHigh = .125f;
        float browLow = .07f;
        float browRatio = browDistance / maxBrowRatio - 1;
        float browRaiseRatio = (Math.Clamp(browRatio, browLow, browHigh) - browLow) / (browHigh - browLow);

        return browRaiseRatio;
    }

    /// <summary>
    /// Take the average of left and right eyebrow raise values
    /// </summary>
    public static float CalcBrow(NormalizedLandmarkList list)
    {
        RepeatedField<NormalizedLandmark> landmarks = list.Landmark;

        if (landmarks.Count != 478)
            return 0;

        float leftBrow = GetBrowRaise(list, Side.Left);
        float rightBrow = GetBrowRaise(list, Side.Right);

        return (leftBrow + rightBrow) / 2;
    }
    #endregion
}
