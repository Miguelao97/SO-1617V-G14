using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPG_SimpleSearchClient
{
    class ExifUtils
    {
        private delegate String TagValueToString(short type, int len, byte[] val);
        private struct TagDescriptor
        {
            public readonly int id;
            public readonly string name;
            public readonly TagValueToString processor;

            public TagDescriptor(int id, string name, TagValueToString processor)
            {
                this.id = id; this.name = name; this.processor = processor;
            }
        }

        public static readonly int Exif_Image_ProcessingSoftware = 11; // 0x000b
        public static readonly int Exif_Image_NewSubfileType = 254; // 0x00fe
        public static readonly int Exif_Image_SubfileType = 255; // 0x00ff
        public static readonly int Exif_Image_ImageWidth = 256; // 0x0100
        public static readonly int Exif_Image_ImageLength = 257; // 0x0101
        public static readonly int Exif_Image_BitsPerSample = 258; // 0x0102
        public static readonly int Exif_Image_Compression = 259; // 0x0103
        public static readonly int Exif_Image_PhotometricInterpretation = 262; // 0x0106
        public static readonly int Exif_Image_Thresholding = 263; // 0x0107
        public static readonly int Exif_Image_CellWidth = 264; // 0x0108
        public static readonly int Exif_Image_CellLength = 265; // 0x0109
        public static readonly int Exif_Image_FillOrder = 266; // 0x010a
        public static readonly int Exif_Image_DocumentName = 269; // 0x010d
        public static readonly int Exif_Image_ImageDescription = 270; // 0x010e
        public static readonly int Exif_Image_Make = 271; // 0x010f
        public static readonly int Exif_Image_Model = 272; // 0x0110
        public static readonly int Exif_Image_StripOffsets = 273; // 0x0111
        public static readonly int Exif_Image_Orientation = 274; // 0x0112
        public static readonly int Exif_Image_SamplesPerPixel = 277; // 0x0115
        public static readonly int Exif_Image_RowsPerStrip = 278; // 0x0116
        public static readonly int Exif_Image_StripByteCounts = 279; // 0x0117
        public static readonly int Exif_Image_XResolution = 282; // 0x011a
        public static readonly int Exif_Image_YResolution = 283; // 0x011b
        public static readonly int Exif_Image_PlanarConfiguration = 284; // 0x011c
        public static readonly int Exif_Image_GrayResponseUnit = 290; // 0x0122
        public static readonly int Exif_Image_GrayResponseCurve = 291; // 0x0123
        public static readonly int Exif_Image_T4Options = 292; // 0x0124
        public static readonly int Exif_Image_T6Options = 293; // 0x0125
        public static readonly int Exif_Image_ResolutionUnit = 296; // 0x0128
        public static readonly int Exif_Image_PageNumber = 297; // 0x0129
        public static readonly int Exif_Image_TransferFunction = 301; // 0x012d
        public static readonly int Exif_Image_Software = 305; // 0x0131
        public static readonly int Exif_Image_DateTime = 306; // 0x0132
        public static readonly int Exif_Image_Artist = 315; // 0x013b
        public static readonly int Exif_Image_HostComputer = 316; // 0x013c
        public static readonly int Exif_Image_Predictor = 317; // 0x013d
        public static readonly int Exif_Image_WhitePoint = 318; // 0x013e
        public static readonly int Exif_Image_PrimaryChromaticities = 319; // 0x013f
        public static readonly int Exif_Image_ColorMap = 320; // 0x0140
        public static readonly int Exif_Image_HalftoneHints = 321; // 0x0141
        public static readonly int Exif_Image_TileWidth = 322; // 0x0142
        public static readonly int Exif_Image_TileLength = 323; // 0x0143
        public static readonly int Exif_Image_TileOffsets = 324; // 0x0144
        public static readonly int Exif_Image_TileByteCounts = 325; // 0x0145
        public static readonly int Exif_Image_SubIFDs = 330; // 0x014a
        public static readonly int Exif_Image_InkSet = 332; // 0x014c
        public static readonly int Exif_Image_InkNames = 333; // 0x014d
        public static readonly int Exif_Image_NumberOfInks = 334; // 0x014e
        public static readonly int Exif_Image_DotRange = 336; // 0x0150
        public static readonly int Exif_Image_TargetPrinter = 337; // 0x0151
        public static readonly int Exif_Image_ExtraSamples = 338; // 0x0152
        public static readonly int Exif_Image_SampleFormat = 339; // 0x0153
        public static readonly int Exif_Image_SMinSampleValue = 340; // 0x0154
        public static readonly int Exif_Image_SMaxSampleValue = 341; // 0x0155
        public static readonly int Exif_Image_TransferRange = 342; // 0x0156
        public static readonly int Exif_Image_ClipPath = 343; // 0x0157
        public static readonly int Exif_Image_XClipPathUnits = 344; // 0x0158
        public static readonly int Exif_Image_YClipPathUnits = 345; // 0x0159
        public static readonly int Exif_Image_Indexed = 346; // 0x015a
        public static readonly int Exif_Image_JPEGTables = 347; // 0x015b
        public static readonly int Exif_Image_OPIProxy = 351; // 0x015f
        public static readonly int Exif_Image_JPEGProc = 512; // 0x0200
        public static readonly int Exif_Image_JPEGInterchangeFormat = 513; // 0x0201
        public static readonly int Exif_Image_JPEGInterchangeFormatLength = 514; // 0x0202
        public static readonly int Exif_Image_JPEGRestartInterval = 515; // 0x0203
        public static readonly int Exif_Image_JPEGLosslessPredictors = 517; // 0x0205
        public static readonly int Exif_Image_JPEGPointTransforms = 518; // 0x0206
        public static readonly int Exif_Image_JPEGQTables = 519; // 0x0207
        public static readonly int Exif_Image_JPEGDCTables = 520; // 0x0208
        public static readonly int Exif_Image_JPEGACTables = 521; // 0x0209
        public static readonly int Exif_Image_YCbCrCoefficients = 529; // 0x0211
        public static readonly int Exif_Image_YCbCrSubSampling = 530; // 0x0212
        public static readonly int Exif_Image_YCbCrPositioning = 531; // 0x0213
        public static readonly int Exif_Image_ReferenceBlackWhite = 532; // 0x0214
        public static readonly int Exif_Image_XMLPacket = 700; // 0x02bc
        public static readonly int Exif_Image_Rating = 18246; // 0x4746
        public static readonly int Exif_Image_RatingPercent = 18249; // 0x4749
        public static readonly int Exif_Image_ImageID = 32781; // 0x800d
        public static readonly int Exif_Image_CFARepeatPatternDim = 33421; // 0x828d
        public static readonly int Exif_Image_CFAPattern = 33422; // 0x828e
        public static readonly int Exif_Image_BatteryLevel = 33423; // 0x828f
        public static readonly int Exif_Image_Copyright = 33432; // 0x8298
        public static readonly int Exif_Image_ExposureTime = 33434; // 0x829a
        public static readonly int Exif_Image_FNumber = 33437; // 0x829d
        public static readonly int Exif_Image_IPTCNAA = 33723; // 0x83bb
        public static readonly int Exif_Image_ImageResources = 34377; // 0x8649
        public static readonly int Exif_Image_ExifTag = 34665; // 0x8769
        public static readonly int Exif_Image_InterColorProfile = 34675; // 0x8773
        public static readonly int Exif_Image_ExposureProgram = 34850; // 0x8822
        public static readonly int Exif_Image_SpectralSensitivity = 34852; // 0x8824
        public static readonly int Exif_Image_GPSTag = 34853; // 0x8825
        public static readonly int Exif_Image_ISOSpeedRatings = 34855; // 0x8827
        public static readonly int Exif_Image_OECF = 34856; // 0x8828
        public static readonly int Exif_Image_Interlace = 34857; // 0x8829
        public static readonly int Exif_Image_TimeZoneOffset = 34858; // 0x882a
        public static readonly int Exif_Image_SelfTimerMode = 34859; // 0x882b
        public static readonly int Exif_Image_DateTimeOriginal = 36867; // 0x9003
        public static readonly int Exif_Image_CompressedBitsPerPixel = 37122; // 0x9102
        public static readonly int Exif_Image_ShutterSpeedValue = 37377; // 0x9201
        public static readonly int Exif_Image_ApertureValue = 37378; // 0x9202
        public static readonly int Exif_Image_BrightnessValue = 37379; // 0x9203
        public static readonly int Exif_Image_ExposureBiasValue = 37380; // 0x9204
        public static readonly int Exif_Image_MaxApertureValue = 37381; // 0x9205
        public static readonly int Exif_Image_SubjectDistance = 37382; // 0x9206
        public static readonly int Exif_Image_MeteringMode = 37383; // 0x9207
        public static readonly int Exif_Image_LightSource = 37384; // 0x9208
        public static readonly int Exif_Image_Flash = 37385; // 0x9209
        public static readonly int Exif_Image_FocalLength = 37386; // 0x920a
        public static readonly int Exif_Image_FlashEnergy = 37387; // 0x920b
        public static readonly int Exif_Image_SpatialFrequencyResponse = 37388; // 0x920c
        public static readonly int Exif_Image_Noise = 37389; // 0x920d
        public static readonly int Exif_Image_FocalPlaneXResolution = 37390; // 0x920e
        public static readonly int Exif_Image_FocalPlaneYResolution = 37391; // 0x920f
        public static readonly int Exif_Image_FocalPlaneResolutionUnit = 37392; // 0x9210
        public static readonly int Exif_Image_ImageNumber = 37393; // 0x9211
        public static readonly int Exif_Image_SecurityClassification = 37394; // 0x9212
        public static readonly int Exif_Image_ImageHistory = 37395; // 0x9213
        public static readonly int Exif_Image_SubjectLocation = 37396; // 0x9214
        public static readonly int Exif_Image_ExposureIndex = 37397; // 0x9215
        public static readonly int Exif_Image_TIFFEPStandardID = 37398; // 0x9216
        public static readonly int Exif_Image_SensingMethod = 37399; // 0x9217
        public static readonly int Exif_Image_XPTitle = 40091; // 0x9c9b
        public static readonly int Exif_Image_XPComment = 40092; // 0x9c9c
        public static readonly int Exif_Image_XPAuthor = 40093; // 0x9c9d
        public static readonly int Exif_Image_XPKeywords = 40094; // 0x9c9e
        public static readonly int Exif_Image_XPSubject = 40095; // 0x9c9f
        public static readonly int Exif_Image_PrintImageMatching = 50341; // 0xc4a5
        public static readonly int Exif_Image_DNGVersion = 50706; // 0xc612
        public static readonly int Exif_Image_DNGBackwardVersion = 50707; // 0xc613
        public static readonly int Exif_Image_UniqueCameraModel = 50708; // 0xc614
        public static readonly int Exif_Image_LocalizedCameraModel = 50709; // 0xc615
        public static readonly int Exif_Image_CFAPlaneColor = 50710; // 0xc616
        public static readonly int Exif_Image_CFALayout = 50711; // 0xc617
        public static readonly int Exif_Image_LinearizationTable = 50712; // 0xc618
        public static readonly int Exif_Image_BlackLevelRepeatDim = 50713; // 0xc619
        public static readonly int Exif_Image_BlackLevel = 50714; // 0xc61a
        public static readonly int Exif_Image_BlackLevelDeltaH = 50715; // 0xc61b
        public static readonly int Exif_Image_BlackLevelDeltaV = 50716; // 0xc61c
        public static readonly int Exif_Image_WhiteLevel = 50717; // 0xc61d
        public static readonly int Exif_Image_DefaultScale = 50718; // 0xc61e
        public static readonly int Exif_Image_DefaultCropOrigin = 50719; // 0xc61f
        public static readonly int Exif_Image_DefaultCropSize = 50720; // 0xc620
        public static readonly int Exif_Image_ColorMatrix1 = 50721; // 0xc621
        public static readonly int Exif_Image_ColorMatrix2 = 50722; // 0xc622
        public static readonly int Exif_Image_CameraCalibration1 = 50723; // 0xc623
        public static readonly int Exif_Image_CameraCalibration2 = 50724; // 0xc624
        public static readonly int Exif_Image_ReductionMatrix1 = 50725; // 0xc625
        public static readonly int Exif_Image_ReductionMatrix2 = 50726; // 0xc626
        public static readonly int Exif_Image_AnalogBalance = 50727; // 0xc627
        public static readonly int Exif_Image_AsShotNeutral = 50728; // 0xc628
        public static readonly int Exif_Image_AsShotWhiteXY = 50729; // 0xc629
        public static readonly int Exif_Image_BaselineExposure = 50730; // 0xc62a
        public static readonly int Exif_Image_BaselineNoise = 50731; // 0xc62b
        public static readonly int Exif_Image_BaselineSharpness = 50732; // 0xc62c
        public static readonly int Exif_Image_BayerGreenSplit = 50733; // 0xc62d
        public static readonly int Exif_Image_LinearResponseLimit = 50734; // 0xc62e
        public static readonly int Exif_Image_CameraSerialNumber = 50735; // 0xc62f
        public static readonly int Exif_Image_LensInfo = 50736; // 0xc630
        public static readonly int Exif_Image_ChromaBlurRadius = 50737; // 0xc631
        public static readonly int Exif_Image_AntiAliasStrength = 50738; // 0xc632
        public static readonly int Exif_Image_ShadowScale = 50739; // 0xc633
        public static readonly int Exif_Image_DNGPrivateData = 50740; // 0xc634
        public static readonly int Exif_Image_MakerNoteSafety = 50741; // 0xc635
        public static readonly int Exif_Image_CalibrationIlluminant1 = 50778; // 0xc65a
        public static readonly int Exif_Image_CalibrationIlluminant2 = 50779; // 0xc65b
        public static readonly int Exif_Image_BestQualityScale = 50780; // 0xc65c
        public static readonly int Exif_Image_RawDataUniqueID = 50781; // 0xc65d
        public static readonly int Exif_Image_OriginalRawFileName = 50827; // 0xc68b
        public static readonly int Exif_Image_OriginalRawFileData = 50828; // 0xc68c
        public static readonly int Exif_Image_ActiveArea = 50829; // 0xc68d
        public static readonly int Exif_Image_MaskedAreas = 50830; // 0xc68e
        public static readonly int Exif_Image_AsShotICCProfile = 50831; // 0xc68f
        public static readonly int Exif_Image_AsShotPreProfileMatrix = 50832; // 0xc690
        public static readonly int Exif_Image_CurrentICCProfile = 50833; // 0xc691
        public static readonly int Exif_Image_CurrentPreProfileMatrix = 50834; // 0xc692
        public static readonly int Exif_Image_ColorimetricReference = 50879; // 0xc6bf
        public static readonly int Exif_Image_CameraCalibrationSignature = 50931; // 0xc6f3
        public static readonly int Exif_Image_ProfileCalibrationSignature = 50932; // 0xc6f4
        public static readonly int Exif_Image_AsShotProfileName = 50934; // 0xc6f6
        public static readonly int Exif_Image_NoiseReductionApplied = 50935; // 0xc6f7
        public static readonly int Exif_Image_ProfileName = 50936; // 0xc6f8
        public static readonly int Exif_Image_ProfileHueSatMapDims = 50937; // 0xc6f9
        public static readonly int Exif_Image_ProfileHueSatMapData1 = 50938; // 0xc6fa
        public static readonly int Exif_Image_ProfileHueSatMapData2 = 50939; // 0xc6fb
        public static readonly int Exif_Image_ProfileToneCurve = 50940; // 0xc6fc
        public static readonly int Exif_Image_ProfileEmbedPolicy = 50941; // 0xc6fd
        public static readonly int Exif_Image_ProfileCopyright = 50942; // 0xc6fe
        public static readonly int Exif_Image_ForwardMatrix1 = 50964; // 0xc714
        public static readonly int Exif_Image_ForwardMatrix2 = 50965; // 0xc715
        public static readonly int Exif_Image_PreviewApplicationName = 50966; // 0xc716
        public static readonly int Exif_Image_PreviewApplicationVersion = 50967; // 0xc717
        public static readonly int Exif_Image_PreviewSettingsName = 50968; // 0xc718
        public static readonly int Exif_Image_PreviewSettingsDigest = 50969; // 0xc719
        public static readonly int Exif_Image_PreviewColorSpace = 50970; // 0xc71a
        public static readonly int Exif_Image_PreviewDateTime = 50971; // 0xc71b
        public static readonly int Exif_Image_RawImageDigest = 50972; // 0xc71c
        public static readonly int Exif_Image_OriginalRawFileDigest = 50973; // 0xc71d
        public static readonly int Exif_Image_SubTileBlockSize = 50974; // 0xc71e
        public static readonly int Exif_Image_RowInterleaveFactor = 50975; // 0xc71f
        public static readonly int Exif_Image_ProfileLookTableDims = 50981; // 0xc725
        public static readonly int Exif_Image_ProfileLookTableData = 50982; // 0xc726
        public static readonly int Exif_Image_OpcodeList1 = 51008; // 0xc740
        public static readonly int Exif_Image_OpcodeList2 = 51009; // 0xc741
        public static readonly int Exif_Image_OpcodeList3 = 51022; // 0xc74e
        public static readonly int Exif_Image_NoiseProfile = 51041; // 0xc761
        public static readonly int Exif_Photo_ExposureTime = 33434; // 0x829a
        public static readonly int Exif_Photo_FNumber = 33437; // 0x829d
        public static readonly int Exif_Photo_ExposureProgram = 34850; // 0x8822
        public static readonly int Exif_Photo_SpectralSensitivity = 34852; // 0x8824
        public static readonly int Exif_Photo_ISOSpeedRatings = 34855; // 0x8827
        public static readonly int Exif_Photo_OECF = 34856; // 0x8828
        public static readonly int Exif_Photo_SensitivityType = 34864; // 0x8830
        public static readonly int Exif_Photo_StandardOutputSensitivity = 34865; // 0x8831
        public static readonly int Exif_Photo_RecommendedExposureIndex = 34866; // 0x8832
        public static readonly int Exif_Photo_ISOSpeed = 34867; // 0x8833
        public static readonly int Exif_Photo_ISOSpeedLatitudeyyy = 34868; // 0x8834
        public static readonly int Exif_Photo_ISOSpeedLatitudezzz = 34869; // 0x8835
        public static readonly int Exif_Photo_ExifVersion = 36864; // 0x9000
        public static readonly int Exif_Photo_DateTimeOriginal = 36867; // 0x9003
        public static readonly int Exif_Photo_DateTimeDigitized = 36868; // 0x9004
        public static readonly int Exif_Photo_ComponentsConfiguration = 37121; // 0x9101
        public static readonly int Exif_Photo_CompressedBitsPerPixel = 37122; // 0x9102
        public static readonly int Exif_Photo_ShutterSpeedValue = 37377; // 0x9201
        public static readonly int Exif_Photo_ApertureValue = 37378; // 0x9202
        public static readonly int Exif_Photo_BrightnessValue = 37379; // 0x9203
        public static readonly int Exif_Photo_ExposureBiasValue = 37380; // 0x9204
        public static readonly int Exif_Photo_MaxApertureValue = 37381; // 0x9205
        public static readonly int Exif_Photo_SubjectDistance = 37382; // 0x9206
        public static readonly int Exif_Photo_MeteringMode = 37383; // 0x9207
        public static readonly int Exif_Photo_LightSource = 37384; // 0x9208
        public static readonly int Exif_Photo_Flash = 37385; // 0x9209
        public static readonly int Exif_Photo_FocalLength = 37386; // 0x920a
        public static readonly int Exif_Photo_SubjectArea = 37396; // 0x9214
        public static readonly int Exif_Photo_MakerNote = 37500; // 0x927c
        public static readonly int Exif_Photo_UserComment = 37510; // 0x9286
        public static readonly int Exif_Photo_SubSecTime = 37520; // 0x9290
        public static readonly int Exif_Photo_SubSecTimeOriginal = 37521; // 0x9291
        public static readonly int Exif_Photo_SubSecTimeDigitized = 37522; // 0x9292
        public static readonly int Exif_Photo_FlashpixVersion = 40960; // 0xa000
        public static readonly int Exif_Photo_ColorSpace = 40961; // 0xa001
        public static readonly int Exif_Photo_PixelXDimension = 40962; // 0xa002
        public static readonly int Exif_Photo_PixelYDimension = 40963; // 0xa003
        public static readonly int Exif_Photo_RelatedSoundFile = 40964; // 0xa004
        public static readonly int Exif_Photo_InteroperabilityTag = 40965; // 0xa005
        public static readonly int Exif_Photo_FlashEnergy = 41483; // 0xa20b
        public static readonly int Exif_Photo_SpatialFrequencyResponse = 41484; // 0xa20c
        public static readonly int Exif_Photo_FocalPlaneXResolution = 41486; // 0xa20e
        public static readonly int Exif_Photo_FocalPlaneYResolution = 41487; // 0xa20f
        public static readonly int Exif_Photo_FocalPlaneResolutionUnit = 41488; // 0xa210
        public static readonly int Exif_Photo_SubjectLocation = 41492; // 0xa214
        public static readonly int Exif_Photo_ExposureIndex = 41493; // 0xa215
        public static readonly int Exif_Photo_SensingMethod = 41495; // 0xa217
        public static readonly int Exif_Photo_FileSource = 41728; // 0xa300
        public static readonly int Exif_Photo_SceneType = 41729; // 0xa301
        public static readonly int Exif_Photo_CFAPattern = 41730; // 0xa302
        public static readonly int Exif_Photo_CustomRendered = 41985; // 0xa401
        public static readonly int Exif_Photo_ExposureMode = 41986; // 0xa402
        public static readonly int Exif_Photo_WhiteBalance = 41987; // 0xa403
        public static readonly int Exif_Photo_DigitalZoomRatio = 41988; // 0xa404
        public static readonly int Exif_Photo_FocalLengthIn35mmFilm = 41989; // 0xa405
        public static readonly int Exif_Photo_SceneCaptureType = 41990; // 0xa406
        public static readonly int Exif_Photo_GainControl = 41991; // 0xa407
        public static readonly int Exif_Photo_Contrast = 41992; // 0xa408
        public static readonly int Exif_Photo_Saturation = 41993; // 0xa409
        public static readonly int Exif_Photo_Sharpness = 41994; // 0xa40a
        public static readonly int Exif_Photo_DeviceSettingDescription = 41995; // 0xa40b
        public static readonly int Exif_Photo_SubjectDistanceRange = 41996; // 0xa40c
        public static readonly int Exif_Photo_ImageUniqueID = 42016; // 0xa420
        public static readonly int Exif_Photo_CameraOwnerName = 42032; // 0xa430
        public static readonly int Exif_Photo_BodySerialNumber = 42033; // 0xa431
        public static readonly int Exif_Photo_LensSpecification = 42034; // 0xa432
        public static readonly int Exif_Photo_LensMake = 42035; // 0xa433
        public static readonly int Exif_Photo_LensModel = 42036; // 0xa434
        public static readonly int Exif_Photo_LensSerialNumber = 42037; // 0xa435
         
        public static readonly int Exif_GPSInfo_GPSVersionID = 0; // 0x0000
        public static readonly int Exif_GPSInfo_GPSLatitudeRef = 1; // 0x0001
        public static readonly int Exif_GPSInfo_GPSLatitude = 2; // 0x0002
        public static readonly int Exif_GPSInfo_GPSLongitudeRef = 3; // 0x0003
        public static readonly int Exif_GPSInfo_GPSLongitude = 4; // 0x0004
        public static readonly int Exif_GPSInfo_GPSAltitudeRef = 5; // 0x0005
        public static readonly int Exif_GPSInfo_GPSAltitude = 6; // 0x0006
        public static readonly int Exif_GPSInfo_GPSTimeStamp = 7; // 0x0007
        public static readonly int Exif_GPSInfo_GPSSatellites = 8; // 0x0008
        public static readonly int Exif_GPSInfo_GPSStatus = 9; // 0x0009
        public static readonly int Exif_GPSInfo_GPSMeasureMode = 10; // 0x000a
        public static readonly int Exif_GPSInfo_GPSDOP = 11; // 0x000b
        public static readonly int Exif_GPSInfo_GPSSpeedRef = 12; // 0x000c
        public static readonly int Exif_GPSInfo_GPSSpeed = 13; // 0x000d
        public static readonly int Exif_GPSInfo_GPSTrackRef = 14; // 0x000e
        public static readonly int Exif_GPSInfo_GPSTrack = 15; // 0x000f
        public static readonly int Exif_GPSInfo_GPSImgDirectionRef = 16; // 0x0010
        public static readonly int Exif_GPSInfo_GPSImgDirection = 17; // 0x0011
        public static readonly int Exif_GPSInfo_GPSMapDatum = 18; // 0x0012
        public static readonly int Exif_GPSInfo_GPSDestLatitudeRef = 19; // 0x0013
        public static readonly int Exif_GPSInfo_GPSDestLatitude = 20; // 0x0014
        public static readonly int Exif_GPSInfo_GPSDestLongitudeRef = 21; // 0x0015
        public static readonly int Exif_GPSInfo_GPSDestLongitude = 22; // 0x0016
        public static readonly int Exif_GPSInfo_GPSDestBearingRef = 23; // 0x0017
        public static readonly int Exif_GPSInfo_GPSDestBearing = 24; // 0x0018
        public static readonly int Exif_GPSInfo_GPSDestDistanceRef = 25; // 0x0019
        public static readonly int Exif_GPSInfo_GPSDestDistance = 26; // 0x001a
        public static readonly int Exif_GPSInfo_GPSProcessingMethod = 27; // 0x001b
        public static readonly int Exif_GPSInfo_GPSAreaInformation = 28; // 0x001c
        public static readonly int Exif_GPSInfo_GPSDateStamp = 29; // 0x001d
        public static readonly int Exif_GPSInfo_GPSDifferential = 30; // 0x001e

        static TagDescriptor[] descriptors =
        {
            new TagDescriptor(Exif_Image_ProcessingSoftware, "Exif_Image_ProcessingSoftware", AsciiToString),
            new TagDescriptor(Exif_Image_NewSubfileType, "Exif_Image_NewSubfileType", LongToString),
            new TagDescriptor(Exif_Image_SubfileType, "Exif_Image_SubfileType", ShortToString),
            new TagDescriptor(Exif_Image_ImageWidth, "Exif_Image_ImageWidth", LongToString),
            new TagDescriptor(Exif_Image_ImageLength, "Exif_Image_ImageLength", LongToString),
            new TagDescriptor(Exif_Image_BitsPerSample, "Exif_Image_BitsPerSample", ShortToString),
            new TagDescriptor(Exif_Image_Compression, "Exif_Image_Compression", ShortToString),
            new TagDescriptor(Exif_Image_PhotometricInterpretation, "Exif_Image_PhotometricInterpretation", ShortToString),
            new TagDescriptor(Exif_Image_Thresholding, "Exif_Image_Thresholding", ShortToString),
            new TagDescriptor(Exif_Image_CellWidth, "Exif_Image_CellWidth", ShortToString),
            new TagDescriptor(Exif_Image_CellLength, "Exif_Image_CellLength", ShortToString),
            new TagDescriptor(Exif_Image_FillOrder, "Exif_Image_FillOrder", ShortToString),
            new TagDescriptor(Exif_Image_DocumentName, "Exif_Image_DocumentName", AsciiToString),
            new TagDescriptor(Exif_Image_ImageDescription, "Exif_Image_ImageDescription", AsciiToString),
            new TagDescriptor(Exif_Image_Make, "Exif_Image_Make", AsciiToString),
            new TagDescriptor(Exif_Image_Model, "Exif_Image_Model", AsciiToString),
            new TagDescriptor(Exif_Image_StripOffsets, "Exif_Image_StripOffsets", LongToString),
            new TagDescriptor(Exif_Image_Orientation, "Exif_Image_Orientation", ShortToString),
            new TagDescriptor(Exif_Image_SamplesPerPixel, "Exif_Image_SamplesPerPixel", ShortToString),
            new TagDescriptor(Exif_Image_RowsPerStrip, "Exif_Image_RowsPerStrip", LongToString),
            new TagDescriptor(Exif_Image_StripByteCounts, "Exif_Image_StripByteCounts", LongToString),
            new TagDescriptor(Exif_Image_XResolution, "Exif_Image_XResolution", RationalToString),
            new TagDescriptor(Exif_Image_YResolution, "Exif_Image_YResolution", RationalToString),
            new TagDescriptor(Exif_Image_PlanarConfiguration, "Exif_Image_PlanarConfiguration", ShortToString),
            new TagDescriptor(Exif_Image_GrayResponseUnit, "Exif_Image_GrayResponseUnit", ShortToString),
            new TagDescriptor(Exif_Image_GrayResponseCurve, "Exif_Image_GrayResponseCurve", ShortToString),
            new TagDescriptor(Exif_Image_T4Options, "Exif_Image_T4Options", LongToString),
            new TagDescriptor(Exif_Image_T6Options, "Exif_Image_T6Options", LongToString),
            new TagDescriptor(Exif_Image_ResolutionUnit, "Exif_Image_ResolutionUnit", ShortToString),
            new TagDescriptor(Exif_Image_PageNumber, "Exif_Image_PageNumber", ShortToString),
            new TagDescriptor(Exif_Image_TransferFunction, "Exif_Image_TransferFunction", ShortToString),
            new TagDescriptor(Exif_Image_Software, "Exif_Image_Software", AsciiToString),
            new TagDescriptor(Exif_Image_DateTime, "Exif_Image_DateTime", AsciiToString),
            new TagDescriptor(Exif_Image_Artist, "Exif_Image_Artist", AsciiToString),
            new TagDescriptor(Exif_Image_HostComputer, "Exif_Image_HostComputer", AsciiToString),
            new TagDescriptor(Exif_Image_Predictor, "Exif_Image_Predictor", ShortToString),
            new TagDescriptor(Exif_Image_WhitePoint, "Exif_Image_WhitePoint", RationalToString),
            new TagDescriptor(Exif_Image_PrimaryChromaticities, "Exif_Image_PrimaryChromaticities", RationalToString),
            new TagDescriptor(Exif_Image_ColorMap, "Exif_Image_ColorMap", ShortToString),
            new TagDescriptor(Exif_Image_HalftoneHints, "Exif_Image_HalftoneHints", ShortToString),
            new TagDescriptor(Exif_Image_TileWidth, "Exif_Image_TileWidth", ShortToString),
            new TagDescriptor(Exif_Image_TileLength, "Exif_Image_TileLength", ShortToString),
            new TagDescriptor(Exif_Image_TileOffsets, "Exif_Image_TileOffsets", ShortToString),
            new TagDescriptor(Exif_Image_TileByteCounts, "Exif_Image_TileByteCounts", ShortToString),
            new TagDescriptor(Exif_Image_SubIFDs, "Exif_Image_SubIFDs", LongToString),
            new TagDescriptor(Exif_Image_InkSet, "Exif_Image_InkSet", ShortToString),
            new TagDescriptor(Exif_Image_InkNames, "Exif_Image_InkNames", AsciiToString),
            new TagDescriptor(Exif_Image_NumberOfInks, "Exif_Image_NumberOfInks", ShortToString),
            new TagDescriptor(Exif_Image_DotRange, "Exif_Image_DotRange", ByteToString),
            new TagDescriptor(Exif_Image_TargetPrinter, "Exif_Image_TargetPrinter", AsciiToString),
            new TagDescriptor(Exif_Image_ExtraSamples, "Exif_Image_ExtraSamples", ShortToString),
            new TagDescriptor(Exif_Image_SampleFormat, "Exif_Image_SampleFormat", ShortToString),
            new TagDescriptor(Exif_Image_SMinSampleValue, "Exif_Image_SMinSampleValue", ShortToString),
            new TagDescriptor(Exif_Image_SMaxSampleValue, "Exif_Image_SMaxSampleValue", ShortToString),
            new TagDescriptor(Exif_Image_TransferRange, "Exif_Image_TransferRange", ShortToString),
            new TagDescriptor(Exif_Image_ClipPath, "Exif_Image_ClipPath", ByteToString),
            new TagDescriptor(Exif_Image_XClipPathUnits, "Exif_Image_XClipPathUnits", SShortToString),
            new TagDescriptor(Exif_Image_YClipPathUnits, "Exif_Image_YClipPathUnits", SShortToString),
            new TagDescriptor(Exif_Image_Indexed, "Exif_Image_Indexed", ShortToString),
            new TagDescriptor(Exif_Image_JPEGTables, "Exif_Image_JPEGTables", UndefinedToString),
            new TagDescriptor(Exif_Image_OPIProxy, "Exif_Image_OPIProxy", ShortToString),
            new TagDescriptor(Exif_Image_JPEGProc, "Exif_Image_JPEGProc", LongToString),
            new TagDescriptor(Exif_Image_JPEGInterchangeFormat, "Exif_Image_JPEGInterchangeFormat", LongToString),
            new TagDescriptor(Exif_Image_JPEGInterchangeFormatLength, "Exif_Image_JPEGInterchangeFormatLength", LongToString),
            new TagDescriptor(Exif_Image_JPEGRestartInterval, "Exif_Image_JPEGRestartInterval", ShortToString),
            new TagDescriptor(Exif_Image_JPEGLosslessPredictors, "Exif_Image_JPEGLosslessPredictors", ShortToString),
            new TagDescriptor(Exif_Image_JPEGPointTransforms, "Exif_Image_JPEGPointTransforms", ShortToString),
            new TagDescriptor(Exif_Image_JPEGQTables, "Exif_Image_JPEGQTables", LongToString),
            new TagDescriptor(Exif_Image_JPEGDCTables, "Exif_Image_JPEGDCTables", LongToString),
            new TagDescriptor(Exif_Image_JPEGACTables, "Exif_Image_JPEGACTables", LongToString),
            new TagDescriptor(Exif_Image_YCbCrCoefficients, "Exif_Image_YCbCrCoefficients", RationalToString),
            new TagDescriptor(Exif_Image_YCbCrSubSampling, "Exif_Image_YCbCrSubSampling", ShortToString),
            new TagDescriptor(Exif_Image_YCbCrPositioning, "Exif_Image_YCbCrPositioning", ShortToString),
            new TagDescriptor(Exif_Image_ReferenceBlackWhite, "Exif_Image_ReferenceBlackWhite", RationalToString),
            new TagDescriptor(Exif_Image_XMLPacket, "Exif_Image_XMLPacket", ByteToString),
            new TagDescriptor(Exif_Image_Rating, "Exif_Image_Rating", ShortToString),
            new TagDescriptor(Exif_Image_RatingPercent, "Exif_Image_RatingPercent", ShortToString),
            new TagDescriptor(Exif_Image_ImageID, "Exif_Image_ImageID", AsciiToString),
            new TagDescriptor(Exif_Image_CFARepeatPatternDim, "Exif_Image_CFARepeatPatternDim", ShortToString),
            new TagDescriptor(Exif_Image_CFAPattern, "Exif_Image_CFAPattern", ByteToString),
            new TagDescriptor(Exif_Image_BatteryLevel, "Exif_Image_BatteryLevel", RationalToString),
            new TagDescriptor(Exif_Image_Copyright, "Exif_Image_Copyright", AsciiToString),
            new TagDescriptor(Exif_Image_ExposureTime, "Exif_Image_ExposureTime", RationalToString),
            new TagDescriptor(Exif_Image_FNumber, "Exif_Image_FNumber", RationalToString),
            new TagDescriptor(Exif_Image_IPTCNAA, "Exif_Image_IPTCNAA", LongToString),
            new TagDescriptor(Exif_Image_ImageResources, "Exif_Image_ImageResources", ByteToString),
            new TagDescriptor(Exif_Image_ExifTag, "Exif_Image_ExifTag", LongToString),
            new TagDescriptor(Exif_Image_InterColorProfile, "Exif_Image_InterColorProfile", UndefinedToString),
            new TagDescriptor(Exif_Image_ExposureProgram, "Exif_Image_ExposureProgram", ShortToString),
            new TagDescriptor(Exif_Image_SpectralSensitivity, "Exif_Image_SpectralSensitivity", AsciiToString),
            new TagDescriptor(Exif_Image_GPSTag, "Exif_Image_GPSTag", LongToString),
            new TagDescriptor(Exif_Image_ISOSpeedRatings, "Exif_Image_ISOSpeedRatings", ShortToString),
            new TagDescriptor(Exif_Image_OECF, "Exif_Image_OECF", UndefinedToString),
            new TagDescriptor(Exif_Image_Interlace, "Exif_Image_Interlace", ShortToString),
            new TagDescriptor(Exif_Image_TimeZoneOffset, "Exif_Image_TimeZoneOffset", SShortToString),
            new TagDescriptor(Exif_Image_SelfTimerMode, "Exif_Image_SelfTimerMode", ShortToString),
            new TagDescriptor(Exif_Image_DateTimeOriginal, "Exif_Image_DateTimeOriginal", AsciiToString),
            new TagDescriptor(Exif_Image_CompressedBitsPerPixel, "Exif_Image_CompressedBitsPerPixel", RationalToString),
            new TagDescriptor(Exif_Image_ShutterSpeedValue, "Exif_Image_ShutterSpeedValue", SRationalToString),
            new TagDescriptor(Exif_Image_ApertureValue, "Exif_Image_ApertureValue", RationalToString),
            new TagDescriptor(Exif_Image_BrightnessValue, "Exif_Image_BrightnessValue", SRationalToString),
            new TagDescriptor(Exif_Image_ExposureBiasValue, "Exif_Image_ExposureBiasValue", SRationalToString),
            new TagDescriptor(Exif_Image_MaxApertureValue, "Exif_Image_MaxApertureValue", RationalToString),
            new TagDescriptor(Exif_Image_SubjectDistance, "Exif_Image_SubjectDistance", SRationalToString),
            new TagDescriptor(Exif_Image_MeteringMode, "Exif_Image_MeteringMode", ShortToString),
            new TagDescriptor(Exif_Image_LightSource, "Exif_Image_LightSource", ShortToString),
            new TagDescriptor(Exif_Image_Flash, "Exif_Image_Flash", ShortToString),
            new TagDescriptor(Exif_Image_FocalLength, "Exif_Image_FocalLength", RationalToString),
            new TagDescriptor(Exif_Image_FlashEnergy, "Exif_Image_FlashEnergy", RationalToString),
            new TagDescriptor(Exif_Image_SpatialFrequencyResponse, "Exif_Image_SpatialFrequencyResponse", UndefinedToString),
            new TagDescriptor(Exif_Image_Noise, "Exif_Image_Noise", UndefinedToString),
            new TagDescriptor(Exif_Image_FocalPlaneXResolution, "Exif_Image_FocalPlaneXResolution", RationalToString),
            new TagDescriptor(Exif_Image_FocalPlaneYResolution, "Exif_Image_FocalPlaneYResolution", RationalToString),
            new TagDescriptor(Exif_Image_FocalPlaneResolutionUnit, "Exif_Image_FocalPlaneResolutionUnit", ShortToString),
            new TagDescriptor(Exif_Image_ImageNumber, "Exif_Image_ImageNumber", LongToString),
            new TagDescriptor(Exif_Image_SecurityClassification, "Exif_Image_SecurityClassification", AsciiToString),
            new TagDescriptor(Exif_Image_ImageHistory, "Exif_Image_ImageHistory", AsciiToString),
            new TagDescriptor(Exif_Image_SubjectLocation, "Exif_Image_SubjectLocation", ShortToString),
            new TagDescriptor(Exif_Image_ExposureIndex, "Exif_Image_ExposureIndex", RationalToString),
            new TagDescriptor(Exif_Image_TIFFEPStandardID, "Exif_Image_TIFFEPStandardID", ByteToString),
            new TagDescriptor(Exif_Image_SensingMethod, "Exif_Image_SensingMethod", ShortToString),
            new TagDescriptor(Exif_Image_XPTitle, "Exif_Image_XPTitle", ByteToString),
            new TagDescriptor(Exif_Image_XPComment, "Exif_Image_XPComment", ByteToString),
            new TagDescriptor(Exif_Image_XPAuthor, "Exif_Image_XPAuthor", ByteToString),
            new TagDescriptor(Exif_Image_XPKeywords, "Exif_Image_XPKeywords", ByteToString),
            new TagDescriptor(Exif_Image_XPSubject, "Exif_Image_XPSubject", ByteToString),
            new TagDescriptor(Exif_Image_PrintImageMatching, "Exif_Image_PrintImageMatching", UndefinedToString),
            new TagDescriptor(Exif_Image_DNGVersion, "Exif_Image_DNGVersion", ByteToString),
            new TagDescriptor(Exif_Image_DNGBackwardVersion, "Exif_Image_DNGBackwardVersion", ByteToString),
            new TagDescriptor(Exif_Image_UniqueCameraModel, "Exif_Image_UniqueCameraModel", AsciiToString),
            new TagDescriptor(Exif_Image_LocalizedCameraModel, "Exif_Image_LocalizedCameraModel", ByteToString),
            new TagDescriptor(Exif_Image_CFAPlaneColor, "Exif_Image_CFAPlaneColor", ByteToString),
            new TagDescriptor(Exif_Image_CFALayout, "Exif_Image_CFALayout", ShortToString),
            new TagDescriptor(Exif_Image_LinearizationTable, "Exif_Image_LinearizationTable", ShortToString),
            new TagDescriptor(Exif_Image_BlackLevelRepeatDim, "Exif_Image_BlackLevelRepeatDim", ShortToString),
            new TagDescriptor(Exif_Image_BlackLevel, "Exif_Image_BlackLevel", RationalToString),
            new TagDescriptor(Exif_Image_BlackLevelDeltaH, "Exif_Image_BlackLevelDeltaH", SRationalToString),
            new TagDescriptor(Exif_Image_BlackLevelDeltaV, "Exif_Image_BlackLevelDeltaV", SRationalToString),
            new TagDescriptor(Exif_Image_WhiteLevel, "Exif_Image_WhiteLevel", ShortToString),
            new TagDescriptor(Exif_Image_DefaultScale, "Exif_Image_DefaultScale", RationalToString),
            new TagDescriptor(Exif_Image_DefaultCropOrigin, "Exif_Image_DefaultCropOrigin", ShortToString),
            new TagDescriptor(Exif_Image_DefaultCropSize, "Exif_Image_DefaultCropSize", ShortToString),
            new TagDescriptor(Exif_Image_ColorMatrix1, "Exif_Image_ColorMatrix1", SRationalToString),
            new TagDescriptor(Exif_Image_ColorMatrix2, "Exif_Image_ColorMatrix2", SRationalToString),
            new TagDescriptor(Exif_Image_CameraCalibration1, "Exif_Image_CameraCalibration1", SRationalToString),
            new TagDescriptor(Exif_Image_CameraCalibration2, "Exif_Image_CameraCalibration2", SRationalToString),
            new TagDescriptor(Exif_Image_ReductionMatrix1, "Exif_Image_ReductionMatrix1", SRationalToString),
            new TagDescriptor(Exif_Image_ReductionMatrix2, "Exif_Image_ReductionMatrix2", SRationalToString),
            new TagDescriptor(Exif_Image_AnalogBalance, "Exif_Image_AnalogBalance", RationalToString),
            new TagDescriptor(Exif_Image_AsShotNeutral, "Exif_Image_AsShotNeutral", ShortToString),
            new TagDescriptor(Exif_Image_AsShotWhiteXY, "Exif_Image_AsShotWhiteXY", RationalToString),
            new TagDescriptor(Exif_Image_BaselineExposure, "Exif_Image_BaselineExposure", SRationalToString),
            new TagDescriptor(Exif_Image_BaselineNoise, "Exif_Image_BaselineNoise", RationalToString),
            new TagDescriptor(Exif_Image_BaselineSharpness, "Exif_Image_BaselineSharpness", RationalToString),
            new TagDescriptor(Exif_Image_BayerGreenSplit, "Exif_Image_BayerGreenSplit", LongToString),
            new TagDescriptor(Exif_Image_LinearResponseLimit, "Exif_Image_LinearResponseLimit", RationalToString),
            new TagDescriptor(Exif_Image_CameraSerialNumber, "Exif_Image_CameraSerialNumber", AsciiToString),
            new TagDescriptor(Exif_Image_LensInfo, "Exif_Image_LensInfo", RationalToString),
            new TagDescriptor(Exif_Image_ChromaBlurRadius, "Exif_Image_ChromaBlurRadius", RationalToString),
            new TagDescriptor(Exif_Image_AntiAliasStrength, "Exif_Image_AntiAliasStrength", RationalToString),
            new TagDescriptor(Exif_Image_ShadowScale, "Exif_Image_ShadowScale", SRationalToString),
            new TagDescriptor(Exif_Image_DNGPrivateData, "Exif_Image_DNGPrivateData", ByteToString),
            new TagDescriptor(Exif_Image_MakerNoteSafety, "Exif_Image_MakerNoteSafety", ShortToString),
            new TagDescriptor(Exif_Image_CalibrationIlluminant1, "Exif_Image_CalibrationIlluminant1", ShortToString),
            new TagDescriptor(Exif_Image_CalibrationIlluminant2, "Exif_Image_CalibrationIlluminant2", ShortToString),
            new TagDescriptor(Exif_Image_BestQualityScale, "Exif_Image_BestQualityScale", RationalToString),
            new TagDescriptor(Exif_Image_RawDataUniqueID, "Exif_Image_RawDataUniqueID", ByteToString),
            new TagDescriptor(Exif_Image_OriginalRawFileName, "Exif_Image_OriginalRawFileName", ByteToString),
            new TagDescriptor(Exif_Image_OriginalRawFileData, "Exif_Image_OriginalRawFileData", UndefinedToString),
            new TagDescriptor(Exif_Image_ActiveArea, "Exif_Image_ActiveArea", ShortToString),
            new TagDescriptor(Exif_Image_MaskedAreas, "Exif_Image_MaskedAreas", ShortToString),
            new TagDescriptor(Exif_Image_AsShotICCProfile, "Exif_Image_AsShotICCProfile", UndefinedToString),
            new TagDescriptor(Exif_Image_AsShotPreProfileMatrix, "Exif_Image_AsShotPreProfileMatrix", SRationalToString),
            new TagDescriptor(Exif_Image_CurrentICCProfile, "Exif_Image_CurrentICCProfile", UndefinedToString),
            new TagDescriptor(Exif_Image_CurrentPreProfileMatrix, "Exif_Image_CurrentPreProfileMatrix", SRationalToString),
            new TagDescriptor(Exif_Image_ColorimetricReference, "Exif_Image_ColorimetricReference", ShortToString),
            new TagDescriptor(Exif_Image_CameraCalibrationSignature, "Exif_Image_CameraCalibrationSignature", ByteToString),
            new TagDescriptor(Exif_Image_ProfileCalibrationSignature, "Exif_Image_ProfileCalibrationSignature", ByteToString),
            new TagDescriptor(Exif_Image_AsShotProfileName, "Exif_Image_AsShotProfileName", ByteToString),
            new TagDescriptor(Exif_Image_NoiseReductionApplied, "Exif_Image_NoiseReductionApplied", RationalToString),
            new TagDescriptor(Exif_Image_ProfileName, "Exif_Image_ProfileName", ByteToString),
            new TagDescriptor(Exif_Image_ProfileHueSatMapDims, "Exif_Image_ProfileHueSatMapDims", LongToString),
            new TagDescriptor(Exif_Image_ProfileHueSatMapData1, "Exif_Image_ProfileHueSatMapData1", FloatToString),
            new TagDescriptor(Exif_Image_ProfileHueSatMapData2, "Exif_Image_ProfileHueSatMapData2", FloatToString),
            new TagDescriptor(Exif_Image_ProfileToneCurve, "Exif_Image_ProfileToneCurve", FloatToString),
            new TagDescriptor(Exif_Image_ProfileEmbedPolicy, "Exif_Image_ProfileEmbedPolicy", LongToString),
            new TagDescriptor(Exif_Image_ProfileCopyright, "Exif_Image_ProfileCopyright", ByteToString),
            new TagDescriptor(Exif_Image_ForwardMatrix1, "Exif_Image_ForwardMatrix1", SRationalToString),
            new TagDescriptor(Exif_Image_ForwardMatrix2, "Exif_Image_ForwardMatrix2", SRationalToString),
            new TagDescriptor(Exif_Image_PreviewApplicationName, "Exif_Image_PreviewApplicationName", ByteToString),
            new TagDescriptor(Exif_Image_PreviewApplicationVersion, "Exif_Image_PreviewApplicationVersion", ByteToString),
            new TagDescriptor(Exif_Image_PreviewSettingsName, "Exif_Image_PreviewSettingsName", ByteToString),
            new TagDescriptor(Exif_Image_PreviewSettingsDigest, "Exif_Image_PreviewSettingsDigest", ByteToString),
            new TagDescriptor(Exif_Image_PreviewColorSpace, "Exif_Image_PreviewColorSpace", LongToString),
            new TagDescriptor(Exif_Image_PreviewDateTime, "Exif_Image_PreviewDateTime", AsciiToString),
            new TagDescriptor(Exif_Image_RawImageDigest, "Exif_Image_RawImageDigest", UndefinedToString),
            new TagDescriptor(Exif_Image_OriginalRawFileDigest, "Exif_Image_OriginalRawFileDigest", UndefinedToString),
            new TagDescriptor(Exif_Image_SubTileBlockSize, "Exif_Image_SubTileBlockSize", LongToString),
            new TagDescriptor(Exif_Image_RowInterleaveFactor, "Exif_Image_RowInterleaveFactor", LongToString),
            new TagDescriptor(Exif_Image_ProfileLookTableDims, "Exif_Image_ProfileLookTableDims", LongToString),
            new TagDescriptor(Exif_Image_ProfileLookTableData, "Exif_Image_ProfileLookTableData", FloatToString),
            new TagDescriptor(Exif_Image_OpcodeList1, "Exif_Image_OpcodeList1", UndefinedToString),
            new TagDescriptor(Exif_Image_OpcodeList2, "Exif_Image_OpcodeList2", UndefinedToString),
            new TagDescriptor(Exif_Image_OpcodeList3, "Exif_Image_OpcodeList3", UndefinedToString),
            new TagDescriptor(Exif_Image_NoiseProfile, "Exif_Image_NoiseProfile", DoubleToString),
            new TagDescriptor(Exif_Photo_ExposureTime, "Exif_Photo_ExposureTime", RationalToString),
            new TagDescriptor(Exif_Photo_FNumber, "Exif_Photo_FNumber", RationalToString),
            new TagDescriptor(Exif_Photo_ExposureProgram, "Exif_Photo_ExposureProgram", ShortToString),
            new TagDescriptor(Exif_Photo_SpectralSensitivity, "Exif_Photo_SpectralSensitivity", AsciiToString),
            new TagDescriptor(Exif_Photo_ISOSpeedRatings, "Exif_Photo_ISOSpeedRatings", ShortToString),
            new TagDescriptor(Exif_Photo_OECF, "Exif_Photo_OECF", UndefinedToString),
            new TagDescriptor(Exif_Photo_SensitivityType, "Exif_Photo_SensitivityType", ShortToString),
            new TagDescriptor(Exif_Photo_StandardOutputSensitivity, "Exif_Photo_StandardOutputSensitivity", LongToString),
            new TagDescriptor(Exif_Photo_RecommendedExposureIndex, "Exif_Photo_RecommendedExposureIndex", LongToString),
            new TagDescriptor(Exif_Photo_ISOSpeed, "Exif_Photo_ISOSpeed", LongToString),
            new TagDescriptor(Exif_Photo_ISOSpeedLatitudeyyy, "Exif_Photo_ISOSpeedLatitudeyyy", LongToString),
            new TagDescriptor(Exif_Photo_ISOSpeedLatitudezzz, "Exif_Photo_ISOSpeedLatitudezzz", LongToString),
            new TagDescriptor(Exif_Photo_ExifVersion, "Exif_Photo_ExifVersion", UndefinedToString),
            new TagDescriptor(Exif_Photo_DateTimeOriginal, "Exif_Photo_DateTimeOriginal", AsciiToString),
            new TagDescriptor(Exif_Photo_DateTimeDigitized, "Exif_Photo_DateTimeDigitized", AsciiToString),
            new TagDescriptor(Exif_Photo_ComponentsConfiguration, "Exif_Photo_ComponentsConfiguration", UndefinedToString),
            new TagDescriptor(Exif_Photo_CompressedBitsPerPixel, "Exif_Photo_CompressedBitsPerPixel", RationalToString),
            new TagDescriptor(Exif_Photo_ShutterSpeedValue, "Exif_Photo_ShutterSpeedValue", SRationalToString),
            new TagDescriptor(Exif_Photo_ApertureValue, "Exif_Photo_ApertureValue", RationalToString),
            new TagDescriptor(Exif_Photo_BrightnessValue, "Exif_Photo_BrightnessValue", SRationalToString),
            new TagDescriptor(Exif_Photo_ExposureBiasValue, "Exif_Photo_ExposureBiasValue", SRationalToString),
            new TagDescriptor(Exif_Photo_MaxApertureValue, "Exif_Photo_MaxApertureValue", RationalToString),
            new TagDescriptor(Exif_Photo_SubjectDistance, "Exif_Photo_SubjectDistance", RationalToString),
            new TagDescriptor(Exif_Photo_MeteringMode, "Exif_Photo_MeteringMode", ShortToString),
            new TagDescriptor(Exif_Photo_LightSource, "Exif_Photo_LightSource", ShortToString),
            new TagDescriptor(Exif_Photo_Flash, "Exif_Photo_Flash", ShortToString),
            new TagDescriptor(Exif_Photo_FocalLength, "Exif_Photo_FocalLength", RationalToString),
            new TagDescriptor(Exif_Photo_SubjectArea, "Exif_Photo_SubjectArea", ShortToString),
            new TagDescriptor(Exif_Photo_MakerNote, "Exif_Photo_MakerNote", UndefinedToString),
            new TagDescriptor(Exif_Photo_UserComment, "Exif_Photo_UserComment", CommentToString),
            new TagDescriptor(Exif_Photo_SubSecTime, "Exif_Photo_SubSecTime", AsciiToString),
            new TagDescriptor(Exif_Photo_SubSecTimeOriginal, "Exif_Photo_SubSecTimeOriginal", AsciiToString),
            new TagDescriptor(Exif_Photo_SubSecTimeDigitized, "Exif_Photo_SubSecTimeDigitized", AsciiToString),
            new TagDescriptor(Exif_Photo_FlashpixVersion, "Exif_Photo_FlashpixVersion", UndefinedToString),
            new TagDescriptor(Exif_Photo_ColorSpace, "Exif_Photo_ColorSpace", ShortToString),
            new TagDescriptor(Exif_Photo_PixelXDimension, "Exif_Photo_PixelXDimension", LongToString),
            new TagDescriptor(Exif_Photo_PixelYDimension, "Exif_Photo_PixelYDimension", LongToString),
            new TagDescriptor(Exif_Photo_RelatedSoundFile, "Exif_Photo_RelatedSoundFile", AsciiToString),
            new TagDescriptor(Exif_Photo_InteroperabilityTag, "Exif_Photo_InteroperabilityTag", LongToString),
            new TagDescriptor(Exif_Photo_FlashEnergy, "Exif_Photo_FlashEnergy", RationalToString),
            new TagDescriptor(Exif_Photo_SpatialFrequencyResponse, "Exif_Photo_SpatialFrequencyResponse", UndefinedToString),
            new TagDescriptor(Exif_Photo_FocalPlaneXResolution, "Exif_Photo_FocalPlaneXResolution", RationalToString),
            new TagDescriptor(Exif_Photo_FocalPlaneYResolution, "Exif_Photo_FocalPlaneYResolution", RationalToString),
            new TagDescriptor(Exif_Photo_FocalPlaneResolutionUnit, "Exif_Photo_FocalPlaneResolutionUnit", ShortToString),
            new TagDescriptor(Exif_Photo_SubjectLocation, "Exif_Photo_SubjectLocation", ShortToString),
            new TagDescriptor(Exif_Photo_ExposureIndex, "Exif_Photo_ExposureIndex", RationalToString),
            new TagDescriptor(Exif_Photo_SensingMethod, "Exif_Photo_SensingMethod", ShortToString),
            new TagDescriptor(Exif_Photo_FileSource, "Exif_Photo_FileSource", UndefinedToString),
            new TagDescriptor(Exif_Photo_SceneType, "Exif_Photo_SceneType", UndefinedToString),
            new TagDescriptor(Exif_Photo_CFAPattern, "Exif_Photo_CFAPattern", UndefinedToString),
            new TagDescriptor(Exif_Photo_CustomRendered, "Exif_Photo_CustomRendered", ShortToString),
            new TagDescriptor(Exif_Photo_ExposureMode, "Exif_Photo_ExposureMode", ShortToString),
            new TagDescriptor(Exif_Photo_WhiteBalance, "Exif_Photo_WhiteBalance", ShortToString),
            new TagDescriptor(Exif_Photo_DigitalZoomRatio, "Exif_Photo_DigitalZoomRatio", RationalToString),
            new TagDescriptor(Exif_Photo_FocalLengthIn35mmFilm, "Exif_Photo_FocalLengthIn35mmFilm", ShortToString),
            new TagDescriptor(Exif_Photo_SceneCaptureType, "Exif_Photo_SceneCaptureType", ShortToString),
            new TagDescriptor(Exif_Photo_GainControl, "Exif_Photo_GainControl", ShortToString),
            new TagDescriptor(Exif_Photo_Contrast, "Exif_Photo_Contrast", ShortToString),
            new TagDescriptor(Exif_Photo_Saturation, "Exif_Photo_Saturation", ShortToString),
            new TagDescriptor(Exif_Photo_Sharpness, "Exif_Photo_Sharpness", ShortToString),
            new TagDescriptor(Exif_Photo_DeviceSettingDescription, "Exif_Photo_DeviceSettingDescription", UndefinedToString),
            new TagDescriptor(Exif_Photo_SubjectDistanceRange, "Exif_Photo_SubjectDistanceRange", ShortToString),
            new TagDescriptor(Exif_Photo_ImageUniqueID, "Exif_Photo_ImageUniqueID", AsciiToString),
            new TagDescriptor(Exif_Photo_CameraOwnerName, "Exif_Photo_CameraOwnerName", AsciiToString),
            new TagDescriptor(Exif_Photo_BodySerialNumber, "Exif_Photo_BodySerialNumber", AsciiToString),
            new TagDescriptor(Exif_Photo_LensSpecification, "Exif_Photo_LensSpecification", RationalToString),
            new TagDescriptor(Exif_Photo_LensMake, "Exif_Photo_LensMake", AsciiToString),
            new TagDescriptor(Exif_Photo_LensModel, "Exif_Photo_LensModel", AsciiToString),
            new TagDescriptor(Exif_Photo_LensSerialNumber, "Exif_Photo_LensSerialNumber", AsciiToString),
           
            new TagDescriptor(Exif_GPSInfo_GPSVersionID, "Exif_GPSInfo_GPSVersionID", ByteToString),
            new TagDescriptor(Exif_GPSInfo_GPSLatitudeRef, "Exif_GPSInfo_GPSLatitudeRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSLatitude, "Exif_GPSInfo_GPSLatitude", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSLongitudeRef, "Exif_GPSInfo_GPSLongitudeRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSLongitude, "Exif_GPSInfo_GPSLongitude", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSAltitudeRef, "Exif_GPSInfo_GPSAltitudeRef", ByteToString),
            new TagDescriptor(Exif_GPSInfo_GPSAltitude, "Exif_GPSInfo_GPSAltitude", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSTimeStamp, "Exif_GPSInfo_GPSTimeStamp", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSSatellites, "Exif_GPSInfo_GPSSatellites", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSStatus, "Exif_GPSInfo_GPSStatus", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSMeasureMode, "Exif_GPSInfo_GPSMeasureMode", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDOP, "Exif_GPSInfo_GPSDOP", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSSpeedRef, "Exif_GPSInfo_GPSSpeedRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSSpeed, "Exif_GPSInfo_GPSSpeed", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSTrackRef, "Exif_GPSInfo_GPSTrackRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSTrack, "Exif_GPSInfo_GPSTrack", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSImgDirectionRef, "Exif_GPSInfo_GPSImgDirectionRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSImgDirection, "Exif_GPSInfo_GPSImgDirection", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSMapDatum, "Exif_GPSInfo_GPSMapDatum", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestLatitudeRef, "Exif_GPSInfo_GPSDestLatitudeRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestLatitude, "Exif_GPSInfo_GPSDestLatitude", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestLongitudeRef, "Exif_GPSInfo_GPSDestLongitudeRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestLongitude, "Exif_GPSInfo_GPSDestLongitude", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestBearingRef, "Exif_GPSInfo_GPSDestBearingRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestBearing, "Exif_GPSInfo_GPSDestBearing", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestDistanceRef, "Exif_GPSInfo_GPSDestDistanceRef", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDestDistance, "Exif_GPSInfo_GPSDestDistance", RationalToString),
            new TagDescriptor(Exif_GPSInfo_GPSProcessingMethod, "Exif_GPSInfo_GPSProcessingMethod", UndefinedToString),
            new TagDescriptor(Exif_GPSInfo_GPSAreaInformation, "Exif_GPSInfo_GPSAreaInformation", UndefinedToString),
            new TagDescriptor(Exif_GPSInfo_GPSDateStamp, "Exif_GPSInfo_GPSDateStamp", AsciiToString),
            new TagDescriptor(Exif_GPSInfo_GPSDifferential, "Exif_GPSInfo_GPSDifferential", ShortToString)
        };

        // extractors
        private static int AppendRational(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadUInt32());
            sb.Append('/');
            sb.Append(reader.ReadUInt32());
            return sizeof(UInt32) * 2;
        }

        private static int AppendSRational(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadInt32());
            sb.Append('/');
            sb.Append(reader.ReadInt32());
            return sizeof(Int32) * 2;
        }

        private static int AppendDouble(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadDouble());
            return sizeof(Double);
        }

        private static int AppendShort(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadUInt16());
            return sizeof(UInt16);
        }

        private static int AppendSShort(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadInt16());
            return sizeof(Int16);
        }

        private static int AppendLong(StringBuilder sb, BinaryReader reader)
        {
            sb.Append(reader.ReadUInt32());
            return sizeof(UInt32);
        }
        private static String RationalToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder(); 
            len -= AppendRational(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendRational(sb, reader);
            }
            return sb.ToString();
        }

        private static String SRationalToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            len -= AppendSRational(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendSRational(sb, reader);
            }
            return sb.ToString();
        }



        private static String CommentToString(short type, int len, byte[] value)
        {
            return ByteToString(type, 4, value);
        }

        private static String DoubleToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            len -= AppendDouble(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendDouble(sb, reader);
            }
            return sb.ToString();
        }

        private static String AsciiToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            StreamReader reader = new StreamReader(ms);
            StringBuilder sb = new StringBuilder();
            int c;
            while ((c = reader.Read()) != 0)
                sb.Append((char)c);
            return sb.ToString();
        }

        private static String FloatToString(short type, int len, byte[] value)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("Float value");
            return sb.ToString();
        }

        private static String UndefinedToString(short type, int len, byte[] value)
        {
            return "Undefined";
        }

        private static String LongToString(short type, int len, byte[] value)
        {
            if (type == 3) return ShortToString(type, len, value);
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            len -= AppendLong(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendLong(sb, reader);
            }
            return sb.ToString();
        }

        private static String ShortToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            len -= AppendShort(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendShort(sb, reader);
            }
            return sb.ToString();
        }

        private static String SShortToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            len -= AppendSShort(sb, reader);
            while (len > 0)
            {
                sb.Append(',');
                len -= AppendSShort(sb, reader);
            }
            return sb.ToString();
        }

        private static String ByteToString(short type, int len, byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryReader reader = new BinaryReader(ms);
            StringBuilder sb = new StringBuilder();
            while (len-- > 0)
            {
                sb.Append(reader.ReadByte());
                sb.Append(' ');
            }
            return sb.ToString();
        }
        
        public static String TagToString(int tagId, short type, int len, Byte[] value)
        {
            if (value != null)
            {
                foreach (TagDescriptor td in descriptors)
                {
                    if (td.id == tagId)
                    {

                        return String.Format("{0}={1}",
                            td.name,
                            td.processor(type, len, value));
                    }
                }
            }
            return "";
        }
    }
}
