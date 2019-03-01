﻿/* This file is part of INDISharp, Copyright © 2014-2015 Ilia Platone <info@iliaplatone.com>.
*
*  INDISharp is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  INDISharp is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with INDISharp.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace INDI
{
    #region Custom Event Argument classes
    public class INDICameraBlobEventArgs : EventArgs
	{
		public byte[] ImageData;
		public string Format;
		public string Name;
        public string Vector;

		public INDICameraBlobEventArgs(byte[] imagedata, string name, string vector, string format)
		{
            Vector = vector;
			Name = name;
			Format = format;
			ImageData = imagedata;
		}
    }
    public class INDICameraNumberEventArgs : IsNewNumberEventArgs
    {
        public INDICameraNumberType Type;
        public List<INDINumber> Values;
        public INDICameraNumberEventArgs(NumberVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "CCD_EXPOSURE":
                    Type = INDICameraNumberType.Exposure;
                    break;
                case "CCD_ABORT_EXPOSURE":
                    Type = INDICameraNumberType.AbortExposure;
                    break;
                case "CCD_FRAME":
                    Type = INDICameraNumberType.FrameSize;
                    break;
                case "CCD_TEMPERATURE":
                    Type = INDICameraNumberType.Temperature;
                    break;
                case "CCD_COOLER_POWER":
                    Type = INDICameraNumberType.CoolerPower;
                    break;
                case "CCD_BINNING":
                    Type = INDICameraNumberType.Binning;
                    break;
                case "CCD_INFO":
                    Type = INDICameraNumberType.Informations;
                    break;

                case "TIME_LST":
                    Type = INDICameraNumberType.TimeLst;
                    break;
                case "GEOGRAPHIC_COORD":
                    Type = INDICameraNumberType.GeographicCoord;
                    break;
                case "ATMOSPHERE":
                    Type = INDICameraNumberType.Atmosphere;
                    break;
                default:
                    Type = INDICameraNumberType.Other;
                    break;
            }
        }
    }
    public class INDICameraSwitchEventArgs : IsNewSwitchEventArgs
    {
        public INDICameraSwitchType Type;
        public List<INDISwitch> Values;
        public INDICameraSwitchEventArgs(SwitchVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "CCD_COOLER":
                    Type = INDICameraSwitchType.Cooler;
                    break;
                case "CCD_FRAME_TYPE":
                    Type = INDICameraSwitchType.FrameType;
                    break;
                case "CCD_COMPRESSION":
                    Type = INDICameraSwitchType.Compression;
                    break;
                case "CCD_FRAME_RESET":
                    Type = INDICameraSwitchType.FrameReset;
                    break;

                case "CONNECTION":
                    Type = INDICameraSwitchType.Connection;
                    break;
                case "UPLOAD_MODE":
                    Type = INDICameraSwitchType.UploadMode;
                    break;
                default:
                    Type = INDICameraSwitchType.Other;
                    break;
            }
        }
    }
    public class INDICameraTextEventArgs : IsNewTextEventArgs
    {
        public INDICameraTextType Type;
        public List<INDIText> Values;
        public INDICameraTextEventArgs(TextVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "CCD_CFA":
                    Type = INDICameraTextType.Cfa;
                    break;

                case "DEVICE_PORT":
                    Type = INDICameraTextType.DevicePort;
                    break;
                case "TIME_UTC":
                    Type = INDICameraTextType.TimeUtc;
                    break;
                case "UPLOAD_SETTINGS":
                    Type = INDICameraTextType.UploadSettings;
                    break;
                case "ACTIVE_DEVICES":
                    Type = INDICameraTextType.ActiveDevices;
                    break;
                default:
                    Type = INDICameraTextType.Other;
                    break;
            }
        }
    }
    #endregion
    #region Enums
    public enum INDICameraNumberType
    {
        TimeLst,
        GeographicCoord,
        Atmosphere,
        Other,

        Exposure,
        AbortExposure,
        FrameSize,
        Temperature,
        CoolerPower,
        Binning,
        Informations,
    }
    public enum INDICameraSwitchType
    {
        Connection,
        UploadMode,
        Other,

        Cooler,
        FrameType,
        Compression,
        FrameReset,
    }
    public enum INDICameraTextType
    {
        DevicePort,
        TimeUtc,
        UploadSettings,
        ActiveDevices,
        Other,

        Cfa,
    }
    public enum INDIFrameType
    {
        LIGHT = 0,
        BIAS,
        DARK,
        FLAT
    };
    #endregion

    // TODO: major reworking required
    // empty catch == bad!
    public class INDICamera : INDIDevice
	{
		public event EventHandler<INDICameraBlobEventArgs> IsNewBlob;
        public event EventHandler<INDICameraNumberEventArgs> IsNewNumber;
        public event EventHandler<INDICameraSwitchEventArgs> IsNewSwitch;
        public event EventHandler<INDICameraTextEventArgs> IsNewText;
        #region Constructors / Initialization
        public INDICamera(string name, INDIClient host, bool client = true)
            : base(name, host, client)
        {
            EnableBLOB(true);
            if (!client)
            {
                AddNumberVector(new NumberVector(Name, "CCD_EXPOSURE", "Exposure", "Main Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("CCD_EXPOSURE_VALUE", "Duration (s)", "%5.2f", 0.05, 10000.0, 0.05, 1.0)
            }));
                AddSwitchVector(new SwitchVector(Name, "CCD_ABORT_EXPOSURE", "Expose Abort", "Main Control", "rw", "AtMostOne", new List<INDISwitch>
            {
                new INDISwitch("ABORT", "Abort", false)
            }));
                AddNumberVector(new NumberVector(Name, "CCD_TEMPERATURE", "Temperature", "Main Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("CCD_TEMPERATURE_VALUE", "Temperature (C)", "%5.2f", -50.0, 50.0, 0.0, 20.0)
            }));
                AddNumberVector(new NumberVector(Name, "CCD_FRAME", "Frame", "Image Settings", "rw", "", new List<INDINumber>
            {
                new INDINumber("X", "Left", "%4.0f", 0.0, 16000.0, 1.0, 0.0),
                new INDINumber("Y", "Top", "%4.0f", 0.0, 16000.0, 1.0, 0.0),
                new INDINumber("WIDTH", "Width", "%4.0f", 0.0, 16000.0, 1.0, 16000.0),
                new INDINumber("HEIGHT", "Height", "%4.0f", 0.0, 16000.0, 1.0, 16000.0)
            }));
                AddNumberVector(new NumberVector(Name, "CCD_BINNING", "Binning", "Image Settings", "rw", "", new List<INDINumber>
            {
                new INDINumber("HOR_BIN", "X", "%2.0f", 1.0, 4.0, 1.0, 1.0),
                new INDINumber("VER_BIN", "Y", "%2.0f", 1.0, 4.0, 1.0, 1.0)
            }));
                AddSwitchVector(new SwitchVector(Name, "CCD_COMPRESSION", "Image", "Image Settings", "rw", "OneOfMany", new List<INDISwitch>
            {
                new INDISwitch("CCD_COMPRESS", "Compress", false),
                new INDISwitch("CCD_RAW", "Raw", true)
            }));
                AddSwitchVector(new SwitchVector(Name, "CCD_FRAME_TYPE", "Frame Type", "Image Settings", "rw", "OneOfMany", new List<INDISwitch>
            {
                new INDISwitch("FRAME_LIGHT", "Light", true),
                new INDISwitch("FRAME_BIAS", "Bias", false),
                new INDISwitch("FRAME_DARK", "Dark", false),
                new INDISwitch("FRAME_FLAT", "Flat", false)
            }));
                AddNumberVector(new NumberVector(Name, "CCD_INFO", "CCD Information", "Image Info", "ro", "", new List<INDINumber>
            {
                new INDINumber("CCD_MAX_X", "Resolution x", "%4.0f", 1.0, 16000.0, 0.0, 16000.0),
                new INDINumber("CCD_MAX_Y", "Resolution y", "%4.0f", 1.0, 16000.0, 0.0, 16000.0),
                new INDINumber("CCD_PIXEL_SIZE", "Pixel size (um)", "%5.2f", 1.0, 40.0, 0.0, 9.0),
                new INDINumber("CCD_PIXEL_SIZE_X", "Pixel size X", "%5.2f", 1.0, 40.0, 0.0, 9.0),
                new INDINumber("CCD_PIXEL_SIZE_Y", "Pixel size Y", "%5.2f", 1.0, 40.0, 0.0, 9.0),
                new INDINumber("CCD_BITSPERPIXEL", "Bits per pixel", "%3.0f", 8.0, 64.0, 0.0, 16.0)
            }));
                AddTextVector(new TextVector(Name, "CCD_CFA", "Bayer Info", "Image Info", "ro", "", new List<INDIText>
            {
                new INDIText("CFA_OFFSET_X", "X Offset", "0"),
                new INDIText("CFA_OFFSET_Y", "Y Offset", "0"),
                new INDIText("CFA_TYPE", "Filter", "")
            }));
                AddBlobVector(new BlobVector(Name, "CCD", "Image Data", "Image Streams", "ro", "", new List<INDIBlob>
                {
                    new INDIBlob("CCD1", Name + " image data", ".fits", new byte[1], 1),
                }));
                DriverInterface |= DRIVER_INTERFACE.CCD_INTERFACE;
            }
        }
        #endregion

        #region Standard Methods
        public void StartExposure(double duration, INDIFrameType type)
        {
            try
            {
                SetSwitchVector("CCD_FRAME_TYPE", (int)type);
                SetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE", duration);
            }
            catch { }
        }

        public void ResetFrame()
        {
            try
            {
                SetSwitch("CCD_FRAME_RESET", "RESET", true);
            }
            catch { }
        }

        public void Abort()
        {
            try
            {
                SetSwitch("CCD_ABORT_EXPOSURE", "ABORT", true);
            }
            catch { }
        }

        public override void isNewNumber(object sender, IsNewNumberEventArgs e)
        {
            base.isNewNumber(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewNumber?.Invoke(this, new INDICameraNumberEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void isNewSwitch(object sender, IsNewSwitchEventArgs e)
        {
            base.isNewSwitch(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewSwitch?.Invoke(this, new INDICameraSwitchEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void isNewText(object sender, IsNewTextEventArgs e)
        {
            base.isNewText(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewText?.Invoke(this, new INDICameraTextEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void isNewBlob(object sender, IsNewBlobEventArgs e)
        {
            base.isNewBlob(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    INDIClient caller = (INDIClient)sender;
                    for (int i = 0; i < e.Vector.Values.Count; i++)
                    {
                        Console.WriteLine("Received BLOB " + e.Vector.Values[i].Name + " of size " + e.Vector.Values[i].Size + " from device " + e.Device + "@" + caller.Address + ":" + caller.Port);
                        IsNewBlob?.Invoke(this, new INDICameraBlobEventArgs(e.Vector.Values[i].Value, e.Vector.Values[i].Name, e.Vector.Name, e.Vector.Values[0].Format));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Standard Properties
        public bool CompessionEnabled
        {
            set
            {
                try
                {
                    SetSwitch("CCD_COMPRESSION", "CCD_COMPRESSION_ON", value);
                }
                catch
                {
                }
            }
            get
            {
                try
                {
                    return GetSwitch("CCD_COMPRESSION", "CCD_COMPRESSION_ON").Value;
                }
                catch
                {
                }
                return false;
            }
        }

        public int BayerX
        {
            get
            {
                try
                {
                    return int.Parse(GetText("CCD_CFA", "CFA_OFFSET_X").Value);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int BayerY
        {
            get
            {
                try
                {
                    return int.Parse(GetText("CCD_CFA", "CFA_OFFSET_Y").Value);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double ExposureTime
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").Value;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public bool CoolerStarted
        {
            get
            {
                try
                {
                    return GetSwitch("CCD_COOLER", "COOLER_ON").Value;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("CCD_COOLER", value ? 0 : 1);
                }
                catch { }
            }
        }

        public double CoolerPower
        {
            get
            {
                try
                {
                    return GetNumber("CCD_COOLER_POWER", "CCD_COOLER_VALUE").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_COOLER_POWER", "CCD_COOLER_VALUE", value);
                }
                catch { }
            }
        }

        public bool CanSetCoolerPower
        {
            get
            {
                try
                {
                    return GetNumberVector("CCD_COOLER_POWER").Permission == "rw";
                }
                catch
                {
                    return false;
                }
            }
        }

        public int BinX
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_BINNING", "HOR_BIN").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_BINNING", "HOR_BIN", value);
                }
                catch
                {
                }
            }
        }

        public int BinY
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_BINNING", "VER_BIN").Value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_BINNING", "VER_BIN", value);
                }
                catch
                {
                }
            }
        }

        public int MaxBinX
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_BINNING", "HOR_BIN").Max;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int MaxBinY
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_BINNING", "VER_BIN").Max;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int StartX
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_FRAME", "X").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "X", value);
                }
                catch
                {
                }
            }
        }

        public int StartY
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_FRAME", "Y").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "Y", value);
                }
                catch
                {
                }
            }
        }

        public int Width
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_FRAME", "WIDTH").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "WIDTH", value);
                }
                catch
                {
                }
            }
        }

        public int Height
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_FRAME", "HEIGHT").Value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "HEIGHT", value);
                }
                catch
                {
                }
            }
        }

        public int FullWidth
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_INFO", "CCD_MAX_X").Value;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int FullHeight
        {
            get
            {
                try
                {
                    return (int)GetNumber("CCD_INFO", "CCD_MAX_Y").Value;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double MinExposure
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").Min;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double MaxExposure
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").Max;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double ExposureStep
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").Step;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double MaxADU
        {
            get
            {
                try
                {
                    return 65536;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double PixelSizeX
        {
            get
            {
                try
                {
                    return GetNumber("CCD_INFO", "CCD_PIXEL_SIZE_X").Value;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double PixelSizeY
        {
            get
            {
                try
                {
                    return GetNumber("CCD_INFO", "CCD_PIXEL_SIZE_Y").Value;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public double CCDTemperature
        {
            get
            {
                try
                {
                    return GetNumber("CCD_TEMPERATURE", "CCD_TEMPERATURE_VALUE").Value;
                }
                catch
                {
                    return 0.0;
                }
            }
            set
            {
                try
                {
                    if (CanSetCCDTemperature)
                    {
                        SetNumber("CCD_TEMPERATURE", "CCD_TEMPERATURE_VALUE", value);
                    }
                }
                catch
                {
                }
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                try
                {
                    return (GetNumberVector("CCD_TEMPERATURE").Permission == "rw");
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion
    }
}

