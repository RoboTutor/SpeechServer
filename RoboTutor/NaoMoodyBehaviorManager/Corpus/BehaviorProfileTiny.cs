using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileTiny : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Tiny"; }
        }

        public BehaviorProfileTiny()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("AmplitudeFinger", new Parameter(0, "Pose"));
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
            double ampfinger  = Parameters["AmplitudeFinger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            double rshoulderroll  = -10 * width;
            double rshoulderpitch = -20 * height + 50;
            double relbowroll     = 88.5;
            double relbowyaw      = 30*width+70;
            double rwristyaw      = -65 * palmdir;
            // open
            double rhand_open     = 0.2*ampfinger + 0.8;
            double rhand_close    = 0.4;

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
                headYaw = double.NaN;
            }

            RArm rarm_open = new RArm(false, rshoulderpitch, rshoulderroll, relbowyaw, relbowroll, rwristyaw, rhand_open);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_close = new RArm(false, rshoulderpitch, rshoulderroll, relbowyaw, relbowroll, rwristyaw, rhand_close);

            // Joints
            PoseProfile pose_rarm_open = new PoseProfile("OpenPose", rarm_open, true, 1.0, 0);
            PoseProfile pose_head = new PoseProfile("OpenPose", head);
            PoseProfile pose_rarm_close = new PoseProfile("ClosePose", rarm_close, true, 1.0, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_rarm_open);
            lpp.Add(pose_head);
            lpp.Add(pose_rarm_close);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double ampfinger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.045 * valence + 0.55;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["AmplitudeFinger"].Value = ampfinger;
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
