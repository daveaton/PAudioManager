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

namespace CoreAudioApi
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct PropVariant
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        private readonly short vt;

//        [FieldOffset(2)]
//        private readonly short wReserved1;

//        [FieldOffset(4)]
//        private readonly short wReserved2;

//        [FieldOffset(6)]
//        private readonly short wReserved3;

//        [FieldOffset(8)]
//        private readonly sbyte cVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private readonly byte bVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private short iVal;

//        [FieldOffset(8)]
 //       private readonly ushort uiVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private readonly int lVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private readonly uint ulVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private readonly long hVal;

//        [FieldOffset(8)]
 //       private readonly ulong uhVal;

//        [FieldOffset(8)]
//        private readonly float fltVal;

//        [FieldOffset(8)]
//        private readonly double dblVal;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private Interfaces.Blob blobVal;

//        [FieldOffset(8)]
//        private readonly DateTime date;

//        [FieldOffset(8)]
//        private readonly bool boolVal;

//        [FieldOffset(8)]
//        private readonly int scode;

//        [FieldOffset(8)]
//        private readonly FILETIME filetime;

        [System.Runtime.InteropServices.FieldOffset(8)]
        private readonly System.IntPtr everything_else;

        //I'm sure there is a more efficient way to do this but this works ..for now..
        internal byte[] GetBlob()
        {
            byte[] Result = new byte[blobVal.Length];
            for (int i = 0; i < blobVal.Length; i++)
            {
                Result[i] = System.Runtime.InteropServices.Marshal.ReadByte((System.IntPtr)((long)(blobVal.Data) + i));
            }
            return Result;
        }

        public object Value
        {
            get
            {
                System.Runtime.InteropServices.VarEnum ve = (System.Runtime.InteropServices.VarEnum)vt;
                switch (ve)
                {
                    case System.Runtime.InteropServices.VarEnum.VT_I1:
                        return bVal;

                    case System.Runtime.InteropServices.VarEnum.VT_I2:
                        return iVal;

                    case System.Runtime.InteropServices.VarEnum.VT_I4:
                        return lVal;

                    case System.Runtime.InteropServices.VarEnum.VT_I8:
                        return hVal;

                    case System.Runtime.InteropServices.VarEnum.VT_INT:
                        return iVal;

                    case System.Runtime.InteropServices.VarEnum.VT_UI4:
                        return ulVal;

                    case System.Runtime.InteropServices.VarEnum.VT_LPWSTR:
                        return System.Runtime.InteropServices.Marshal.PtrToStringUni(everything_else);

                    case System.Runtime.InteropServices.VarEnum.VT_BLOB:
                        return GetBlob();
                }
                return "FIXME Type = " + ve;
            }
        }
    }
}