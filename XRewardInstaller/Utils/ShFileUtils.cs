using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Spareio.Installer.Utils
{
    static public class ShFileUtils
    {
        static public bool DeleteOrRename(string fileName)
        {
            // verify the ability to override existing file
            // if it is not possible, we can try to rename it
            if (File.Exists(fileName))
            {
                // delete it
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    // remove read only attribute from the file before attempt to delete it
                    if (fileInfo.IsReadOnly)
                    {
                        fileInfo.IsReadOnly = false;
                    }
                    File.Delete(fileName);
                }
                catch
                {
                    // delete failed, which means that the file is probably locked
                    // we can try to rename it
                    var iRes = Rename(fileName, String.Format("{0}.old.{1}", fileName, DateTime.Now.ToFileTime()));

                    return iRes == 0;
                }
            }

            return true;
        }

        static public int Rename(string from, string to)
        {
            String multiSource = StringArrayToMultiString(new[] { from });
            String multiDest = StringArrayToMultiString(new[] { to });

            var _struct = new InteropSHFileOperation.SHFILEOPSTRUCT
            {
                fFlags = new InteropSHFileOperation.FILEOP_FLAGS().Flag,
                wFunc = (UInt32)InteropSHFileOperation.FO_Func.FO_RENAME,
                pFrom = Marshal.StringToHGlobalUni(multiSource),
                pTo = Marshal.StringToHGlobalUni(multiDest),
                hwnd = IntPtr.Zero,
                fAnyOperationsAborted = 0,
                hNameMappings = IntPtr.Zero,
                lpszProgressTitle = ""
            };

            return InteropSHFileOperation.SHFileOperation(ref _struct);
        }
        private static String StringArrayToMultiString(String[] stringArray)
        {
            String multiString = "";

            if (stringArray == null)
                return "";

            for (int i = 0; i < stringArray.Length; i++)
                multiString += stringArray[i] + '\0';

            multiString += '\0';

            return multiString;
        }
    }

    public class InteropSHFileOperation
    {
        public enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;   // Window handle to the dialog box to display 
            // information about the status of the file 
            // operation. 
            public UInt32 wFunc;   // Value that indicates which operation to 
            // perform.
            public IntPtr pFrom;   // Address of a buffer to specify one or more 
            // source file names. These names must be
            // fully qualified paths. Standard Microsoft®   
            // MS-DOS® wild cards, such as "*", are 
            // permitted in the file-name position. 
            // Although this member is declared as a 
            // null-terminated string, it is used as a 
            // buffer to hold multiple file names. Each 
            // file name must be terminated by a single 
            // NULL character. An additional NULL 
            // character must be appended to the end of 
            // the final name to indicate the end of pFrom. 
            public IntPtr pTo;   // Address of a buffer to contain the name of 
            // the destination file or directory. This 
            // parameter must be set to NULL if it is not 
            // used. Like pFrom, the pTo member is also a 
            // double-null terminated string and is handled 
            // in much the same way. 
            public UInt16 fFlags;   // Flags that control the file operation. 

            public Int32 fAnyOperationsAborted;

            // Value that receives TRUE if the user aborted 
            // any file operations before they were 
            // completed, or FALSE otherwise. 

            public IntPtr hNameMappings;

            // A handle to a name mapping object containing 
            // the old and new names of the renamed files. 
            // This member is used only if the 
            // fFlags member includes the 
            // FOF_WANTMAPPINGHANDLE flag.

            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpszProgressTitle;

            // Address of a string to use as the title of 
            // a progress dialog box. This member is used 
            // only if fFlags includes the 
            // FOF_SIMPLEPROGRESS flag.

        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static public extern int SHFileOperation([In, Out] ref SHFILEOPSTRUCT lpFileOp);

        private SHFILEOPSTRUCT _ShFile;
        public FILEOP_FLAGS fFlags;


        public class FILEOP_FLAGS
        {
            [Flags]
            private enum FILEOP_FLAGS_ENUM : ushort
            {
                FOF_MULTIDESTFILES = 0x0001,
                FOF_CONFIRMMOUSE = 0x0002,
                FOF_SILENT = 0x0004,  // don't create progress/report
                FOF_RENAMEONCOLLISION = 0x0008,
                FOF_NOCONFIRMATION = 0x0010,  // Don't prompt the user.
                FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
                // Must be freed using SHFreeNameMappings
                FOF_ALLOWUNDO = 0x0040,
                FOF_FILESONLY = 0x0080,  // on *.*, do only files
                FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
                FOF_NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
                FOF_NOERRORUI = 0x0400,  // don't put up error UI
                FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
                FOF_NORECURSION = 0x1000,  // don't recurse into directories.
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
                FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
                FOF_NORECURSEREPARSE = 0x8000,  // treat reparse points as objects, not containers
            }

            public bool FOF_MULTIDESTFILES = false;
            public bool FOF_CONFIRMMOUSE = false;
            public bool FOF_SILENT = true;
            public bool FOF_RENAMEONCOLLISION = false;
            public bool FOF_NOCONFIRMATION = true;
            public bool FOF_WANTMAPPINGHANDLE = false;
            public bool FOF_ALLOWUNDO = false;
            public bool FOF_FILESONLY = false;
            public bool FOF_SIMPLEPROGRESS = false;
            public bool FOF_NOCONFIRMMKDIR = false;
            public bool FOF_NOERRORUI = false;
            public bool FOF_NOCOPYSECURITYATTRIBS = false;
            public bool FOF_NORECURSION = false;
            public bool FOF_NO_CONNECTED_ELEMENTS = false;
            public bool FOF_WANTNUKEWARNING = false;
            public bool FOF_NORECURSEREPARSE = false;

            public ushort Flag
            {
                get
                {
                    ushort ReturnValue = 0;

                    if (this.FOF_MULTIDESTFILES)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_MULTIDESTFILES;
                    if (this.FOF_CONFIRMMOUSE)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_CONFIRMMOUSE;
                    if (this.FOF_SILENT)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_SILENT;
                    if (this.FOF_RENAMEONCOLLISION)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_RENAMEONCOLLISION;
                    if (this.FOF_NOCONFIRMATION)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMATION;
                    if (this.FOF_WANTMAPPINGHANDLE)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTMAPPINGHANDLE;
                    if (this.FOF_ALLOWUNDO)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_ALLOWUNDO;
                    if (this.FOF_FILESONLY)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_FILESONLY;
                    if (this.FOF_SIMPLEPROGRESS)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_SIMPLEPROGRESS;
                    if (this.FOF_NOCONFIRMMKDIR)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMMKDIR;
                    if (this.FOF_NOERRORUI)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOERRORUI;
                    if (this.FOF_NOCOPYSECURITYATTRIBS)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCOPYSECURITYATTRIBS;
                    if (this.FOF_NORECURSION)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSION;
                    if (this.FOF_NO_CONNECTED_ELEMENTS)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NO_CONNECTED_ELEMENTS;
                    if (this.FOF_WANTNUKEWARNING)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTNUKEWARNING;
                    if (this.FOF_NORECURSEREPARSE)
                        ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSEREPARSE;

                    return ReturnValue;
                }
            }
        }

    }
}
