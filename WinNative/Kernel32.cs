using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.WinNative {
    public static class Kernel32 {

        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const UInt32 FILE_SHARE_READ = 0x00000001;
        public const UInt32 FILE_SHARE_WRITE = 0x00000002;
        public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const UInt32 OPEN_EXISTING = 3;
        public const UInt32 FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        public const Int32 INVALID_HANDLE_VALUE = -1;
        public const UInt32 FSCTL_QUERY_USN_JOURNAL = 0x000900f4;
        public const UInt32 FSCTL_ENUM_USN_DATA = 0x000900b3;
        public const UInt32 FSCTL_CREATE_USN_JOURNAL = 0x000900e7;

        public const uint IOCTL_DISK_GET_DRIVE_LAYOUT_EX = 0x00070050U;

        /// <summary>
        /// https://www.pinvoke.net/default.aspx/kernel32.CreateFile
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes,
            uint dwCreationDisposition, uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volumeName"></param>
        /// <param name="uniqueVolumeName"></param>
        /// <param name="uniqueNameBufferCapacity"></param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Auto, BestFitMapping = false)]
        public static extern bool GetVolumeNameForVolumeMountPoint(String volumeName, StringBuilder uniqueVolumeName, int uniqueNameBufferCapacity);


        [DllImport(@"kernel32.dll", SetLastError = true)]
        public static extern unsafe bool ReadFile(
            SafeFileHandle hFile,          // handle to file
            byte* pBuffer,                 // data buffer, should be fixed
            int NumberOfBytesToRead,       // number of bytes to read
            IntPtr pNumberOfBytesRead,     // number of bytes read, provide IntPtr.Zero here
            NativeOverlapped* lpOverlapped // should be fixed, if not IntPtr.Zero
        );

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct NativeOverlapped {
            private IntPtr InternalLow;
            private IntPtr InternalHigh;
            public long Offset;
            public IntPtr EventHandle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpFileInformation"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetFileInformationByHandle(IntPtr hFile,
            out BY_HANDLE_FILE_INFORMATION lpFileInformation);


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BY_HANDLE_FILE_INFORMATION {
            public uint FileAttributes;
            [Obsolete]
            public FILETIME CreationTime;
            [Obsolete]
            public FILETIME LastAccessTime;
            [Obsolete]
            public FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }


        public static int ERROR_INSUFFICIENT_BUFFER = 122;

        /// <summary>
        /// https://www.pinvoke.net/default.aspx/kernel32.querydosdevice
        /// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name. The function can also obtain a list of all existing MS-DOS device names.
        /// </summary>
        /// <param name="lpDeviceName"></param>
        /// <param name="lpTargetPath"></param>
        /// <param name="ucchMax"></param>
        /// <returns></returns>
/*        [DllImport("kernel32.dll")]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint ucchMax);*/

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint QueryDosDevice(string lpDeviceName, IntPtr lpTargetPath, uint ucchMax);


        /// <summary>
        /// Retrieves extended information about the physical disk's geometry: type, number of cylinders, tracks per
        /// cylinder, sectors per track, and bytes per sector.
        /// </summary>
        public const uint IOCTL_DISK_GET_DRIVE_GEOMETRY_EX = 0x000700A0U;



        /// <summary>
        /// Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);



        /// <summary>
        /// https://www.pinvoke.net/default.aspx/Enums/MEDIA_TYPE.html
        /// </summary>
        public enum MEDIA_TYPE : uint {

            /// <summary>
            /// Format is unknown 
            /// </summary>
            Unknown = 0x00,
            F5_1Pt2_512 = 0x01,
            F3_1Pt44_512 = 0x02,
            F3_2Pt88_512 = 0x03,
            F3_20Pt8_512 = 0x04,
            F3_720_512 = 0x05,
            F5_360_512 = 0x06,
            F5_320_512 = 0x07,
            F5_320_1024 = 0x08,
            F5_180_512 = 0x09,
            F5_160_512 = 0x0a,

            /// <summary>
            /// Removable media other than floppy
            /// </summary>
            RemovableMedia = 0x0b,

            /// <summary>
            /// Fixed hard disk media 
            /// </summary>
            FixedMedia = 0x0c,
            F3_120M_512 = 0x0d,
            F3_640_512 = 0x0e,
            F5_640_512 = 0x0f,
            F5_720_512 = 0x10,
            F3_1Pt2_512 = 0x11,
            F3_1Pt23_1024 = 0x12,
            F5_1Pt23_1024 = 0x13,
            F3_128Mb_512 = 0x14,
            F3_230Mb_512 = 0x15,
            F8_256_128 = 0x16,
            F3_200Mb_512 = 0x17,
            F3_240M_512 = 0x18,
            F3_32M_512 = 0x19
        }


        /// <summary>
        /// https://www.pinvoke.net/default.aspx/Structures/DISK_GEOMETRY.html
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DISK_GEOMETRY {
            /// <summary>
            /// The number of cylinders.
            /// </summary>
            [FieldOffset(0)]
            public Int64 Cylinders;

            /// <summary>
            /// The type of media. For a list of values, see MEDIA_TYPE.
            /// </summary>
            [FieldOffset(8)]
            public MEDIA_TYPE MediaType;

            /// <summary>
            /// The number of tracks per cylinder.
            /// </summary>
            [FieldOffset(12)]
            public uint TracksPerCylinder;

            /// <summary>
            /// The number of sectors per track.
            /// </summary>
            [FieldOffset(16)]
            public uint SectorsPerTrack;

            /// <summary>
            /// The number of bytes per sector.
            /// </summary>
            [FieldOffset(20)]
            public uint BytesPerSector;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DISK_GEOMETRY_EX {
            /// <summary>
            /// A DISK_GEOMETRY structure.
            /// </summary>
            [FieldOffset(0)]
            public DISK_GEOMETRY Geometry;

            /// <summary>
            /// The disk size, in bytes.
            /// </summary>
            [FieldOffset(24)]
            public Int64 DiskSize;

            /// <summary>
            /// Any additional data.
            /// </summary>
            [FieldOffset(32)]
            public Byte Data;
        }


        /// <summary>
        /// Represents the format of a partition.
        /// </summary>
        public enum PARTITION_STYLE : uint {
            /// <summary>
            /// Master boot record (MBR) format.
            /// </summary>
            PARTITION_STYLE_MBR = 0,

            /// <summary>
            /// GUID Partition Table (GPT) format.
            /// </summary>
            PARTITION_STYLE_GPT = 1,

            /// <summary>
            /// Partition not formatted in either of the recognized formats—MBR or GPT.
            /// </summary>
            PARTITION_STYLE_RAW = 2
        }

        /// <summary>
        /// Provides information about a drive's master boot record (MBR) partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_MBR {
            /// <summary>
            /// The signature of the drive.
            /// </summary>
            [FieldOffset(0)]
            public uint Signature;
        }

        /// <summary>
        /// Contains information about a drive's GUID partition table (GPT) partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_GPT {
            /// <summary>
            /// The GUID of the disk.
            /// </summary>
            [FieldOffset(0)]
            public Guid DiskId;

            /// <summary>
            /// The starting byte offset of the first usable block.
            /// </summary>
            [FieldOffset(16)]
            public Int64 StartingUsableOffset;

            /// <summary>
            /// The size of the usable blocks on the disk, in bytes.
            /// </summary>
            [FieldOffset(24)]
            public Int64 UsableLength;

            /// <summary>
            /// The maximum number of partitions that can be defined in the usable block.
            /// </summary>
            [FieldOffset(32)]
            public uint MaxPartitionCount;
        }

        /// <summary>
        /// Contains extended information about a drive's partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_EX {
            /// <summary>
            /// The style of the partitions on the drive enumerated by the PARTITION_STYLE enumeration.
            /// </summary>
            [FieldOffset(0)]
            public PARTITION_STYLE PartitionStyle;

            /// <summary>
            /// The number of partitions on a drive.
            /// 
            /// On disks with the MBR layout, this value is always a multiple of 4. Any partitions that are unused have
            /// a partition type of PARTITION_ENTRY_UNUSED.
            /// </summary>
            [FieldOffset(4)]
            public uint PartitionCount;

            /// <summary>
            /// A DRIVE_LAYOUT_INFORMATION_MBR structure containing information about the master boot record type 
            /// partitioning on the drive.
            /// </summary>
            [FieldOffset(8)]
            public DRIVE_LAYOUT_INFORMATION_MBR Mbr;

            /// <summary>
            /// A DRIVE_LAYOUT_INFORMATION_GPT structure containing information about the GUID disk partition type 
            /// partitioning on the drive.
            /// </summary>
            [FieldOffset(8)]
            public DRIVE_LAYOUT_INFORMATION_GPT Gpt;

            /// <summary>
            /// A variable-sized array of PARTITION_INFORMATION_EX structures, one structure for each partition on the 
            /// drive.
            /// </summary>
            [FieldOffset(48)]
            public PARTITION_INFORMATION_EX PartitionEntry;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PARTITION_INFORMATION_EX {
            /// <summary>
            /// The format of the partition. For a list of values, see PARTITION_STYLE.
            /// </summary>
            [FieldOffset(0)]
            public PARTITION_STYLE PartitionStyle;

            /// <summary>
            /// The starting offset of the partition.
            /// </summary>
            [FieldOffset(8)]
            public Int64 StartingOffset;

            /// <summary>
            /// The length of the partition, in bytes.
            /// </summary>
            [FieldOffset(16)]
            public Int64 PartitionLength;

            /// <summary>
            /// The number of the partition (1-based).
            /// </summary>
            [FieldOffset(24)]
            public uint PartitionNumber;

            /// <summary>
            /// If this member is TRUE, the partition information has changed. When you change a partition (with 
            /// IOCTL_DISK_SET_DRIVE_LAYOUT), the system uses this member to determine which partitions have changed
            /// and need their information rewritten.
            /// </summary>
            [FieldOffset(28)]
            [MarshalAs(UnmanagedType.I1)]
            public bool RewritePartition;

            /// <summary>
            /// A PARTITION_INFORMATION_MBR structure that specifies partition information specific to master boot 
            /// record (MBR) disks. The MBR partition format is the standard AT-style format.
            /// </summary>
            [FieldOffset(32)]
            public PARTITION_INFORMATION_MBR Mbr;

            /// <summary>
            /// A PARTITION_INFORMATION_GPT structure that specifies partition information specific to GUID partition 
            /// table (GPT) disks. The GPT format corresponds to the EFI partition format.
            /// </summary>
            [FieldOffset(32)]
            public PARTITION_INFORMATION_GPT Gpt;
        }

        /// <summary>
        /// Contains partition information specific to master boot record (MBR) disks.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct PARTITION_INFORMATION_MBR {
            #region Constants
            /// <summary>
            /// An unused entry partition.
            /// </summary>
            public const byte PARTITION_ENTRY_UNUSED = 0x00;

            /// <summary>
            /// A FAT12 file system partition.
            /// </summary>
            public const byte PARTITION_FAT_12 = 0x01;

            /// <summary>
            /// A FAT16 file system partition.
            /// </summary>
            public const byte PARTITION_FAT_16 = 0x04;

            /// <summary>
            /// An extended partition.
            /// </summary>
            public const byte PARTITION_EXTENDED = 0x05;

            /// <summary>
            /// An IFS partition.
            /// </summary>
            public const byte PARTITION_IFS = 0x07;

            /// <summary>
            /// A FAT32 file system partition.
            /// </summary>
            public const byte PARTITION_FAT32 = 0x0B;

            /// <summary>
            /// A logical disk manager (LDM) partition.
            /// </summary>
            public const byte PARTITION_LDM = 0x42;

            /// <summary>
            /// An NTFT partition.
            /// </summary>
            public const byte PARTITION_NTFT = 0x80;

            /// <summary>
            /// A valid NTFT partition.
            /// 
            /// The high bit of a partition type code indicates that a partition is part of an NTFT mirror or striped array.
            /// </summary>
            public const byte PARTITION_VALID_NTFT = 0xC0;
            #endregion

            /// <summary>
            /// The type of partition. For a list of values, see Disk Partition Types.
            /// </summary>
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.U1)]
            public byte PartitionType;

            /// <summary>
            /// If this member is TRUE, the partition is bootable.
            /// </summary>
            [FieldOffset(1)]
            [MarshalAs(UnmanagedType.I1)]
            public bool BootIndicator;

            /// <summary>
            /// If this member is TRUE, the partition is of a recognized type.
            /// </summary>
            [FieldOffset(2)]
            [MarshalAs(UnmanagedType.I1)]
            public bool RecognizedPartition;

            /// <summary>
            /// The number of hidden sectors in the partition.
            /// </summary>
            [FieldOffset(4)]
            public uint HiddenSectors;
        }

        /// <summary>
        /// Contains GUID partition table (GPT) partition information.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct PARTITION_INFORMATION_GPT {
            /// <summary>
            /// A GUID that identifies the partition type.
            /// 
            /// Each partition type that the EFI specification supports is identified by its own GUID, which is 
            /// published by the developer of the partition.
            /// </summary>
            [FieldOffset(0)]
            public Guid PartitionType;

            /// <summary>
            /// The GUID of the partition.
            /// </summary>
            [FieldOffset(16)]
            public Guid PartitionId;

            /// <summary>
            /// The Extensible Firmware Interface (EFI) attributes of the partition.
            /// 
            /// </summary>
            [FieldOffset(32)]
            public UInt64 Attributes;

            /// <summary>
            /// A wide-character string that describes the partition.
            /// </summary>
            [FieldOffset(40)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
            public string Name;
        }


        /// <summary>
        /// Moves the file pointer of the specified file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetFilePointer")]
        public unsafe static extern int SetFilePointerWin32(SafeFileHandle handle, int lo, int* hi, int origin);


        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, uint uType,
        int cxDesired, int cyDesired, uint fuLoad);


        public delegate bool EnumResNameDelegate(
        IntPtr hModule,
        IntPtr lpszType,
        IntPtr lpszName,
        IntPtr lParam);


        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceNamesWithID(IntPtr hModule, uint lpszType, EnumResNameDelegate lpEnumFunc, IntPtr lParam);

        

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon(IntPtr handle);


    }
}
