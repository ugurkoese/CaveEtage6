using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System;

namespace JoyconAPI
{
    /// <summary>
    /// Manage all connected Joycons and save it to a list.
    /// This class is a singleton
    /// </summary>
    public class JoyconManager : MonoBehaviour
    {
        // Settings accessible via Unity
        public bool EnableIMU = true;
        public bool EnableLocalize = true;

        // Different operating systems either do or don't like the trailing zero
        private const ushort vendor_id = 0x57e;
        private const ushort vendor_id_ = 0x057e;
        private const ushort product_l = 0x2006;
        private const ushort product_r = 0x2007;

        /// <summary>
        /// A list of connected joycons
        /// </summary>
        public List<Joycon> joycons; // Array of all connected Joy-Cons
        private static JoyconManager instance = null;
        
        /// <summary>
        /// Get the initialized instance of the class JoyconManager
        /// </summary>
        /// <returns>
        /// Instance of JoyconManger class
        /// </returns>
        public static JoyconManager Instance
        {
            get {
                 return instance;
            }
        }
        ///<summary>
        ///Upon start controllers are initialised
        ///</summary>
        void Awake()
        {
            if (instance != null) Destroy(gameObject);
            instance = this;
            int i = 0;

            joycons = new List<Joycon>();
            bool isLeft = false;
            HIDapi.hid_init();

            IntPtr ptr = HIDapi.hid_enumerate(vendor_id, 0x0);
            IntPtr top_ptr = ptr;

            if (ptr == IntPtr.Zero)
            {
                ptr = HIDapi.hid_enumerate(vendor_id_, 0x0);
                if (ptr == IntPtr.Zero)
                {
                    HIDapi.hid_free_enumeration(ptr);
                    Debug.Log("No Joy-Cons found!");
                }
            }
            hid_device_info enumerate;
            while (ptr != IntPtr.Zero)
            {
                enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));

                Debug.Log(enumerate.product_id);
                if (enumerate.product_id == product_l || enumerate.product_id == product_r)
                {
                    if (enumerate.product_id == product_l)
                    {
                        isLeft = true;
                        Debug.Log("Left Joy-Con connected.");
                    }
                    else if (enumerate.product_id == product_r)
                    {
                        isLeft = false;
                        Debug.Log("Right Joy-Con connected.");
                    }
                    else
                    {
                        Debug.Log("Non Joy-Con input device skipped.");
                    }
                    IntPtr handle = HIDapi.hid_open_path(enumerate.path);
                    HIDapi.hid_set_nonblocking(handle, 1);
                    joycons.Add(new Joycon(handle, EnableIMU, EnableLocalize & EnableIMU, 0.05f, isLeft));
                    ++i;
                }
                ptr = enumerate.next;
            }
            HIDapi.hid_free_enumeration(top_ptr);
        }

        ///<summary>
        ///Sets corresponding LED's to differentiate the first and second controller
        ///</summary>
        void Start()
        {
            for (int i = 0; i < joycons.Count; ++i)
            {
                Debug.Log(i);
                Joycon jc = joycons[i];
                byte LEDs = 0x0;
                LEDs |= (byte)(0x1 << i);
                jc.Attach(leds_: LEDs);
                jc.Begin();
            }
        }

        ///<summary>
        ///Updates joycons
        ///</summary>
        void Update()
        {
            for (int i = 0; i < joycons.Count; ++i)
            {
                joycons[i].Update();
            }
        }

        ///<summary>
        ///Disconnects joycons when application ends
        ///</summary>
        void OnApplicationQuit()
        {
            for (int i = 0; i < joycons.Count; ++i)
            {
                joycons[i].Detach();
            }
        }
    }
}
