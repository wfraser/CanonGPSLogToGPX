using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CanonGPSLogWPF
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern int SHOpenFolderAndSelectItems(
            IntPtr pidlFolder,
            uint cidl,
            IntPtr apidl,
            uint dwFlags
            );

        [DllImport("shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern IntPtr ILCreateFromPathW(
            string pszPath
            );

        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern void ILFree(
            IntPtr pidlList
            );
    }
}
