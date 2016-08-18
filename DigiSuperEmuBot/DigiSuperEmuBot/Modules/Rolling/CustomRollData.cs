using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DigiSuperEmuBot.Modules.Rolling
{
    [Serializable]
    [XmlRoot("CustomRollList")]
    [XmlInclude(typeof(CustomRollData))]
    public class CustomRollList
    {
        public List<CustomRollData> CustomRollDataList { get; set; }

        public CustomRollList()
        {
            CustomRollDataList = new List<CustomRollData>();
        }
        [Serializable]

        public class CustomRollData
        {
            [XmlAttribute]
            public string UserId { get; set; }
            [XmlAttribute]
            public List<string> CustomRollName { get; set; }
            [XmlAttribute]
            public List<string> CustomRollInput { get; set; }

            public CustomRollData(string userId)
            {
                UserId = userId;
                CustomRollName = new List<string>();
                CustomRollInput = new List<string>();
            }

            public CustomRollData()
            {
                UserId = "nope";
                CustomRollName = new List<string>();
                CustomRollInput = new List<string>();
            }

            /// <summary>
            /// Grab the custom roll if available.
            /// </summary>
            /// <param name="rollToGrab">The custom rolls name.</param>
            /// <returns>the roll in #d# format. "failure" if nothing found.</returns>
            public string GetRoll(string rollToGrab)
            {
                for (int i = 0; i < CustomRollName.Count; i++)
                {
                    if (rollToGrab.Equals(CustomRollName[i]))
                    {
                        return CustomRollInput[i];
                    }
                }
                return "failure";
            }

            /// <summary>
            /// Creating custom roll
            /// </summary>
            /// <param name="rollName"></param>
            /// <param name="rollInput"></param>
            /// <returns>If the roll was created or not</returns>
            public bool CreateRoll(string rollName, string rollInput)
            {
                if (rollInput.IndexOf('d') != -1 && rollInput.IndexOf('d') != 0)
                {
                    if (!RollingMethods.TestRoll(rollInput))
                    {
                        return false;
                    }
                }
                else { return false; }
                
                if (CustomRollName.Any(RollName => RollName.Equals(rollName)))
                {
                    return false;
                }
                
                CustomRollName.Add(rollName);
                CustomRollInput.Add(rollInput);
                return true;
            }

            /// <summary>
            /// Delete the custom roll
            /// </summary>
            /// <returns>If the delete succeeded or not</returns>
            public bool DeleteRoll(string rollName)
            {
                for (int i = 0; i < CustomRollName.Count; i++)
                {
                    if (!CustomRollName[i].Equals(rollName)) continue;
                    CustomRollName.RemoveAt(i);
                    CustomRollInput.RemoveAt(i);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Change the custom roll.
            /// </summary>
            /// <returns>If the roll change succeeded or not</returns>
            public bool ChangeRoll(string rollName, string rollInput)
            {
                if (!RollingMethods.TestRoll(rollInput))
                    return false;
                for (int i = 0; i < CustomRollName.Count; i++)
                    if (CustomRollName[i].Equals(rollName))
                        CustomRollInput[i] = rollInput;
                return false;
            }

            public string ListRolls()
            {
                if (CustomRollInput.Count < 1) return "Nothing recorded. How odd...";
                string builder = "";
                for(int i = 0; i < CustomRollName.Count; i++)
                    builder += $"\n{CustomRollName[i]} --- {CustomRollInput[i]}";
                return builder;
            }

            public void PurgeRolls()
            {
                while(CustomRollName.Count > 0)
                {
                    CustomRollName.RemoveAt(0);
                    CustomRollInput.RemoveAt(0);
                }
            }

        }
    }
    


}
