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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;  // MTL

namespace NaoManager
{
    public interface IBehaviorProfile
    {
        /// <summary>
        /// load motiontimeline by valence and arousal, with additional arguments
        /// </summary>
        /// <param name="valence"></param>
        /// <param name="arousal"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        MotionTimeline LoadBehavior(double valence, double arousal=0, string args=null);

        /// <summary>
        /// load motiontimeline when only modulating one parameter
        /// </summary>
        /// <param name="paramind"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        MotionTimeline LoadSingleParam(int paramind, int level);

        /// <summary>
        /// load motiontimeline by preset name and mode (aps, ips, ups)
        /// </summary>
        /// <param name="setname"></param>
        /// <param name="setmode"></param>
        /// <returns></returns>
        MotionTimeline LoadPreset(string setname, int setmode);

        /// <summary>
        /// Get names of all presets
        /// </summary>
        /// <returns></returns>
        List<string> GetPresetNames();

        /// <summary>
        /// Get values of all parameters of a specific preset
        /// </summary>
        /// <param name="setname"></param>
        /// <returns></returns>
        List<double> GetPresetParameters(string setname);

        /// <summary>
        /// Get Dictionary<names, values> of all parameters of this behavior
        /// </summary>
        /// <returns></returns>
        Dictionary<string, double> GetAllParameters();

        List<string> ParameterNames{get;}
    }

}
