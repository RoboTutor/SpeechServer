/*
 * TODO: The two presets should be merged!
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConnectorNao;// MTL

namespace NaoManager
{
    public class BehaviorProfileWaving : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Waving"; }
        }

        List<double> Neutral;
        public BehaviorProfileWaving()
        {
            this.Parameters.Add("HandHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));

            //InitValues = new List<double>() { 0.5, 0.5, 0.7, 3, 0.2, 0.25, 0.25, 0, 0 };
            //LowerBounds = new List<double>() { 0.0, 0.0, 0.4, 1, 0.0, 0.02, 0.02, -5, -10 };
            //UpperBounds = new List<double>() { 1.0, 1.0, 1.0, 10, 5.0, 0.50, 0.50, 7, 10 };
            //LBName = new List<string>() { "low", "flexible", "small", "1", "short", "slow", "slow", "down", "left" };
            //UBName = new List<string>() { "high", "rigid", "large", "10", "long(sec)", "fast(%)", "fast(%)", "up", "right" };
            //DecimalPlaces = new List<int>() { 2, 2, 2, 0, 1, 2, 2, 1, 1 };
            Neutral = new List<double>() { 0.55, 0.65, 0.70, 2, 0.2, 0.23, 0.27, 0, 0 };

            // TODO: presets load from XML files
            #region Preset
            //List<string> PresetName  = new List<string>() { "neutral", "angry", "sad", "depressed", "veryunhappy", "excited", "happy", "elated", "veryhappy" };
            List<string> PresetName = new List<string>() { "neutral", "angry", "sad", "veryunhappy", "happy", "veryhappy" };
            //List<double> neutral     = new List<double>() { 0.5, 0.60, 0.70, 2, 0.2, 0.25, 0.25,  0, 0 };
            //List<double> angry       = new List<double>() { 0.3, 0.83, 0.58, 3, 0.1, 0.30, 0.35,  0, 3 };
            //List<double> sad         = new List<double>() { 0.3, 0.54, 0.58, 2, 0.1, 0.12, 0.15, -3, 3 };
            List<double> depressed = new List<double>() { 0.1, 0.54, 0.58, 1, 0.5, 0.12, 0.15, -5, 6 };
            //List<double> veryunhappy = new List<double>() { 0.1, 0.43, 0.46, 1, 0.1, 0.05, 0.05, -5, 6 };
            List<double> excited = new List<double>() { 0.9, 1.00, 0.58, 4, 0.0, 0.30, 0.35, 5, 0 };
            //List<double> happy       = new List<double>() { 0.8, 0.90, 0.82, 3, 0.1, 0.30, 0.35,  3, 0 };
            List<double> elated = new List<double>() { 0.9, 1.00, 0.82, 4, 0.0, 0.40, 0.45, 5, 0 };
            //List<double> veryhappy   = new List<double>() { 0.9, 1.00, 0.90, 4, 0.1, 0.40, 0.45,  5, 0 };

            // final
            List<double> angry = new List<double>() { 0.20, 0.85, 0.58, 1, 0.0, 0.43, 0.47, -3, 3 };
            List<double> sad = new List<double>() { 0.30, 0.45, 0.58, 2, 0.4, 0.13, 0.17, -3, 3 };
            List<double> veryunhappy = new List<double>() { 0.20, 0.35, 0.46, 1, 0.5, 0.05, 0.09, -5, 6 };
            List<double> happy = new List<double>() { 0.75, 0.85, 0.82, 3, 0.1, 0.33, 0.37, 3, 0 };
            List<double> veryhappy = new List<double>() { 0.95, 1.00, 0.94, 4, 0.0, 0.43, 0.47, 5, 0 };

            // Individual parameter levels
            List<double> handheight = new List<double>() { 0.1, 0.3, 0.5, 0.8, 0.9 };
            List<double> fingerrigidness = new List<double>() { 0.43, 0.54, 0.60, 0.83, 0.90, 1.00 };
            List<double> amplitude = new List<double>() { 0.46, 0.58, 0.70, 0.82, 0.90 };
            List<double> repetition = new List<double>() { 1, 2, 3, 4 };
            List<double> holdtime = new List<double>() { 0.0, 0.1, 0.2, 0.5 };
            List<double> decayspeed = new List<double>() { 0.05, 0.12, 0.25, 0.30, 0.40 };
            List<double> wavingspeed = new List<double>() { 0.05, 0.15, 0.25, 0.35, 0.45 };
            List<double> headupdown = new List<double>() { -5, -3, 0, 3, 5 };
            List<double> headleftright = new List<double>() { 0, 3, 6 };


            //                              HndHgt FngRgd Ampltd Repeat HoldTm DecSpd PntSpd HeadUD HeadLR
            //List<double> P0 = new List<double>() { 0.22, 0.40, 0.52, 2, 0.27, 0.12, 0.15, -5.0, 0 };
            //List<double> P1 = new List<double>() { 0.36, 0.56, 0.66, 2, 0.27, 0.17, 0.22, -3.6, 0 };
            //List<double> P2 = new List<double>() { 0.50, 0.74, 0.79, 2, 0.17, 0.26, 0.33, 2.0, 0 };
            //List<double> P3 = new List<double>() { 0.85, 0.85, 0.90, 3, 0.13, 0.31, 0.39, 3.8, 0 };
            //List<double> P4 = new List<double>() { 1.00, 0.98, 0.97, 3, 0.07, 0.37, 0.45, 4.7, 0 };
            //  !!! below used somewhere !!!
            List<double> P0 = new List<double>() { 0.21, 0.32, 0.52, 1, 0.29, 0.07, 0.12, -4.7, 0 };
            List<double> P1 = new List<double>() { 0.31, 0.41, 0.64, 2, 0.26, 0.14, 0.19, -3.3, 0 };
            List<double> P2 = new List<double>() { 0.56, 0.67, 0.78, 2, 0.20, 0.26, 0.33, 1.8, 0 };
            List<double> P3 = new List<double>() { 0.78, 0.83, 0.87, 3, 0.13, 0.33, 0.40, 3.7, 0 };
            List<double> P4 = new List<double>() { 0.99, 0.99, 0.98, 4, 0.03, 0.38, 0.47, 5.1, 0 };
            #endregion
            //PresetParams.Add(-10, P0);
            //PresetParams.Add(-5, P1);
            //PresetParams.Add(0, P2);
            //PresetParams.Add(5, P3);
            //PresetParams.Add(10, P4);

            // be consistent with PresetName
            // "neutral", "angry", "sad", "depressed", "veryunhappy", "excited", "happy", "elated", "veryhappy"
            Presets.Add("neutral", Neutral); // 0
            Presets.Add("angry", angry);
            Presets.Add("sad", sad);
            Presets.Add("depressed", depressed);
            Presets.Add("veryunhappy", veryunhappy);
            Presets.Add("excited", excited);
            Presets.Add("happy", happy);
            Presets.Add("elated", elated);
            Presets.Add("veryhappy", veryhappy);

            ParamLvls.Add("handheight", handheight);
            ParamLvls.Add("fingerrigidness", fingerrigidness);
            ParamLvls.Add("amplitude", amplitude);
            ParamLvls.Add("repetition", repetition);
            ParamLvls.Add("holdtime", holdtime);
            ParamLvls.Add("decayspeed", decayspeed);
            ParamLvls.Add("wavingspeed", wavingspeed);
            ParamLvls.Add("headupdown", headupdown);
            ParamLvls.Add("headleftright", headleftright);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setname">mood labels</param>
        /// <param name="setmode">APS, IPS, UPS</param>
        /// <returns></returns>
        public override MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();

            // For an experiment
            //List<bool> ImportanceMasks = new List<bool>() { true, false, true, true, false, false, true, true, false };

            //if (setmode == 0) // APS all parameters
            //{
            //    return CoreParameterToMotion(Presets[setname]);
            //}
            //else if (setmode == 1) // IPS return "minimum set"
            //{
            //    List<double> preset_params = Presets[setname];
            //    List<double> preset_params_minimumset = new List<double>();
            //    for (int i = 0; i < preset_params.Count; i++)
            //    {
            //        if (ImportanceMasks[i]) preset_params_minimumset.Add(preset_params[i]);
            //        else preset_params_minimumset.Add(Presets["neutral"][i]);
            //    }
            //    return CoreParameterToMotion(preset_params_minimumset);
            //}
            //else if (setmode == 2) // UPS less important parameters
            //{
            //    List<double> preset_params = Presets[setname];
            //    List<double> preset_params_minimumset = new List<double>();
            //    for (int i = 0; i < preset_params.Count; i++)
            //    {
            //        if (ImportanceMasks[i] == false) preset_params_minimumset.Add(preset_params[i]);
            //        else preset_params_minimumset.Add(Presets["neutral"][i]);
            //    }
            //    return CoreParameterToMotion(preset_params_minimumset);
            //}
            //else
            //{
            //    // exception
            //    return null;
            //}
        }

        public override MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
            //List<double> single_modulated_params = new List<double>(Neutral);
            //List<double> lvls = (ParamLvls.Values.ToList<List<double>>())[paramind];
            //single_modulated_params[paramind] = lvls[level];
            //return CoreParameterToMotion(single_modulated_params);
        }

        #region Core
        // Motion parameters -> joint values
        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double handheight      = Parameters["HandHeight"].Value;
            double fingerrigidness = Parameters["Finger"].Value;
            double amplitude       = Parameters["Amplitude"].Value;
            double headupdown      = Parameters["HeadUpDown"].Value;
            double headleftright   = Parameters["HeadLeftRight"].Value;

            double shoulderPitch   = 0;
            double shoulderRollOut = 0;
            double shoulderRollInw = 0;
            double elbowYawOut     = 0;
            double elbowYawInw     = 0;
            double elbowRollOut    = 0;
            double elbowRollInw    = 0;
            double wristYawOut     = 0;
            double wristYawInw     = 0;
            double hand            = 0;
            double headpitch       = 0;
            double headyaw         = 0;

            //////////////////////////////////////////////////////////////////////////
            shoulderPitch = -53.939 * handheight * handheight - 76.061 * handheight + 48.602;
            hand = fingerrigidness; // *180D / Math.PI;

            // Waving Mode 1
            if (handheight <= 0.8 && handheight >= 0)
            {
                double shoulderRoll = -20.0;
                // ShoulderRoll
                double dSR;
                if (amplitude <= 0.82) dSR = 10.0;
                else dSR = 50.0 * amplitude - 31.0;
                // waving pose pair
                shoulderRollOut = shoulderRoll - dSR;
                shoulderRollInw = shoulderRoll + dSR;

                // ElbowYaw main joint for the waving
                double elbowYaw;
                if(handheight < 0.5) elbowYaw= 88.0;
                else elbowYaw = -40.0 * handheight + 108.0;
                double dEY;
                if (amplitude <= 0.82) dEY = amplitude * 50.0 - 10.0;
                else dEY = 31.0;
                elbowYawOut = elbowYaw + dEY;
                elbowYawInw = elbowYaw - dEY;

                // Waving Mode 1
                // WristYaw and ElbowRoll is computed
                // Outward
                double arg1 = elbowYawOut * Math.PI / 180D;
                double arg2 = shoulderPitch * Math.PI / 180D;
                double arg3 = shoulderRollOut * Math.PI / 180D;
                Microsoft.FSharp.Math.Vector<double> resultOut
                    = NaoJointChain.Arm.OptPalmDirComp(arg1, arg2, arg3);
                wristYawOut = resultOut[0] * 180 / Math.PI;
                elbowRollOut = resultOut[1] * 180 / Math.PI;
                // Inward
                double arg4 = elbowYawInw * Math.PI / 180D;
                double arg5 = shoulderRollInw * Math.PI / 180D;
                Microsoft.FSharp.Math.Vector<double> resultInw
                    = NaoJointChain.Arm.OptPalmDirComp(arg4, arg2, arg5);
                wristYawInw = resultInw[0] * 180 / Math.PI;
                elbowRollInw = resultInw[1] * 180 / Math.PI;
                System.Diagnostics.Debug.WriteLine("Waving mode 1 - WristYawOut: {0}  ElbowRollOut: {1} WristYawInw: {2}  ElbowRollInw: {3}",
                    wristYawOut, elbowRollOut, wristYawInw, elbowRollInw);
            }
            else // Waving Mode 2
            {
                double shoulderRoll;
                if (handheight > 0.4) shoulderRoll = -20.0 * handheight - 12.0;
                else shoulderRoll = -20.0;
                // ShoulderRoll
                double dSR;
                if (amplitude <= 0.7) dSR = amplitude * 50.0 / 3.0 + 10.0 / 3.0;
                else dSR = amplitude * 110.0 / 3.0 - 32.0 / 3.0;
                // waving pose pair
                shoulderRollOut = shoulderRoll - dSR;
                shoulderRollInw = shoulderRoll + dSR;

                // waving mode 2: ElbowRoll takes charge of the waving
                double elbowRoll = 30.0;
                double OutdER, InwdER;
                if (amplitude > 0.7)
                {
                    OutdER = amplitude * 80.0 / 3.0 + 4.0 / 3.0;
                    InwdER = 20;
                } 
                else
                {
                    InwdER = OutdER = amplitude * 100.0 / 3.0 - 10.0 / 3.0;
                }
                elbowRollInw = elbowRoll + InwdER;
                elbowRollOut = elbowRoll - OutdER;

                // waving mode 2: ElbowYaw and WristYaw will keep the hand facing forward
                double elbowYaw = 0.0;
                double dEY      = 0.0;
                elbowYawOut     = elbowYaw + dEY;
                elbowYawInw     = elbowYaw - dEY;
                // WristYaw and ElbowYaw is computed
                wristYawOut     = 0.0;
                wristYawInw     = 0.0;

                //// Outward
                //double arg1 = elbowYaw * Math.PI / 180D;
                //double arg2 = elbowRollOut * Math.PI / 180D;
                //double arg3 = shoulderPitch * Math.PI / 180D;
                //double arg4 = shoulderRollOut * Math.PI / 180D;
                //Microsoft.FSharp.Math.Vector<double> resultOut
                //    = NaoJointChain.Arm.OptPalmDirCompWY(arg1, arg2, arg3, arg4);
                //wristYawOut = resultOut[0] * 180 / Math.PI;
                //elbowYawOut = resultOut[1] * 180 / Math.PI;
                //// Inward
                ////double arg5 = elbowYawInw * Math.PI / 180D;
                //double arg6 = elbowRollInw * Math.PI / 180D;
                //double arg7 = shoulderRollInw * Math.PI / 180D;
                //Microsoft.FSharp.Math.Vector<double> resultInw
                //    = NaoJointChain.Arm.OptPalmDirCompWY(arg1, arg6, arg3, arg7);
                //wristYawInw = resultInw[0] * 180 / Math.PI;
                //elbowYawInw = resultInw[1] * 180 / Math.PI;
                ////Console.WriteLine("WristYawOut: {0}  ElbowYawOut: {1} WristYawInw: {2}  ElbowYawInw: {3}",
                ////    wristYawOut, elbowYawOut, wristYawInw, elbowYawInw);
                //Console.WriteLine("WristYawOut: {0}   WristYawInw: {1}",
                //    wristYawOut, wristYawInw);

                System.Diagnostics.Debug.WriteLine("Waving mode 2");
            }

            // -35.0 ~ 25.0
            headpitch = -5.0 * headupdown;
            // -50 ~ 50
            headyaw   = 5.0 * headleftright;


            Head head = new Head(false, headpitch, headyaw);
            PoseProfile pose_head = new PoseProfile("OutSwing", head);

            // Pose1
            // frame1 outward
            RArm out_arm = new RArm(false,shoulderPitch,shoulderRollOut,elbowYawOut,elbowRollOut,wristYawOut,hand);
            PoseProfile pose_arm_out = new PoseProfile("OutSwing", out_arm, true);
            // frame2 inward
            RArm inw_arm = new RArm(false,shoulderPitch,shoulderRollInw,elbowYawInw,elbowRollInw,wristYawInw,hand);
            PoseProfile pose_arm_inw = new PoseProfile("InwSwing", inw_arm, true);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_arm_out);
            lpp.Add(pose_arm_inw);

            return lpp;
        }

        // return: Waving parameters
        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            double handheight = 0.04 * valence + 0.56;
            double amp = 0.023 * valence + 0.75;
            double frigid = 0.035 * valence + 0.64;
             
            double holdtime = -0.0005 * valence * valence - 0.013 * valence + 0.2;
            double rep = (int)(0.125 * valence);
            
            double factor = 1.4;
            double motspd = factor * (0.036 * valence + 0.6);
            double decspd = factor * (0.032 * valence + 0.47);

            double headupdown = 0.533 * valence + 0.54;
            double headdist = 0;

            Parameters["HandHeight"].Value = handheight;
            Parameters["Amplitude"].Value = amp;
            Parameters["Finger"].Value = frigid;
            Parameters["HeadUpDown"].Value = headupdown;
            Parameters["HeadLeftRight"].Value = headdist;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
        #endregion Core

    } // class WavingBehavior
}
