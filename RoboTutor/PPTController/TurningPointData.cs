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
using System.Xml;
using System.IO;

namespace PPTController
{
    /// <summary>
    /// Real time access to Turningpoint generated data in XML files.
    /// </summary>
    public class TurningPointData
    {
        string TurningPointDataFile;

        public TurningPointData()
        {
            TurningPointDataFile = Environment.GetEnvironmentVariable("APPDATA") + "\\TTRepository\\ActiveSession\\TTSession.xml";
        }

        public void AnswerDistribution(out bool disagree, out int useranswer, out int useranswer2)
        {
            XmlDocument XmlDoc = new XmlDocument();
            FileStream fs;
            try
            {
            	fs = new FileStream(TurningPointDataFile, FileMode.Open, FileAccess.Read);

                try
                {
                    XmlDoc.Load(fs);
                }
                catch (System.Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }

            XmlNode session = XmlDoc.SelectSingleNode("session");
            XmlNode questionlist = session.SelectSingleNode("questionlist");
            XmlNode questions = questionlist.SelectSingleNode("questions");
            XmlNode current_question = questions.LastChild;
            XmlNode choices = current_question.SelectSingleNode("answers");
            int num_choices = choices.ChildNodes.Count;

            // read user responses
            int[] distribution = new int[num_choices];
            XmlNode responses = current_question.SelectSingleNode("responses");
            for (XmlNode node = responses.FirstChild; node != null; node=node.NextSibling)
            {
                int answer = int.Parse(node.SelectSingleNode("responsestring").InnerText) - 1;
                distribution[answer]++;
            }

            // find the most frequent user answer
            // and see if there is a disagreement
            int max=0,max_ind=0,max2=0,max2_ind=0;
            for(int i=0;i<num_choices;i++)
            {
                int d = distribution[i];
                if (d > max) { max = d; max_ind = i; }
                else if (d >= max2) { max2 = d; max2_ind = i; }
            }

            if (max == max2) { disagree = true; }
            else { disagree = false; }

            useranswer = max_ind; 
            useranswer2 = max2_ind;

            fs.Close();
        }
    }
}
