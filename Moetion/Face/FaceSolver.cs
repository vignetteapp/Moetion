using System.Collections;
using Akihabara;
using System.Numerics;
using Akihabara.Framework.Protobuf;
using Moetion.Extensions;

namespace Moetion.Face
{
    public sealed class FaceSolver
    {
        public FaceData Solve(NormalizedLandmarkList list)
        {
            FaceData data = new FaceData();
            
            //eye keypoints
            data.eyeInnerCornerL = list.Landmark[133].ToVector();
            data.eyeInnerCornerR = list.Landmark[362].ToVector();
            data.eyeOuterCornerL = list.Landmark[130].ToVector();
            
            //eye keypoint distances
            data.eyeInnerDistance = data.eyeInnerCornerL.Distance(data.eyeInnerCornerR);
            data.eyeOuterDistance = data.eyeOuterCornerL.Distance(data.eyeOuterCornerR);
            
            //mouth keypoints
            data.upperInnerLip = list.Landmark[13].ToVector();
            data.lowerInnerLip = list.Landmark[14].ToVector();
            data.mouthCornerLeft = list.Landmark[61].ToVector();
            data.mouthCornerRight = list.Landmark[291].ToVector();
            
            //mouth keypoint distances
            data.mouthOpen = data.upperInnerLip.Distance(data.lowerInnerLip);
            data.mouthWidth = data.mouthCornerLeft.Distance(data.mouthCornerRight);
            
            //mouth open and shape ratios

            return data;
        }
    }
}