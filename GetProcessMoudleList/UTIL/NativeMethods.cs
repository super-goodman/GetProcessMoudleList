﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BypassCrc
{
    public class NativeMethods
    {

        [DllImport("Kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern Ntstatus RtlCreateUserThread(IntPtr processHandle, IntPtr threadSecurity, bool createSuspended, Int32 stackZeroBits, IntPtr stackReserved, IntPtr stackCommit, IntPtr startAddress, IntPtr parameter, ref IntPtr threadHandle, IntPtr clientId);
       
        [DllImport("ntdll.dll")]
        public static extern Ntstatus NtCreateSection(ref IntPtr sectionHandle, AccessMask DesiredAccess, IntPtr objectAttributes, ref long MaximumSize, MemoryProtectionConstraints SectionPageProtection, SectionProtectionConstraints AllocationAttributes, IntPtr fileHandle);
        [DllImport("ntdll.dll")]
        public static extern void NtResumeProcess(IntPtr processHandle);
        [DllImport("ntdll.dll")]
        public static extern void NtSuspendProcess(IntPtr processHandle);
        [DllImport("ntdll.dll")]
        public static extern Ntstatus NtUnmapViewOfSection(IntPtr processHandle, IntPtr baseAddress);
        [DllImport("ntdll.dll")]
        public static extern Ntstatus NtMapViewOfSection(IntPtr sectionHandle, IntPtr processHandle, ref IntPtr baseAddress, UIntPtr ZeroBits, int commitSize, ref long SectionOffset, ref uint ViewSize, uint InheritDisposition, MemoryAllocationType allocationType, MemoryProtectionConstraints win32Protect);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int size, out IntPtr lpNumberOfBytesRead);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, out IntPtr lpNumberOfBytesRead);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out Int64 lpBuffer, int size, out IntPtr lpNumberOfBytesRead);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out float lpBuffer, int size, out IntPtr lpNumberOfBytesRead);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, in float lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, in byte lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, MemoryAllocationType flAllocationType, MemoryProtectionConstraints flProtect);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryAllocationType flAllocationType, MemoryProtectionConstraints flProtect);
        [DllImport("Kernel32.dll")]
        public static extern bool VirtualFree(IntPtr lpAddress, int dwSize, MemFree dwFreeType);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr handle, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
        [DllImport("psapi.dll", SetLastError = true)]
        public static extern uint GetMappedFileName(IntPtr m_hProcess, IntPtr lpv, StringBuilder lpFilename, uint nSize);
        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtectionConstraints flNewProtect, out MemoryProtectionConstraints lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
    }

    public enum AccessMask : uint
    {
 
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        SECTION_QUERY = 0x0001,
        SECTION_MAP_WRITE = 0x0002,
        SECTION_MAP_READ = 0x0004,
        SECTION_MAP_EXECUTE = 0x0008,
        SECTION_EXTEND_SIZE = 0x0010,
        SECTION_MAP_EXECUTE_EXPLICIT = 0x0020,
        SECTION_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SECTION_QUERY | SECTION_MAP_WRITE | SECTION_MAP_READ | SECTION_MAP_EXECUTE | SECTION_EXTEND_SIZE)
    }
    public enum ProcessAccessFlags
    {
        PROCESS_ALL_ACCESS = 0xFFFF,
        PROCESS_QUERY_INFORMATION = 0x400,
    }
    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr baseAddress;
        public IntPtr allocationBase;
        public MemoryProtectionConstraints allocationProtect;
        public IntPtr regionSize;
        public State state;
        public MemoryProtectionConstraints protect;
        public Type type;
    }

    public struct SYSTEM_INFO
    {
        public ushort processorArchitecture;
        //ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;  // minimum address
        public IntPtr maximumApplicationAddress;  // maximum address
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }

    public enum MemoryAllocationType
    {
        MEM_COMMIT = 0x00001000,
        MEM_RESERVE = 0x00002000,


    }
    public enum MemoryProtectionConstraints : uint
    {
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_TARGETS_INVALID = 0x40000000,
        PAGE_TARGETS_NO_UPDATE = 0x40000000,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400,

    }
    public enum Ntstatus : uint
    {
        STATUS_ACCESS_VIOLATION = 3221225477,
        STATUS_SUCCESS = 0,
        STATUS_FILE_LOCK_CONFLICT = 0xC0000054,
        STATUS_INVALID_FILE_FOR_SECTION = 0xC0000020,
        STATUS_INVALID_PAGE_PROTECTION = 0xC0000045,
        STATUS_MAPPED_FILE_SIZE_ZERO = 0xC000011E,
        STATUS_SECTION_TOO_BIG = 0xC0000040,
    }
    public enum SectionProtectionConstraints
    {
        SEC_COMMIT = 0x08000000,
    }
    public enum State
    {
        MEM_COMMIT = 0x1000,
        MEM_FREE = 0x10000,
        MEM_RESERVE = 0x2000,
    }
    public enum Type
    {
        MEM_IMAGE = 0x1000000,
        MEM_MAPPED = 0x40000,
        MEM_PRIVATE = 0x20000,
    }
    public enum MemFree
    {
        MEM_RELEASE = 0x00008000,
    }
}
