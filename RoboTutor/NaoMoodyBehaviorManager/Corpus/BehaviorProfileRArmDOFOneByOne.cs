using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileRArmDOFOneByOne : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "ShowRArmDOFOneByOne"; }
        }

        public BehaviorProfileRArmDOFOneByOne()
        {
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
            this.Parameters.Add("Gaze", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(new List<double>(), "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
        }

        public override ConnectorNao.MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        public override ConnectorNao.MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double palmdir = Parameters["PalmDir"].Value;
            double wristyaw;
            if (palmdir > 0) wristyaw = 90;
            else wristyaw = -90;

            double headud = Parameters["HeadUpDown"].Value;
            double headpitch = 29 * headud * headud - 73.5 * headud + 29.5;

            double gaze = Parameters["Gaze"].Value;
            double head_right = -(10 * gaze + 10);
            Head head_r = new Head(false, headpitch, head_right);

            RArm arm_show_orig = new RArm(false, 0, 0, 90, 2, wristyaw, 0);
            PoseProfile pose_show_orig = new PoseProfile("PS_Orig", arm_show_orig);
            PoseProfile gaze_right = new PoseProfile("PS_Orig", head_r);

            RArm arm_show_shoulderpitch = new RArm(false, 30, 0, 90, 2, wristyaw, 0);
            PoseProfile pose_show_shoulderpitch = new PoseProfile("PS_SP", arm_show_shoulderpitch);
            PoseProfile pose_show_orig1 = new PoseProfile("PS_Orig1", arm_show_orig);

            RArm arm_show_shoulderroll = new RArm(false, 0, -30, 90, 2, wristyaw, 0);
            PoseProfile pose_show_shouldroll = new PoseProfile("PS_SR", arm_show_shoulderroll);
            PoseProfile pose_show_orig2 = new PoseProfile("PS_Orig2", arm_show_orig);

            RArm arm_show_elbowroll = new RArm(false, 0, 0, 90, 88.5, -wristyaw, 0);
            PoseProfile pose_show_elbowroll = new PoseProfile("PS_ER", arm_show_elbowroll);

            RArm arm_show_elbowyaw1 = new RArm(false, 0, 0, 119.5, 88.5, -wristyaw, 0);
            PoseProfile pose_show_elbowyaw1 = new PoseProfile("PS_EY1", arm_show_elbowyaw1);

            RArm arm_show_elbowyaw2 = new RArm(false, 0, 0, 60, 88.5, -wristyaw, 0);
            PoseProfile pose_show_elbowyaw2 = new PoseProfile("PS_EY2", arm_show_elbowyaw2);

            RArm arm_show_wristyaw1 = new RArm(false, 0, 0, 90, 88.5, -wristyaw, 0);
            PoseProfile pose_show_wristyaw1 = new PoseProfile("PS_WY1", arm_show_wristyaw1);

            RArm arm_show_wristyaw2 = new RArm(false, 0, 0, 90, 88.5, wristyaw, 0);
            PoseProfile pose_show_wristyaw2 = new PoseProfile("PS_WY2", arm_show_wristyaw2);

            PoseProfile pose_show_orig3 = new PoseProfile("PS_Orig3", arm_show_orig);
            RArm arm_show_finger = new RArm(false, 0, 0, 90, 2, wristyaw, 1);
            PoseProfile pose_show_finger = new PoseProfile("PS_FG", arm_show_finger);

            PoseProfile pose_show_orig4 = new PoseProfile("PS_Orig4", arm_show_orig);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_show_orig);
            lpp.Add(gaze_right);
            lpp.Add(pose_show_shoulderpitch);
            lpp.Add(pose_show_orig1);
            lpp.Add(pose_show_shouldroll);
            lpp.Add(pose_show_orig2);
            lpp.Add(pose_show_elbowroll);
            lpp.Add(pose_show_elbowyaw1);
            lpp.Add(pose_show_elbowyaw2);
            lpp.Add(pose_show_wristyaw1);
            lpp.Add(pose_show_wristyaw2);
            lpp.Add(pose_show_orig3);
            lpp.Add(pose_show_finger);
            lpp.Add(pose_show_orig4);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double motspd = 0.6;
            double decspd = 0.5;
            double holdtime = 0.8;
            double holdtime_s = 0.3;
            double rep = 0;

            double palmdir = 0.1 * valence;
            double gaze = 0.05 * valence + 0.5;
            double headud = 0.05 * valence + 0.5;

            Parameters["Gaze"].Value        = gaze;
            Parameters["HeadUpDown"].Value  = headud;
            Parameters["PalmDir"].Value     = palmdir;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value  = decspd;
            // Start pose
            Parameters["HoldTime"].ValueList.Add(holdtime);
            // Shoulder
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            // Elbow
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            // Wrist
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            // Finger
            Parameters["HoldTime"].ValueList.Add(holdtime_s);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            //
            Parameters["Repetition"].Value  = rep;
        }
    }
}
