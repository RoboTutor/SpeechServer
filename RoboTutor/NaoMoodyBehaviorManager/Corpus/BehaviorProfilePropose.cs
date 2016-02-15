using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfilePropose : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Propose"; }
        }

        public BehaviorProfilePropose()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

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
            double width   = Parameters["Width"].Value;
            double height  = Parameters["Height"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            double rshoulderroll_init  = 18;
            double rshoulderpitch_init = 10;
            double relbowroll_init     = 88.5;
            double relbowyaw_init      = 40;
            double rwristyaw_init      = 30;
            double rhand_init          = 0.6;

            double rshoulderroll  = -30 * width;
            double rshoulderpitch = -20 * height + 50;
            double relbowroll     = 88.5;
            double relbowyaw      = 20 * width + 70;
            double rwristyaw      = -60 * palmdir + 90;
            double hand           = 0.2 * finger + 0.6;

            // ? degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // -20 ~ 0 degree
            double headYaw;
            if (headlr > 0.5)
            {
                headYaw = -40 * headlr + 20;
            } 
            else
            {
                headYaw = 0;
            }

            RArm rarm_init = new RArm(false, rshoulderpitch_init, rshoulderroll_init, relbowyaw_init, relbowroll_init, rwristyaw_init, rhand_init);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_end = new RArm(false, rshoulderpitch, rshoulderroll, relbowyaw, relbowroll, rwristyaw, hand);

            // Joints
            PoseProfile pose_rarm_init = new PoseProfile("InitPose", rarm_init);
            PoseProfile pose_head_init = new PoseProfile("InitPose", head);
            PoseProfile pose_rarm_end = new PoseProfile("EndPose", rarm_end);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_rarm_init);
            lpp.Add(pose_head_init);
            lpp.Add(pose_rarm_end);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.045 * valence + 0.55;
            double rep = 0; // could be more => emphasize
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
