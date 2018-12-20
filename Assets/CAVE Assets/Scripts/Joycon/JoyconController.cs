using System.Collections;
using System.Collections.Generic;
using JoyconAPI;
using UnityEngine;

namespace CaveAsset
{
    namespace Joycon
    {
        public class JoyconController : MonoBehaviour
        {
            private JoyconManager manager;
            private List<JoyconAPI.Joycon> joyconList;

            ///<summary>
            ///Instantiates the joycons 
            ///</summary>
            void Start()
            {
                manager = JoyconManager.Instance;
                joyconList = manager.joycons;
            }

            ///<summary>
            ///Gets the number of connected joycons
            ///</summary>
            ///<returns>
            ///The number of connected joycons
            ///</returns>
            public int GetNumberOfJoycons()
            {
                return joyconList.Count;
            }

            ///<summary>
            ///Gets the left joycon, regardless of connection order
            ///</summary>
            ///<returns> 
            ///Left joycon
            ///</returns>
            public JoyconAPI.Joycon GetLeftJoycon()
            {
                foreach (JoyconAPI.Joycon joy in joyconList)
                {
                    if (joy.isLeft)
                        return joy;
                }
                return null;
            }

            ///<summary>
            ///Gets the right joycon, regardless of connection order
            ///</summary>
            ///<returns> 
            ///Right joycon
            ///</returns>
            public JoyconAPI.Joycon GetRightJoycon()
            {
                foreach (JoyconAPI.Joycon joy in joyconList)
                {
                    if (!joy.isLeft)
                        return joy;
                }
                return null;
            }

            ///<summary>
            ///Gets the requested joycon
            ///</summary>
            ///<returns> 
            ///Requested joycon
            ///</returns>
            public JoyconAPI.Joycon GetJoycon(int i)
            {
                return joyconList[i];
            }

            /// <summary>
            /// Returns all joycons currently connected
            /// </summary>
            /// <returns>All joycons</returns>
            public List<JoyconAPI.Joycon> GetJoycons()
            {
                return joyconList;
            }
        }
    }
}
