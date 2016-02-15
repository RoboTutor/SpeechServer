using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao; // MTL

namespace NaoManager
{
    public class BehaviorProfilePointing : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Pointing"; }
        }

        List<double> Neutral;
        public BehaviorProfilePointing()
        {
            this.Parameters.Add("PalmUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
            
            //InitValues = new List<double>() { 0, 0.8, 0.9, 0, 2.0, 0.25, 0.25, 0, 0 };
            Neutral = new List<double>() { -0.45, 0.84, 0.90, 0, 2.4, 0.23, 0.27, 0, 0 };
            //LowerBounds = new List<double>() { -1, 0.6, 0.8, 0, 0.0, 0.02, 0.02, -5, -10 };
            //UpperBounds = new List<double>() { 1, 1.0, 1.0, 5, 5.0, 0.50, 0.50, 7, 10 };
            //LBName = new List<string>() { "down", "flexible", "small", "0", "short", "slow", "slow", "down", "left" };
            //UBName = new List<string>() { "up", "rigid", "large", "5", "long(sec)", "fast(%)", "fast(%)", "up", "right" };
            //DecimalPlaces = new List<int>() { 2, 2, 2, 0, 1, 2, 2, 1, 1 };
            

            // TODO: presets load from XML files
            #region Preset
            //List<string> PresetName = new List<string>() { "neutral", "madaggressive", "angry", "sad", "veryunhappy", "elated", "happy", "pleased", "veryhappy" };
            List<string> PresetName = new List<string>() { "neutral", "angry", "sad", "veryunhappy", "happy", "veryhappy" };

            //List<double> neutral       = new List<double>() {  0.0, 0.80, 0.90, 0, 2.4, 0.25, 0.25,  0,  0 };
            List<double> madaggressive = new List<double>() { -1.0, 0.92, 0.98, 2, 1.6, 0.40, 0.45, -5, 0 };
            //List<double> angry         = new List<double>() { -1.0, 0.85, 0.94, 1, 2.4, 0.30, 0.35,  0,  0 };
            //List<double> sad           = new List<double>() { -0.5, 0.70, 0.86, 0, 2.4, 0.10, 0.15, -3,  3 };
            //List<double> veryunhappy   = new List<double>() { -1.0, 0.59, 0.82, 0, 0.8, 0.05, 0.10, -5,  6 };
            List<double> elated = new List<double>() { -0.2, 1.00, 0.98, 4, 0.8, 0.30, 0.45, 5, -2 };
            //List<double> happy         = new List<double>() { -0.2, 0.96, 0.94, 1, 2.4, 0.30, 0.35,  3,  0 };
            List<double> pleased = new List<double>() { -0.2, 0.88, 0.94, 0, 2.9, 0.30, 0.35, 3, 0 };
            //List<double> veryhappy     = new List<double>() { -0.2, 1.00, 0.98, 1, 4.0, 0.40, 0.45,  5, -2 };

            // final
            List<double> angry = new List<double>() { -1.00, 1.00, 1.00, 2, 0.8, 0.43, 0.47, 0, 3 };
            List<double> sad = new List<double>() { -0.45, 0.76, 0.86, 0, 2.0, 0.13, 0.17, -3, 3 };
            List<double> veryunhappy = new List<double>() { -0.75, 0.68, 0.82, 0, 0.8, 0.05, 0.09, -5, 6 };
            List<double> happy = new List<double>() { -0.25, 0.92, 0.94, 1, 2.8, 0.33, 0.37, 3, 0 };
            List<double> veryhappy = new List<double>() { -0.15, 1.00, 1.00, 2, 1.6, 0.43, 0.47, 5, -3 };

            // Individual parameter levels
            List<double> palmupdown = new List<double>() { -1.0, -0.5, -0.2, 0.0 };
            List<double> fingerrigidness = new List<double>() { 0.59, 0.70, 0.80, 0.85, 0.88, 0.92, 0.96, 1.00 };
            List<double> amplitude = new List<double>() { 0.82, 0.86, 0.90, 0.94, 0.98 };
            List<double> repetition = new List<double>() { 0, 1, 2, 4 };
            List<double> holdtime = new List<double>() { 0.8, 1.6, 2.4, 2.9, 4.0 };
            List<double> decayspeed = new List<double>() { 0.05, 0.10, 0.25, 0.30, 0.40 };
            List<double> pointingspeed = new List<double>() { 0.10, 0.15, 0.25, 0.35, 0.45 };
            List<double> headupdown = new List<double>() { -5, -3, 0, 3, 5 };
            List<double> headleftright = new List<double>() { -2, 0, 3, 6 };



            /// Preset
            /// ------------------------------------PalmUD FngRgd Ampltd Repeat HoldTm DecSpd PntSpd HeadUD HeadLR	
            ///                                     
            //List<double> P0 = new List<double>() { -0.83, 0.73, 0.82, 0, 0.37, 0.13, 0.15, -4.6, 3.0 };
            //List<double> P1 = new List<double>() { -0.67, 0.84, 0.84, 0, 0.46, 0.17, 0.19, -4.0, 1.5 };
            //List<double> P2 = new List<double>() { 0.16, 0.95, 0.93, 0, 0.82, 0.26, 0.30, 3.0, -2.8 };
            //List<double> P3 = new List<double>() { 0.67, 0.98, 0.96, 0, 0.93, 0.34, 0.40, 5.0, -3.4 };
            //List<double> P4 = new List<double>() { 0.87, 1.00, 1.00, 1, 1.00, 0.37, 0.47, 6.2, -3.8 };
            // !!! below used somewhere !!!
            //List<double> P0 = new List<double>() { -0.93, 0.69, 0.82, 0, 0.14, 0.08, 0.13, -4.4, 4.0 };
            //List<double> P1 = new List<double>() { -0.54, 0.74, 0.86, 0, 0.29, 0.14, 0.19, -2.9, 2.9 };
            //List<double> P2 = new List<double>() { 0.25, 0.86, 0.93, 0, 0.69, 0.25, 0.32, 2.6, -2.6 };
            //List<double> P3 = new List<double>() { 0.64, 0.92, 0.96, 0, 0.94, 0.33, 0.40, 4.2, -3.2 };
            //List<double> P4 = new List<double>() { 0.98, 1.00, 1.00, 1, 1.14, 0.39, 0.48, 5.9, -3.7 };
            #endregion Presets

            //PresetParams.Add(-10, P0);
            //PresetParams.Add(-5, P1);
            //PresetParams.Add(0, P2);
            //PresetParams.Add(5, P3);
            //PresetParams.Add(10, P4);

            // be consistent with "PresetName" 
            // "neutral", "madaggressive", "angry", "sad", "veryunhappy", "elated", "happy", "pleased", "veryhappy"
            Presets.Add("neutral", Neutral); // 0
            Presets.Add("madaggressive", madaggressive);
            Presets.Add("angry", angry);
            Presets.Add("sad", sad);
            Presets.Add("veryunhappy", veryunhappy);
            Presets.Add("elated", elated);
            Presets.Add("happy", happy);
            Presets.Add("pleased", pleased);
            Presets.Add("veryhappy", veryhappy);

            ParamLvls.Add("palmupdown", palmupdown);
            ParamLvls.Add("fingerrigidness", fingerrigidness);
            ParamLvls.Add("amplitude", amplitude);
            ParamLvls.Add("repetition", repetition);
            ParamLvls.Add("holdtime", holdtime);
            ParamLvls.Add("decayspeed", decayspeed);
            ParamLvls.Add("pointingspeed", pointingspeed);
            ParamLvls.Add("headupdown", headupdown);
            ParamLvls.Add("headleftright", headleftright);
        }

        public override MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();

            // For an experiment
            //    List<bool> ImportanceMasks = new List<bool>() { false, false, true, false, false, false, true, true, false };

            //    if (setmode == 0) // all parameters
            //    {
            //        return CoreParameterToMotion(Presets[setname]);
            //    }
            //    else if (setmode == 1) // return "minimum set"
            //    {
            //        List<double> preset_params = Presets[setname];
            //        List<double> preset_params_minimumset = new List<double>();
            //        for (int i = 0; i < preset_params.Count; i++)
            //        {
            //            if (ImportanceMasks[i]) preset_params_minimumset.Add(preset_params[i]);
            //            else preset_params_minimumset.Add(Presets["neutral"][i]);
            //        }
            //        return CoreParameterToMotion(preset_params_minimumset);
            //    }
            //    else if (setmode == 2) // less important parameters
            //    {
            //        List<double> preset_params = Presets[setname];
            //        List<double> preset_params_minimumset = new List<double>();
            //        for (int i = 0; i < preset_params.Count; i++)
            //        {
            //            if (ImportanceMasks[i] == false) preset_params_minimumset.Add(preset_params[i]);
            //            else preset_params_minimumset.Add(Presets["neutral"][i]);
            //        }
            //        return CoreParameterToMotion(preset_params_minimumset);
            //    }
            //    else
            //    {
            //        // exception
            //        return null;
            //    }
        }

        public override MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        //    List<double> single_modulated_params = new List<double>(Neutral);
        //    List<double> lvls = (ParamLvls.Values.ToList<List<double>>())[paramind];
        //    single_modulated_params[paramind] = lvls[level];
        //    return CoreParameterToMotion(single_modulated_params);
        }

        #region Core
        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            //////////////////////////////////////////////////////////////////////////
            int rep = 0;
            double amplitude = 0;

            // Speed
            double motionspeed = 0.0015 * valence * valence + 0.04 * valence + 0.4;

            // General
            double endHoldTime = 3.0;
            double decaySpd = motionspeed - 0.04;

            //
            if (valence >= -8.0 && valence <= 8.0)
            {
                amplitude = 0.0125 * valence + 0.9;
            }
            else if (valence > 8.0 && valence <= 10.0)
            {
                amplitude = 1D;
                if (valence <= 9.0)
                    rep = 1;
                else if (valence > 9.0)
                    rep = 2;
            }
            else if (valence < -8.0 && valence >= -10.0)
            {
                amplitude = 0.8D;
                endHoldTime = -0.3 * valence * valence - 4.9 * valence - 19D;
                //decaySpd = -0.25 * v - 1.5;
            }
            //
            double openness = valence / 10D;
            double rigidness = -0.001 * valence * valence + 0.02 * valence + 0.9;

            //////////////////////////////////////////////////////////////////////////
            // Accompanying
            double headupdown;
            if (valence >= 0) headupdown = 0.7 * valence;
            else headupdown = 0.5 * valence;
            double headdist = -valence;
            //////////////////////////////////////////////////////////////////////////

            Parameters["PalmUpDown"].Value = openness;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["Finger"].Value = rigidness;
            Parameters["HeadUpDown"].Value = headupdown;
            Parameters["HeadLeftRight"].Value = headdist;
            Parameters["MotionSpeed"].Value = motionspeed;
            Parameters["DecaySpeed"].Value = decaySpd;
            Parameters["HoldTime"].Value = endHoldTime;
            Parameters["Repetition"].Value = rep;
        }

        // motion parameters
        protected override MotionTimeline CoreParameterToMotion(bool right = true)
        {
            double pointingspeed = Parameters["MotionSpeed"].Value;
            double decayspeed = Parameters["DecaySpeed"].Value;
            double holdtime = Parameters["HoldTime"].Value;
            double repetition = Parameters["Repetition"].Value;

            // end pose parameters
            List<double> jointVals = CoreParameterToJoint();
            double shoulderPitch = jointVals[0];
            double shoulderRoll = jointVals[1];
            double elbowYaw = jointVals[2];
            double elbowRoll = jointVals[3];
            double wristYaw = jointVals[4];
            double hand = jointVals[5];
            double headpitch = jointVals[6];
            double headyaw = jointVals[7];

            //////////////////////////////////////////////////////////////////////////
            // initial pose that prepares to point
            double initHoldTime = 0.5;
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
            //////////////////////////////////////////////////////////////////////////

            // Initial Frame
            PoseProfile initArmPose = new PoseProfile();
            initArmPose.Joints = new RArm(false,
                    shoulderPitch_1,
                    shoulderRoll_1,
                    elbowYaw_1,
                    elbowRoll_1,
                    wristYaw_1,
                    hand_1);
            PoseProfile initHeadPose = new PoseProfile();
            //initHeadPose.Joints = new Head(false, headpitch_1, headyaw_1);
            initHeadPose.Joints = new Head(false, headpitch, headyaw);
            MotionFrame initMF = new MotionFrame();
            initMF.PoseList.Add(initArmPose);
            initMF.PoseList.Add(initHeadPose);
            //initMF.SpeedFraction = initSpeed;
            initMF.SpeedFraction = pointingspeed;
            initMF.UsingSpeedFraction = true;
            initMF.HoldTime = initHoldTime;
            initMF.IsAbsolute = true;
            initMF.JointsToList();
            // End Frame (pointing)
            PoseProfile endArmPose = new PoseProfile();
            endArmPose.Joints = new RArm(false,
                    shoulderPitch,
                    shoulderRoll,
                    elbowYaw,
                    elbowRoll,
                    wristYaw,
                    hand);
            //PoseProfile endHeadPose = new PoseProfile();
            //endHeadPose.Joints = new Head(false, headpitch, headyaw);
            MotionFrame endMF = new MotionFrame();
            endMF.PoseList.Add(endArmPose);
            //endMF.PoseList.Add(endHeadPose);
            endMF.SpeedFraction = pointingspeed;
            endMF.UsingSpeedFraction = true;
            endMF.HoldTime = holdtime;
            endMF.IsAbsolute = true;
            endMF.JointsToList();

            // Repeat Frame (pointing)
            MotionFrame repMF = new MotionFrame();
            if (repetition > 0)
            {
                double repHoldTime = 0.0;
                //////////////////////////////////////////////////////////////////////////
                // Pose for Repeat
                /*double shoulderPitch_2, shoulderRoll_2, elbowRoll_2, elbowYaw_2, wristYaw_2, Hand_2;
                shoulderPitch_2 = 0;
                shoulderRoll_2 = 0;
                elbowYaw_2 = 80;
                elbowRoll_2 = 80;
                wristYaw_2 = 80;
                Hand_2 = 0.6 * 180 / (double)Math.PI;*/

                double percent = 0.5;
                double shoulderPitch_2 = Interpolation(shoulderPitch_1, shoulderPitch, percent);
                //double shoulderPitch_2 = shoulderPitch;
                double shoulderRoll_2 = Interpolation(shoulderRoll_1, shoulderRoll, percent);
                //double shoulderRoll_2 = shoulderRoll;
                double elbowYaw_2 = Interpolation(elbowYaw_1, elbowYaw, percent);
                //double elbowYaw_2 = elbowYaw;
                double elbowRoll_2 = Interpolation(elbowRoll_1, elbowRoll, percent);
                double wristYaw_2 = Interpolation(wristYaw_1, wristYaw, percent);
                //double wristYaw_2 = wristYaw;
                double Hand_2 = Interpolation(hand_1, hand, percent);
                //double Hand_2 = hand;

                /*
                PointingParams pp1 = new PointingParams(pp);
                pp1.Amplitude -= 0.1;
                List<double> jointVals_2 = PointingJoints(pp1);
                double shoulderPitch_2 = jointVals_2[0];
                double shoulderRoll_2 = jointVals_2[1];
                double elbowYaw_2 = jointVals_2[2];
                double elbowRoll_2 = jointVals_2[3];
                double wristYaw_2 = jointVals_2[4];
                double Hand_2 = jointVals_2[5];
                */
                //////////////////////////////////////////////////////////////////////////
                PoseProfile repArmPose = new PoseProfile();
                repArmPose.Joints = new RArm(false,
                        shoulderPitch_2,
                        shoulderRoll_2,
                        elbowYaw_2,
                        elbowRoll_2,
                        wristYaw_2,
                        Hand_2);
                //PoseProfile repHeadPose = new PoseProfile();
                //repHeadPose.Joints = new Head(false, headpitch, headyaw);
                repMF.PoseList.Add(repArmPose);
                //repMF.PoseList.Add(repHeadPose);
                repMF.SpeedFraction = pointingspeed;
                repMF.UsingSpeedFraction = true;
                repMF.HoldTime = repHoldTime;
                repMF.IsAbsolute = true;
                repMF.JointsToList();
            }

            // Time Line
            MotionTimeline mtl = new MotionTimeline();
            mtl.BehaviorsDesp = "Pointing";
            mtl.OrderedMFSeq.Add(initMF);
            //
            MotionFrame endMF1 = new MotionFrame(endMF);
            endMF1.HoldTime = 0;
            // Repetition
            for (int i = 0; i < repetition; i++)
            {
                mtl.OrderedMFSeq.Add(endMF1);
                mtl.OrderedMFSeq.Add(repMF);
            }
            mtl.OrderedMFSeq.Add(endMF);

            // decay
            mtl.PoseRecover = true;
            mtl.RecovSpd = decayspeed;
            return mtl;
        }

        double Interpolation(double a, double b, double percent)
        {
            if (percent >= 0 && percent <= 1)
            {
                return a * (1 - percent) + b * percent;
            }
            else
            {
                return double.NaN;
            }
        }

        // Motion Parameters -> Joint Values
        // pose parameters -> rigidness, amplitude, openness, rhythm
        // Pointing direction - Target
        //double targetX = 200D; // screen 
        double targetX = 400D;  //exp 
        double targetY = -500D;
        //double targetZ = -50D; //screen 
        double targetZ = 600D;  //exp
        //double targetZ = -(85D + 100D + 102.9D + 45.19D); // object on the ground
        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            double openness        = Parameters["PalmUpDown"].Value;
            double fingerrigidness = Parameters["Finger"].Value;
            double amplitude       = Parameters["Amplitude"].Value;
            double headupdown      = Parameters["HeadUpDown"].Value;
            double headleftright   = Parameters["HeadLeftRight"].Value;

            // Fixed!
            double hand = fingerrigidness; // *180D / Math.PI;
            // Determine the amplitude
            double amp = amplitude * 218.666;
            //
            double elbowRoll = Math.Acos((amp * amp - Math.Pow(105D, 2D) - Math.Pow(57.75 + 55.95, 2D)) / (2D * 105D * (55.95 + 57.75)));
            elbowRoll *= 180D / Math.PI;
            // debug
            //Console.WriteLine("Amplitude: {0}mm  Elbowroll: {1}degree", amp, elbowRoll);
            // Set to 80 to make the "wristYaw" able to reach "PalmUp" and "PalmDown"
            double elbowYaw = 80D; // exp
            //double elbowYaw = 119D;  // screen
            // Determine the palm direction
            double wristYaw = 10D * openness * openness + 80D * openness + 10D;

            Microsoft.FSharp.Math.Vector<double> result
                = NaoJointChain.Arm.OptPntTo2J(elbowRoll * Math.PI / 180D, elbowYaw * Math.PI / 180D, targetX, targetY, targetZ);
            //  = JointChain.OptPntTo3J(elbowRoll * Math.PI / 180D, targetX, targetY, targetZ);
            // Pointing direction
            double shoulderPitch = result[0] * 180D / Math.PI;
            // moving outwards
            double shoulderRoll = result[1] * 180D / Math.PI;
            //
            //elbowYaw = (double)(180 * result[2] / Math.PI);

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
            // -35.0 ~ 25.0
            headpitch = -5.0 * headupdown;
            // -50 ~ 50
            headyaw = 5.0 * headleftright;
            //////////////////////////////////////////////////////////////////////////

            List<double> PointingEnd = new List<double>();
            PointingEnd.Add(shoulderPitch);
            PointingEnd.Add(shoulderRoll);
            PointingEnd.Add(elbowYaw);
            PointingEnd.Add(elbowRoll);
            PointingEnd.Add(wristYaw);
            PointingEnd.Add(hand);
            PointingEnd.Add(headpitch);
            PointingEnd.Add(headyaw);
            
            return PointingEnd;
        }
        #endregion Core



        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            throw new NotImplementedException();
        }
    } // class PointingBehavior
}
