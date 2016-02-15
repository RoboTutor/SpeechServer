using System;
using System.Collections.Generic;
using System.Diagnostics;
// MTL
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfilePointingOmni : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "PointingOmni"; }
        }

        string Direction = "Omni";

        /// <summary>
        /// Pointing direction.
        /// </summary>
        /// <param name="x">Pointing direction X</param>
        /// <param name="y">Pointing direction Y</param>
        /// <param name="z">Pointing direction Z</param>
        public BehaviorProfilePointingOmni(double x, double y, double z)
        {
            this.TargetX = x;
            this.TargetY = y;
            this.TargetZ = z;

            GenerateSeeds();

            this.Parameters.Add("PalmUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(new List<double>(), "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
        }

        private void GenerateSeeds()
        {
            double dist = Math.Sqrt(TargetX * TargetX + TargetY * TargetY + TargetZ * TargetZ);
            double alpha = Math.Acos(TargetX / dist);
            double beta = Math.Acos(TargetY / dist);
            if (beta > Math.PI / 2) beta -= Math.PI;
            double gamma = Math.Acos(TargetZ / dist);
            if (gamma > Math.PI / 2) gamma -= Math.PI;

            if (alpha<=Math.PI/6 && alpha>=0) // forward
            {
                SeedShldPtch = 0.785; // neg 1.1  pos 0.26 
                SeedShldRoll = 0.19;  // neg 0.12 pos 0.26
                Direction = "Forward";
            }
            else if (beta >= -Math.PI / 6 && beta <= 0) // right/screen
            {
                SeedShldPtch = 1.24; // neg 1.88 pos 0.6
                SeedShldRoll = -0.74; // neg -0.68 pos -0.8 
                Direction = "Right";
            }
            else if (gamma <= Math.PI / 6 && gamma >= 0) // up/sky
            {
                SeedShldPtch = -1.5;
                SeedShldRoll = -0.5;
                Direction = "Up";
            }
            else if (gamma >= -Math.PI / 6 && gamma <= 0) // down/ground
            {
                SeedShldPtch = 1.0;
                SeedShldRoll = -1.0;
                Direction = "Down";
            }
            else if (alpha > 0 && beta < 0 && gamma > 0) // right up
            {
                SeedShldPtch = -0.273; // pos -0.855 neg 0.31
                SeedShldRoll = -0.63; // pos -0.54  neg -0.72 
                Direction = "RightUp";
            }
            else if (alpha > 0 && beta < 0 && gamma < 0) // right down
            {
                SeedShldPtch = 1.5;
                SeedShldRoll = -0.5;
                Direction = "RightDown";
            }
            else if (beta > 0 && gamma < 0) // Forward down
            {
                SeedShldPtch = 1.1; // pos 0.67 neg 1.5
                SeedShldRoll = 0.27; // pos 0.314 neg 0.22
                Direction = "ForwardDown";
            }
            else if (beta > 0 && gamma > 0) // Forward up
            {
                SeedShldPtch = -0.8;
                SeedShldRoll = 0.314;
                Direction = "ForwardUp";
            }
            else
            {
                Debug.WriteLine("Abnormal pointing direction!");
            }
        }

        public override MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        public override MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        #region Core
        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        // Pointing direction - Target
        double TargetX = 400D;   
        double TargetY = 0D;
        double TargetZ = 0D;
        //
        double SeedShldPtch = 0.25;
        double SeedShldRoll = 0;
        double SeedElbYaw  = 1.5;
        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double openness        = Parameters["PalmUpDown"].Value;
            double fingerrigidness = Parameters["Finger"].Value;
            double amplitude       = Parameters["Amplitude"].Value;
            double headupdown      = Parameters["HeadUpDown"].Value;
            double headleftright   = Parameters["HeadLeftRight"].Value;

            double hand = 0.01 * fingerrigidness + 0.9; // *180D / Math.PI;
            double amp = amplitude * 218.666;
            double elbowRoll = Math.Acos((amp * amp - Math.Pow(105D, 2D) - Math.Pow(57.75 + 55.95, 2D)) / (2D * 105D * (55.95 + 57.75)));
            elbowRoll *= 180D / Math.PI;

            double wristYaw;
            if (this.Direction == "Up")
            {
                wristYaw = -70;
                    //-(10D * openness * openness + 80D * openness + 10D);
            } 
            else
            {
            	wristYaw = 10D * openness * openness + 80D * openness + 10D;
            }

            // Determine Shoulder Pitch, Roll
            // Set to 80 to make the "wristYaw" able to reach "PalmUp" and "PalmDown"
            //double elbowYaw = 80D; 
            //Microsoft.FSharp.Math.Vector<double> result
            //    = NaoJointChain.Arm.OptPntTo2JSeed(
            //    elbowRoll * Math.PI / 180D, 
            //    elbowYaw * Math.PI / 180D, 
            //    TargetX, TargetY, TargetZ,
            //    SeedShldPtch, SeedShldRoll);
            Microsoft.FSharp.Math.Vector<double> result
                = NaoJointChain.Arm.OptPntTo3JSeed(
                elbowRoll * Math.PI / 180D,
                TargetX, TargetY, TargetZ,
                SeedShldPtch, SeedShldRoll, SeedElbYaw);
            double shoulderPitch = result[0] * 180D / Math.PI;
            double shoulderRoll = result[1] * 180D / Math.PI;
            double elbowYaw = result[2] * 180D / Math.PI;

            //////////////////////////////////////////////////////////////////////////
            double headpitch, headyaw;
            /*
            if (pp.Valence >= 0)
            {
                Microsoft.FSharp.Math.Vector<double> result1
                          = NaoJointChain.Head.LookAt(targetX, targetY, targetZ);
                headpitch = result1[0] * 180 / Math.PI;
                headyaw = result1[1] * 180 / Math.PI;
            }
            else
            {
                Microsoft.FSharp.Math.Vector<double> result1
                          = NaoJointChain.Head.DistractFrom(targetX, targetY, targetZ);
                headpitch = result1[0] * 180 / Math.PI;
                headyaw = result1[1] * 180 / Math.PI;
            }*/

            // degree
            headpitch = base.NormalizeHeadPitch(headupdown);
            // degree
            if (this.Direction.Contains("Right"))
            {
                headyaw = -base.NormalizeHeadYaw(headleftright);
            }
            else headyaw = 0;

            //double initSpeed = 0.3;
            // pose
            double shoulderPitch_1 = 20;
            double shoulderRoll_1 = -10;
            double elbowYaw_1 = 60;
            double elbowRoll_1 = 88;
            double wristYaw_1 = -20;
            double hand_1 = 0.6; // *180 / (double)Math.PI;
            //double headpitch_1 = 0;
            //double headyaw_1 = 0;      

            // Initial Frame
            RArm initArm = new RArm(false,
                    shoulderPitch_1,
                    shoulderRoll_1,
                    elbowYaw_1,
                    elbowRoll_1,
                    wristYaw_1,
                    hand_1);
            Head head = new Head(false, headpitch, headyaw);
            RArm endArm = new RArm(false,
                    shoulderPitch,
                    shoulderRoll,
                    elbowYaw,
                    elbowRoll,
                    wristYaw,
                    hand);

            PoseProfile initArmPose = new PoseProfile("InitPose", initArm, true, 0.5);
            PoseProfile initHeadPose = new PoseProfile("InitPose", head);
            PoseProfile endArmPose = new PoseProfile("EndPose", endArm, true, 1.0, 0.5);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(initArmPose);
            lpp.Add(initHeadPose);
            lpp.Add(endArmPose);

            return lpp;
        }



        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double factor = 1.3;
            double motionspeed = factor * (0.0015 * valence * valence + 0.04 * valence + 0.4);
            double decaySpd = motionspeed - 0.04;
            double initHoldTime = 0.5;
            double endHoldTime = 0.045 * valence + 0.55;
            double rep = 0;
            
            // pose
            double amplitude = 0.007 * valence + 0.93;
            double openness = valence / 10D;
            double rigidness = 0.05 * valence + 0.5;
            double headupdown = 0.1 * valence;
            double headlr;
            if (valence > 0) headlr = 0.1 * valence;
            else headlr = 0;

            Parameters["PalmUpDown"].Value = openness;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["Finger"].Value = rigidness;
            Parameters["HeadUpDown"].Value = headupdown;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value = motionspeed;
            Parameters["DecaySpeed"].Value = decaySpd;
            Parameters["HoldTime"].ValueList.Add(initHoldTime);
            Parameters["HoldTime"].ValueList.Add(endHoldTime);
            Parameters["Repetition"].Value = rep;
        }
        #endregion Core

    } // class PointingBehavior
}
