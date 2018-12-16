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
            private JoyconAPI.Joycon[] joycons;
            private int joycount;   // Numbers of availible Joycons

            ///<summary>
            ///Instantiates the joycons 
            ///</summary>
            void Start()
            {
                manager = JoyconManager.Instance;
                joyconList = manager.joycons;
                joycount = joyconList.Count;
                joycons = new JoyconAPI.Joycon[joycount];
                for(int i = 0; i < joycount; i++)
                {
                    joycons[i] = joyconList[i];
                }
            }

            ///<summary>
            ///Gets the number of connected joycons
            ///</summary>
            ///<returns>
            ///The number of connected joycons
            ///</returns>
            public int GetNumberOfJoycons()
            {
                return joycount;
            }

            ///<summary>
            ///Gets the left joycon, regardless of connection order
            ///</summary>
            ///<returns> 
            ///Left joycon
            ///</returns>
            public JoyconAPI.Joycon GetLeftJoycon()
            {
                if (joycons[0].isLeft)
                    return joycons[0];
                else
                    return joycons[1];
            }

            ///<summary>
            ///Gets the right joycon, regardless of connection order
            ///</summary>
            ///<returns> 
            ///Right joycon
            ///</returns>
            public JoyconAPI.Joycon GetRightJoycon()
            {
                if (joycons[1].isLeft)
                    return joycons[0];
                else
                    return joycons[1];
            }

            ///<summary>
            ///Gets the requested joycon
            ///</summary>
            ///<returns> 
            ///Requested joycon
            ///</returns>
            public JoyconAPI.Joycon GetJoycon(int i)
            {
                return joycons[i];
            }
        }
    }
}
