using CoreAudioApi.Interfaces;

/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Runtime.InteropServices;

namespace CoreAudioApi
{
    public class MMDevice : IDisposable
    {
        #region Variables

        private readonly IMMDevice _RealDevice;
        private PropertyStore _PropertyStore;
        private AudioMeterInformation _AudioMeterInformation;
        private AudioEndpointVolume _AudioEndpointVolume;
        private AudioSessionManager _AudioSessionManager;

        #endregion Variables

        #region Guids

        private static Guid IID_IAudioMeterInformation = typeof(IAudioMeterInformation).GUID;
        private static Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;
        private static Guid IID_IAudioSessionManager = typeof(IAudioSessionManager2).GUID;

        #endregion Guids

        #region Init

        private void GetPropertyInformation()
        {
            IPropertyStore propstore;
            Marshal.ThrowExceptionForHR(_RealDevice.OpenPropertyStore(EStgmAccess.STGM_READ, out propstore));
            _PropertyStore = new PropertyStore(propstore);
        }

        private void GetAudioSessionManager()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioSessionManager, CLSCTX.ALL, IntPtr.Zero, out result));
            _AudioSessionManager = new AudioSessionManager(result as IAudioSessionManager2);
        }

        private void GetAudioMeterInformation()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioMeterInformation, CLSCTX.ALL, IntPtr.Zero, out result));
            _AudioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }

        private void GetAudioEndpointVolume()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioEndpointVolume, CLSCTX.ALL, IntPtr.Zero, out result));
            _AudioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
        }

        #endregion Init

        #region Properties

        public AudioSessionManager AudioSessionManager
        {
            get
            {
                if (_AudioSessionManager == null)
                    GetAudioSessionManager();

                return _AudioSessionManager;
            }
        }

        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (_AudioMeterInformation == null)
                    GetAudioMeterInformation();

                return _AudioMeterInformation;
            }
        }

        public AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                if (_AudioEndpointVolume == null)
                    GetAudioEndpointVolume();

                return _AudioEndpointVolume;
            }
        }

        public PropertyStore Properties
        {
            get
            {
                if (_PropertyStore == null)
                    GetPropertyInformation();
                return _PropertyStore;
            }
        }

        public string FriendlyName
        {
            get
            {
                if (_PropertyStore == null)
                    GetPropertyInformation();
                if (_PropertyStore.Contains(Pkey.PkeyDeviceInterfaceFriendlyName))
                {
                    return (string)_PropertyStore[Pkey.PkeyDeviceInterfaceFriendlyName].Value;
                }
                else
                    return "Unknown";
            }
        }

        public string ID
        {
            get
            {
                string Result;
                Marshal.ThrowExceptionForHR(_RealDevice.GetId(out Result));
                return Result;
            }
        }

        public EDataFlow DataFlow
        {
            get
            {
                EDataFlow Result;
                IMMEndpoint ep = _RealDevice as IMMEndpoint;
                ep.GetDataFlow(out Result);
                return Result;
            }
        }

        public EDeviceState State
        {
            get
            {
                EDeviceState Result;
                Marshal.ThrowExceptionForHR(_RealDevice.GetState(out Result));
                return Result;
            }
        }

        #endregion Properties

        #region Constructor

        internal MMDevice(IMMDevice realDevice)
        {
            _RealDevice = realDevice;
        }

        #endregion Constructor

        #region Dispose

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are.
        ~MMDevice()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources here
                if (_AudioEndpointVolume != null)
                {
                    _AudioEndpointVolume.Dispose();
                }
            }
            // free native resources here if there are any.
        }

        #endregion Dispose
    }
}