using System.Numerics;

namespace Moetion.Face
{
    public struct FaceData
    {
        public Vector3 eyeInnerCornerL;
        public Vector3 eyeInnerCornerR;
        public Vector3 eyeOuterCornerL;
        public Vector3 eyeOuterCornerR;
        
        public double eyeInnerDistance;
        public double eyeOuterDistance;
        
        public Vector3 upperInnerLip;
        public Vector3 lowerInnerLip;
        public Vector3 mouthCornerLeft;
        public Vector3 mouthCornerRight;
        
        public double mouthOpen;
        public double mouthWidth;

        public double ratioX;
        public double ratioY;
    }
}