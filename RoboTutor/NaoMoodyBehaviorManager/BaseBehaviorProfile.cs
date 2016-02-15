/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * 
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 * 
 * TODO
 * - Transfer Corpus from C# codes into XML files.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    ///
    /// </summary>
    public abstract class BehaviorProfileBase : IBehaviorProfile
    {
        // Properties
        public abstract string BehaviorName { get; }

        public bool BehaviorRecover = true;
        public bool UseSpeedFraction = false;

        // Fields
        // Parameters
        public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
        public class Parameter
        {
            public Parameter(double val, string ptype)
            {
                this.ParamType = ptype;
                this.ValType = "Single"; // must ahead of "this.Value = val;"
                this.Value = val;
            }

            public Parameter(List<double> val, string ptype)
            {
                this.ParamType = ptype;
                this.ValType = "Series"; // must ahead of "this.ValueList = val;"
                this.ValueList = val;
            }

            // Values
            double Value_;
            public double Value
            {
                set
                {
                    if (this.ValType == "Single")
                    {
                        Value_ = value;
                    }
                    else
                    {
                        throw new AccessViolationException("The parameter is initialized as a series-of-values.");
                    }
                }
                get
                {
                    if (this.ValType == "Single")
                    {
                        return Value_;
                    }
                    else
                    {
                        throw new AccessViolationException("The parameter is initialized as a series-of-values.");
                    }
                }
            }
            List<double> ValueList_;
            public List<double> ValueList
            {
                set
                {
                    if (this.ValType == "Series")
                    {
                        ValueList_ = value;
                    }
                    else
                    {
                        throw new AccessViolationException("The parameter is initialized as a single-value.");
                    }
                }
                get 
                { 
                    if (this.ValType == "Series")
                    {
                    	return ValueList_; 
                    } 
                    else
                    {
                        throw new AccessViolationException("The parameter is initialized as a single-value.");
                    }
                }
            }
            public readonly string ValType; // "Single" or "Series"

            // Properties
            public double NeutralValue;
            // Parameter properties
            public string ParamType; // "Pose" or "Motion"
            public bool ImportanceMask;
            public int DecimalPlace; // TODO
            // Bounds
            public double LowerBound;
            public double UpperBound;
            public string LBName;
            public string UBName;
        } 

        #region Interface members
        public virtual ConnectorNao.MotionTimeline LoadBehavior(double valence, double arousal = 0, string args = null)
        {
            MotionTimeline mtl = null;
            if (args == null)
            {
	            CoreAffectToParameter(valence, arousal);
	            mtl = CoreParameterToMotion(true);
            }
            else
            {
                if (args == "BOTH")
                {
                    CoreAffectToParameter(valence, arousal);
                    MotionTimeline mtl_right = CoreParameterToMotion(true);
                    MotionTimeline mtl_left = mtl_right.MirrorLeftRight();
                    mtl = MotionTimeline.MergeLeftRight(mtl_right, mtl_left);
                } 
                else if (args == "LEFT")
                {
                    CoreAffectToParameter(valence, arousal);
                    // left: false
                    mtl = CoreParameterToMotion(false);
                }
                else if (args == "RIGHT")
                {
                    // save as args==NULL
                    CoreAffectToParameter(valence, arousal);
                    // right: true
                    mtl = CoreParameterToMotion(true);
                }
                else if (this.BehaviorName == "LegRandomMove")
                {
                    string[] cmds = args.Split('-');
                    int predelay = 0;
                    double hold = 0;
                    int posedelay = 0;
                    if (cmds[0] != string.Empty) predelay = int.Parse(cmds[0]);
                    if (cmds[1] != string.Empty) hold = double.Parse(cmds[1]);
                    if (cmds[2] != string.Empty) posedelay = int.Parse(cmds[2]);

                    CoreAffectToParameter(valence, arousal);
                    mtl = CoreParameterToMotion(true);

                    if (hold > 0.5)
                    {
	                    int framecnt = mtl.OrderedMFSeq.Count;
	                    double eachhold = hold / framecnt;
	                    foreach (MotionFrame mf in mtl.OrderedMFSeq)
	                    {
	                        mf.HoldTime = eachhold;
	                    }
                    }

                    mtl.PreMotionHold = predelay;
                    mtl.PostMotionHold = posedelay;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BaseBehaviorProfile: Undefined LoadBehavior() arguments!");
                    throw new ArgumentException("Undefined LoadBehavior() arguments!");
                }
            }
            return mtl;
        }

        public abstract MotionTimeline LoadPreset(string setname, int setmode);
        public abstract MotionTimeline LoadSingleParam(int paramind, int level);
        
        /// <summary>
        /// Get current states of all parameters.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> GetAllParameters()
        {
            Dictionary<string, double> dic_p = new Dictionary<string, double>();
            List<string> lpn = this.Parameters.Keys.ToList();
            for(int i=0;i<this.Parameters.Count;i++)
            {
                string pname = lpn[i];
                double pval = this.Parameters[pname].Value;
                dic_p.Add(pname, pval);
            }

            return dic_p;
        }

        public List<string> ParameterNames
        {
            get 
            {
                List<string> lpn = this.Parameters.Keys.ToList();
                return lpn;
            }
        }

        /// <summary>
        /// Get all parameter values of a preset specified by name.
        /// </summary>
        /// <param name="setname">The name of the preset</param>
        /// <returns></returns>
        public List<double> GetPresetParameters(string setname)
        {
            List<double> pp = Presets[setname];
            return pp;
        }
        protected Dictionary<string, List<double>> Presets = new Dictionary<string, List<double>>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetPresetNames()
        {
            return Presets.Keys.ToList<string>();
        }
        #endregion Interface members

        protected Dictionary<string, List<double>> ParamLvls = new Dictionary<string, List<double>>();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetParamLevels()
        {
            List<List<double>> vals = ParamLvls.Values.ToList<List<double>>();
            List<int> cnts = new List<int>();
            foreach (var l in vals)
            {
                cnts.Add(l.Count);
            }
            return cnts;
        }

        /// <summary>
        /// Form MotionTimeLine using pose list.
        /// Repetition is handled, and its fast repetition hold-time.
        /// </summary>
        /// <param name="lpp"></param>
        /// <param name="motspd"></param>
        /// <param name="decspd"></param>
        /// <param name="holdtime"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        protected MotionTimeline CreateMotionTimeline(List<MotionFrame> lmf, double decspd, double drep)
        {
            //!!! Repetition should be done in the pose level, i.e., before Joint-list-ification
            //!!! because in one frame, some poses may be repeated, but others may not.
            // repetition
            int rep = (int)drep;
            List<MotionFrame> mergedframes = lmf;

            if (rep > 0)
            {
	            for (int i = 0; i < mergedframes.Count; i++)
	            {
	                MotionFrame curmf = mergedframes[i];
	                // It is impossible to only repeat the last frame.
	                // A return pose is always necessary.
	                if (curmf.IsRepeated == true && i < mergedframes.Count-1)
	                {
	                    List<MotionFrame> repeatedframes = new List<MotionFrame>();
	
	                    // The 1st repeated motion frame
	                    MotionFrame repmf = new MotionFrame(curmf, false);
	                    foreach (PoseProfile pp in curmf.PoseList)
	                    {
	                    	if (pp.IsRepeated==true)
	                    	{
	                    	    repmf.PoseList.Add(pp);
	                    	}
	                    }
	
	                    repeatedframes.Add(repmf);
	
	                    // Search next successive repeated frames
	                    int k;
	                    for (k = 1; k < mergedframes.Count - i; k++)
	                    {
	                        curmf = mergedframes[i + k];
	                        if (curmf.IsRepeated == false)
	                        {
	                            break;
	                        }
	                        else
	                        {
	                            MotionFrame repmf1 = new MotionFrame(curmf, false);
	                            foreach (PoseProfile pp in curmf.PoseList)
	                            {
	                                if (pp.IsRepeated == true)
	                                {
	                                    repmf1.PoseList.Add(pp);
	                                }
	                            }
	                            repeatedframes.Add(new MotionFrame(repmf1));
	                        }
	                    }
	
	                    // Make the last non-repeated frame hold shorter
	                    MotionFrame lnrf = mergedframes[i+k-1];
	                    Double orighold = lnrf.HoldTime;
	                    for (int u = 0; u < lnrf.PoseList.Count; u++)
	                    {
	                        if (!Double.IsNaN(lnrf.PoseList[u].FastRepeatHoldTime))
	                        {
	                            lnrf.HoldTime = lnrf.PoseList[u].FastRepeatHoldTime;
	                            break;
	                        }
	                    }
	
	                    // interpolation
	                    if (repeatedframes.Count>=2)
	                    {
		                    for (int n = 0; n < repeatedframes.Count; n++)
		                    {
		                        MotionFrame cmf = repeatedframes[n];
		                        
		                        for (int m=0; m<cmf.PoseList.Count; m++)
		                        {
	                                // Interpolation
		                            PoseProfile cp = cmf.PoseList[m];
		                            // assume same order
	                                if (cp.RepeatInterpol < 1 && n + 1 < repeatedframes.Count)
			                        {
	                                    MotionFrame nmf = repeatedframes[n+1];
	                                    PoseProfile np = nmf.PoseList[m];
	                                    cmf.PoseList[m] = cp.InterpolationPose(np, cp.RepeatInterpol);
			                        }
		                        }
		                    }
	                    }
	
	                    // Fast repeat
	                    foreach (MotionFrame mf in repeatedframes)
	                    {
		                    foreach (PoseProfile pp in mf.PoseList)
		                    {
			                    if (!Double.IsNaN(pp.FastRepeatHoldTime))
			                    {
			                        mf.HoldTime = pp.FastRepeatHoldTime;
                                    mf.SpeedFraction = 0.35;
			                    }
		                    }
	                    }
	
	                    // repeat these frames
	                    List<MotionFrame> allrepeatedframes = new List<MotionFrame>();
	                    for (int j = 0; j < rep; j++)
	                    {
	                        // Deep copy
	                        List<MotionFrame> repeatedframes_ = repeatedframes.Select(x => new MotionFrame(x)).ToList();
	                        allrepeatedframes.AddRange(repeatedframes_);
	                    }
	
	                    // Make the last repeated motion frame hold as long as the last non-repeated motion frame holds originally
	                    allrepeatedframes.Last<MotionFrame>().HoldTime = orighold;
	
	                    mergedframes.InsertRange(i + k, allrepeatedframes);
	
	                    // skip to the next original element
	                    i += k + rep * repeatedframes.Count;
	                }
	            }
            }

            // Joint-list-ify
            foreach (MotionFrame mf_ in mergedframes)
            {
                mf_.JointsToList();
            }

            // Time Line
            MotionTimeline mtl = new MotionTimeline();
            mtl.BehaviorsDesp = this.BehaviorName;

            // finalize frames
            mtl.OrderedMFSeq.AddRange(mergedframes);
            // decay
            mtl.PoseRecover = this.BehaviorRecover;
            mtl.RecovSpd = decspd;

            return mtl;
        }

        /// <summary>
        /// Create a list of MotionFrame using a list of PoseProfile
        /// </summary>
        /// <param name="lpp"></param>
        /// <param name="motspd"></param>
        /// <param name="holdtime"></param>
        /// <returns></returns>
        protected List<MotionFrame> CreateMotionFrameList(List<PoseProfile> lpp, double motspd, double holdtime)
        {
            // Merge joint-chains of the same ID to one frame.
            // The order of joint-chains in lpp can be random,
            // but the first emergence of a ID determines the resulting order.
            Dictionary<string, MotionFrame> mf_dic = new Dictionary<string, MotionFrame>();
            int curtimestamp = 0;
            for (int i = 0; i < lpp.Count; i++)
            {
                PoseProfile pp = lpp[i];
                string mfname = pp.ID;
                if (!mf_dic.ContainsKey(mfname))
                {
                    MotionFrame mf = new MotionFrame();
                    mf.TimeStamp = curtimestamp++;
                    mf.PoseList.Add(pp);
                    mf.PoseID = mfname;
                    mf.SpeedFraction = motspd;
                    mf.HoldTime = holdtime;
                    // false: using "angleInterpolation()"; 
                    // true: using "angleInterpolationWithSpeed()"
                    mf.UsingSpeedFraction = this.UseSpeedFraction; 
                    mf.IsAbsolute = true;

                    mf_dic.Add(mfname, mf);
                }
                else
                {
                    mf_dic[mfname].PoseList.Add(pp);
                }
            }

            return mf_dic.Values.ToList<MotionFrame>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpp"></param>
        /// <param name="motspd"></param>
        /// <param name="holdlist"></param>
        /// <returns></returns>
        protected List<MotionFrame> CreateMotionFrameList(List<PoseProfile> lpp, double motspd, List<double> holdlist)
        {
            // Merge joint-chains of the same ID to one frame.
            // The order of joint-chains in lpp can be random,
            // but the first emergence of a ID determines the resulting order.
            Dictionary<string, MotionFrame> mf_dic = new Dictionary<string, MotionFrame>();
            int curtimestamp = 0;
            int framecnt = 0;
            for (int i = 0; i < lpp.Count; i++)
            {
                PoseProfile pp = lpp[i];
                string mfname = pp.ID;
                if (!mf_dic.ContainsKey(mfname))
                {
                    MotionFrame mf = new MotionFrame();
                    mf.TimeStamp = curtimestamp++;
                    mf.PoseList.Add(pp);
                    mf.PoseID = mfname;
                    mf.SpeedFraction = motspd;
                    mf.HoldTime = holdlist[framecnt++];
                    // false: using "angleInterpolation()"; 
                    // true: using "angleInterpolationWithSpeed()"
                    mf.UsingSpeedFraction = this.UseSpeedFraction; 
                    mf.IsAbsolute = true;

                    mf_dic.Add(mfname, mf);
                }
                else
                {
                    mf_dic[mfname].PoseList.Add(pp);
                }
            }

            return mf_dic.Values.ToList<MotionFrame>();
        }

        /// <summary>
        /// Make abstract later
        /// </summary>
        /// <param name="leftright"></param>
        /// <returns></returns>
        protected abstract List<PoseProfile> CoreParameterToPose(bool right = true);

        /// <summary>
        /// Make abstract later
        /// </summary>
        /// <param name="leftright"></param>
        /// <returns></returns>
        protected /*abstract*/virtual List<double> CoreParameterToJointChain(bool right = true)
        {
            return null;
        }

        /// <summary>
        /// Behavior parameters -> joint values
        /// </summary>
        /// <param name="behparams"></param>
        /// <param name="leftright"></param>
        /// <returns></returns>
        protected abstract List<double> CoreParameterToJoint(bool right = true);

        /// <summary>
        /// Behavior parameters -> motion
        /// </summary>
        /// <param name="leftright"></param>
        /// <returns></returns>
        protected virtual MotionTimeline CoreParameterToMotion(bool right = true)
        {
            // motion parameters
            MotionTimeline mtl = null;
            try
            {
                double param_motspd = Parameters["MotionSpeed"].Value;
                double param_decspd = Parameters["DecaySpeed"].Value;

                // Perhaps, it is better to put NormalizeSpeed() inside each behavior profile
                double motspd;
                double decspd;
                if (this.BehaviorName == "LegRandomMove" 
                    || this.BehaviorName == "Balance"
                    || this.BehaviorName == "LeanRight")
                {
                    motspd = param_motspd;
                    decspd = param_decspd;
                } 
                else // Normal routine for most behaviors
                {
                    // Speed normalization
                    motspd = NormalizeSpeed(param_motspd);
                    decspd = NormalizeSpeed(param_decspd);
                }

                //System.Diagnostics.Debug.WriteLine("BaseBehavior: MotionSpeed: " + motspd);

                List<PoseProfile> lpp = CoreParameterToPose();

                // MotionFrame list, hold time
                List<MotionFrame> listmf;
                if (Parameters["HoldTime"].ValType == "Single")
                {
                	double holdtime = Parameters["HoldTime"].Value;
                    listmf = CreateMotionFrameList(lpp, motspd, holdtime);
                } 
                else // "Series"
                {
                    List<double> holdlist = Parameters["HoldTime"].ValueList;
                    listmf = CreateMotionFrameList(lpp, motspd, holdlist);
                }

                double rep = Parameters["Repetition"].Value;

                // MotionTimeline
                MotionTimeline mtl_r = CreateMotionTimeline(listmf, decspd, rep);

                if (right == true)
                {
                    mtl = mtl_r;
                } 
                else
                {
                    mtl = mtl_r.MirrorLeftRight();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BaseBehaviorProfile: Motion parameters not defined!"+Environment.NewLine+ex);
            }

            return mtl;
        }

        /// <summary>
        /// Affect -> Behavior parameters
        /// </summary>
        /// <param name="valence"></param>
        /// <param name="arousal"></param>
        /// <returns></returns>
        protected abstract void CoreAffectToParameter(double valence, double arousal = 0);

        #region Normalization
        /// <summary>
        ///  Can be override/changed by individual behaviors
        /// </summary>
        
        protected double HeadPitchUpLarge = 20; // minus
        protected double HeadPitchUpSmall = 15; // minus
        protected double HeadPitchUpRange
        {
            get { return (HeadPitchUpLarge - HeadPitchUpSmall); }
        }
        protected double HeadPitchDownLarge = 29.5;
        protected double HeadPitchDownSmall = 18;
        protected double HeadPitchDownRange
        {
            get { return (HeadPitchDownLarge - HeadPitchDownSmall); }
        }
        /// <summary>
        /// Constrain the range of head pitch, considering naturalness
        /// </summary>
        /// <param name="param_headpitch"> -1 ~ +1 </param>
        /// <returns>Normalized value for head pitch</returns>
        protected double NormalizeHeadPitch(double param_headpitch)
        {
            if (param_headpitch >= 0)
            {
            	return -(HeadPitchUpSmall + HeadPitchUpRange * param_headpitch);
            } 
            else
            {
                return (HeadPitchDownSmall - HeadPitchDownRange * param_headpitch);
            }
        }
         
        protected double HeadYawLarge = 30;
        protected double HeadYawSmall = 15;
        protected double HeadYawRange
        {
            get { return (HeadYawLarge - HeadYawSmall); }
        }
        /// <summary>
        /// Constrain the range of head yaw, considering naturalness
        /// </summary>
        /// <param name="param_headpitch"> 0 ~ 1 </param>
        /// <returns>Normalized value for head yaw</returns>
        protected double NormalizeHeadYaw(double param_headyaw)
        {
            return (HeadYawSmall + HeadYawRange * param_headyaw);
        }

        protected double SpeedBoundLow = 0.1;
        protected double SpeedBoundHigh = 0.3;
        protected double SpeedRange
        {
            get { return (SpeedBoundHigh - SpeedBoundLow); }
        }
        protected double NormalizeSpeed(double param_speed)
        {
            return (SpeedBoundLow + SpeedRange * param_speed);
        }
        #endregion Normalization

        #region MISC movements
        #region Leg Movements
        /// <summary>
        /// Standard Stand pose; knee closed
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> Stand(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -9.7,
                7.4,
                -5.7,
                -5.2,
                5.2,
                7.4);
            LLeg leg_l = new LLeg(
                false,
                -9.7,
                7.4,
                5.7,
                -5.2,
                5.2,
                -7.4);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> StandOnRight(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -10.3,
                1.6,
                -17.5,
                -4.1,
                8.9,
                12.9);
            LLeg leg_l = new LLeg(
                false,
                -10.3,
                11.2,
                -5.1,
                -5.0,
                0.7,
                0.5);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> StandOnLeft(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -9.1,
                11.1,
                6.9,
                -5.3,
                -0.2,
                -2.4);
            LLeg leg_l = new LLeg(
                false,
                -9.1,
                2.3,
                19.3,
                0.6,
                1.5,
                -16.3);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> LeanRight(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -8.3,
                -2.8,
                -15.7,
                38.7,
                -28.5,
                7.4);
            LLeg leg_l = new LLeg(
                false,
                -8.3,
                 9.8,
                -0.6,
                 4.1,
                -5.7,
                -7.6);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> LeanRightRecover(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                   0,
                -18.6,
                -13.3,
                 32.0,
                -21.7,
                  8.1);
            LLeg leg_l = new LLeg(
                false,
                    0,
                -18.6,
                  2.8,
                 37.5,
                -20.5,
                 -3.4);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg1_1(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                  1.0,
                  3.2,
                 -7.3,
                 -4.8,
                  2.6,
                  7.0);
            LLeg leg_l = new LLeg(
                false,
                  1.0,
                  0.4,
                  1.6,
                 -5.3,
                  5.7,
                 -3.5);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg1_2(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -3.2,
                  2.7,
                 -6.9,
                 -3.8,
                  2.9,
                  7.0);
            LLeg leg_l = new LLeg(
                false,
                 -3.2,
                  0.9,
                 -1.2,
                 -5.3,
                  6.5,
                 -1.1);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg2_1(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -1.0,
                  3.9,
                 -9.8,
                 -4.9,
                  3.0,
                  9.1);
            LLeg leg_l = new LLeg(
                false,
                 -1.0,
                  1.3,
                 -5.1,
                 -5.3,
                  5.7,
                  2.4);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg2_2(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -1.8,
                  7.2,
                 -3.9,
                 -5.2,
                  0.7,
                  4.7);
            LLeg leg_l = new LLeg(
                false,
                 -1.8,
                  7.6,
                  5.8,
                 -5.3,
                  0.4,
                 -6.3);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg3_1(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -4.7,
                 13.2,
                -10.0,
                 -5.2,
                 -1.6,
                  8.7);
            LLeg leg_l = new LLeg(
                false,
                 -4.7,
                 11.7,
                 -2.0,
                 -5.3,
                  0.2,
                 -1.0);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg3_2(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                  0.4,
                  5.0,
                  0.1,
                 -5.3,
                  2.7,
                  1.6);
            LLeg leg_l = new LLeg(
                false,
                  0.4,
                  9.8,
                  7.1,
                 -5.3,
                 -2.1,
                 -7.5);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg_BentKneeLeft(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -9.8,
                  5.3,
                 -5.8,
                  2.1,
                  0.4,
                  6.3);
            LLeg leg_l = new LLeg(
                false,
                 -9.8,
                  7.5,
                  5.7,
                 -5.2,
                  5.1,
                 -7.4);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Paired
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg_BentKneeRight(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -10.2,
                  7.5,
                 -5.7,
                 -5.1,
                  5.2,
                  7.5);
            LLeg leg_l = new LLeg(
                false,
                -10.2,
                 -1.1,
                  5.9,
                  2.9,
                  4.7,
                 -7.2);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Independent; combined with Stand()
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg_Back(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -7.7,
                 -2.0,
                 -3.4,
                  0.9,
                  4.3,
                  3.7);
            LLeg leg_l = new LLeg(
                false,
                 -7.7,
                 -1.4,
                  3.4,
                  0.8,
                  3.8,
                 -5.2);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// Independent; combined with Stand()
        /// </summary>
        /// <param name="mfname"></param>
        /// <returns></returns>
        protected List<PoseProfile> MoveLeg_Front(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                 -9.6,
                 12.7,
                 -6.1,
                 -5.2,
                  1.0,
                  6.4);
            LLeg leg_l = new LLeg(
                false,
                 -9.6,
                 12.0,
                  6.1,
                 -5.3,
                  1.6,
                 -7.6);
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> BendTorso(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -0.8,
                -74.6,
                -6.8,
                38.6,
                -7.6,
                4.2);
            LLeg leg_l = new LLeg(
                false,
                -0.8,
                -74.6,
                6.8,
                38.6,
                -7.6,
                -6.7); // -4.2 symmetry
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        protected List<PoseProfile> BendTorsoSmall(string mfname)
        {
            RLeg leg_r = new RLeg(
                false,
                -17.8,
                -0.6,
                -2.7,
                -5.3,
                5.5,
                4.6);
            LLeg leg_l = new LLeg(
                false,
                -17.8,
                -1.5,
                 3.1,
                -5.3,
                 5.2,
                -5.5); //
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mfname"></param>
        /// <param name="leanforward">0~1.0</param>
        /// <returns></returns>
        protected List<PoseProfile> BodyLeanForward(string mfname, double leanforward = 0)
        {
            // body-hip angle matters
            // body-hip angle = HipPitch + 180
            // body lean angle: How to calculate?
            // upright ~ lean forward    7.4 ~ -3.0
            //  7.4 ~ -20.0
            double hippitch = -27.4 * leanforward + 7.4;
            // -5.2 ~  15.0
            double kneepitch = 20.2 * leanforward - 5.2;
            //  5.2 ~   2.0
            double anklepitch = -3.2 * leanforward + 5.2;
            RLeg leg_r = new RLeg(
                false,
                -9.7,
               hippitch,
                -5.7,
              kneepitch,
             anklepitch,
                 7.4);
            LLeg leg_l = (LLeg)leg_r.MirrorLeftRight();
            PoseProfile pp_leg_r = new PoseProfile(mfname, leg_r);
            PoseProfile pp_leg_l = new PoseProfile(mfname, leg_l);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pp_leg_r);
            lpp.Add(pp_leg_l);

            return lpp;
        }
        #endregion Leg Movements

        #region Arm Movements
        protected List<PoseProfile> StandArmPose(string mfname)
        {
            RArm armR = new RArm(
                false,
                86,
                -7,
                68,
                23.5,
                6,
                0.3);
            LArm armL = (LArm)armR.MirrorLeftRight();
            PoseProfile armppR = new PoseProfile(mfname, armR);
            PoseProfile armppL = new PoseProfile(mfname, armL);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armppR);
            lpp.Add(armppL);

            return lpp;
        }

        protected List<PoseProfile> RightArmPointLeftSonar(string mfname)
        {
            RArm arm = new RArm(
                false,
                44,
                16,
                16,
                63,
                50,
                0.3);
            PoseProfile armpp = new PoseProfile(mfname, arm);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armpp);

            return lpp;
        }

        protected List<PoseProfile> RightArmPointRightSonar(string mfname)
        {
            RArm arm = new RArm(
                false,
                55,
                -15,
                30,
                88.5,
                50,
                0.3);
            PoseProfile armpp = new PoseProfile(mfname, arm);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armpp);

            return lpp;
        }

        protected List<PoseProfile> RightArmPointFrontMic(string mfname)
        {
            RArm arm = new RArm(
                false,
                -33,
                -20,
                45,
                88.5,
                50,
                0.6);
            PoseProfile armpp = new PoseProfile(mfname, arm);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armpp);

            return lpp;
        }

        protected List<PoseProfile> RightArmPointSideMic(string mfname)
        {
            RArm arm = new RArm(
                false,
                -60,
                -30,
                45,
                88.5,
                50,
                0.6);
            PoseProfile armpp = new PoseProfile(mfname, arm);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armpp);

            return lpp;
        }

        protected List<PoseProfile> RightArmPointRearMic(string mfname)
        {
            RArm arm = new RArm(
                false,
                -75,
                -30,
                45,
                88.5,
                50,
                0.6);
            PoseProfile armpp = new PoseProfile(mfname, arm);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armpp);

            return lpp;
        }

        protected List<PoseProfile> ArmChest(string mfname)
        {
            RArm armR = new RArm(
                false,
                60,
                5,
                50,
                88.5,
                15,
                0);
            LArm armL = new LArm(
                false,
                70,
                -5,
                -40,
                -88.5,
                -15,
                 0);
            PoseProfile armppR = new PoseProfile(mfname, armR);
            PoseProfile armppL = new PoseProfile(mfname, armL);
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(armppR);
            lpp.Add(armppL);

            return lpp;
        }
        #endregion Arm Movements

        #region Head Movements

        #endregion Head Movements

        #endregion MISC movements
    }
}
