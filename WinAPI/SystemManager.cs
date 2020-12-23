#region 禁用以下代码，目前用不到
/*
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace WinAPI.SystemManager
{
    /// <summary>
    /// 枚举类型,指定可以允许的重启操作
    /// </summary>
    public enum RestartOptions
    {
        /// <summary>
        /// Shuts down all processes running in the security context of the process that called the ExitWindowsEx function. Then it logs the user off.
        /// 注销，关闭调用ExitWindowsEx()功能的进程安全上下文中所有运行的程序，然后用户退出登录
        /// </summary>
        LogOff = 0,
        /// <summary>
        /// Shuts down the system and turns off the power. The system must support the power-off feature.
        /// 关闭操作系统和电源，计算机必须支持软件控制电源
        /// </summary>
        PowerOff = 8,
        /// <summary>
        /// Shuts down the system and then restarts the system.
        /// 关闭系统然后重启
        /// </summary>
        Reboot = 2,
        /// <summary>
        /// Shuts down the system to a point at which it is safe to turn off the power. All file buffers have been flushed to disk, and all running processes have stopped. If the system supports the power-off feature, the power is also turned off.
        /// 关闭系统，等待合适的时刻关闭电源：当所有文件的缓存区被写入磁盘，所有运行的进程停止，如果系统支持软件控制电源，就关闭电源
        /// </summary>
        ShutDown = 1,
        /// <summary>
        /// Suspends the system.
        /// 挂起
        /// </summary>
        Suspend = -1,
        /// <summary>
        /// Hibernates the system.
        /// 休眠
        /// </summary>
        Hibernate = -2,
    }
    /// <summary>
    /// An LUID is a 64-bit value guaranteed to be unique only on the system on which it was generated. The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted.
    /// 本地唯一标志是一个64位的数值，它被保证在产生它的系统上唯一！LUID的在机器被重启前都是唯一的
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LUID
    {
        /// <summary>
        /// The low order part of the 64 bit value.
        /// 本地唯一标志的低32位
        /// </summary>
        public int LowPart;
        /// <summary>
        /// The high order part of the 64 bit value.
        /// 本地唯一标志的高32位
        /// </summary>
        public int HighPart;
    }
    /// <summary>
    /// The LUID_AND_ATTRIBUTES structure represents a locally unique identifier (LUID) and its attributes.
    /// LUID_AND_ATTRIBUTES 结构呈现了本地唯一标志和它的属性
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LUID_AND_ATTRIBUTES
    {
        /// <summary>
        /// Specifies an LUID value.
        /// </summary>
        public LUID pLuid;
        /// <summary>
        /// Specifies attributes of the LUID. This value contains up to 32 one-bit flags. Its meaning is dependent on the definition and use of the LUID.
        /// 指定了LUID的属性，其值可以是一个32位大小的bit 标志，具体含义根据LUID的定义和使用来看
        /// </summary>
        public int Attributes;
    }
    /// <summary>
    /// The TOKEN_PRIVILEGES structure contains information about a set of privileges for an access token.
    /// TOKEN_PRIVILEGES 结构包含了一个访问令牌的一组权限信息：即该访问令牌具备的权限
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TOKEN_PRIVILEGES
    {
        /// <summary>
        /// Specifies the number of entries in the Privileges array.
        /// 指定了权限数组的容量
        /// </summary>
        public int PrivilegeCount;
        /// <summary>
        /// Specifies an array of LUID_AND_ATTRIBUTES structures. Each structure contains the LUID and attributes of a privilege.
        /// 指定一组的LUID_AND_ATTRIBUTES 结构，每个结构包含了LUID和权限的属性
        /// </summary>
        public LUID_AND_ATTRIBUTES Privileges;
    }
    /// <summary>
    /// 实现了退出Windows的方法
    /// </summary>
    public class WindowsController
    {
        /// <summary>用来在访问令牌中启用和禁用一个权限</summary>
        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        /// <summary>用来查询一个访问令牌</summary>
        private const int TOKEN_QUERY = 0x8;
        /// <summary>权限启用标志</summary>
        private const int SE_PRIVILEGE_ENABLED = 0x2;
        /// <summary>指定了函数需要为请求消息查找系统消息表资源 </summary>
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        /// <summary>
        /// 强制停止进程。当设置了该标志后，系统不会发送WM_QUERYENDSESSION 和 WM_ENDSSESSION消息，这会使应用程序丢失数据。
        /// 因此，除非紧急，你要慎用该标志
        /// </summary>
        private const int EWX_FORCE = 4;
        /// <summary>
        /// LoadLibrary函数将指定的可执行模块映射到调用进程的地址空间
        /// </summary>
        /// <param name="lpLibFileName">可执行模块(dll or exe)的名字：以null结束的字符串指针. 
        /// 该名称是可执行模块的文件名，与模块本身存储的,用关键字LIBRARY在模块定义文件(.def)中指定的名称无关,</param>
        /// <returns>如果执行成功, 返回模块的句柄如果执行失败, 返回 NULL. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);
        /// <summary>
        /// FreeLibrary函数将装载的dll引用计数器减一，当引用计数器的值为0后，模块将从调用进程的地址空间退出，模块的句柄将不可再用
        /// </summary>
        /// <param name="hLibModule">dll模块的句柄. LoadLibrary 或者 GetModuleHandle 函数返回该句柄</param>
        /// <returns>如果执行成功, 返回值为非0,如果失败，返回值为0. 如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
        private static extern int FreeLibrary(IntPtr hLibModule);
        /// <summary>
        /// GetProcAddress 函数获取外部函数的入口地址，或者从指定的DLL获取变量信息
        /// </summary>
        /// <param name="hModule">Dll的句柄，包含了函数或者变量，LoadLibrary 或 GetModuleHandle 函数返回该句柄 </param>
        /// <param name="lpProcName">以null结束的字符串指针，包含函数或者变量名，或者函数的顺序值，如果该参数是一个顺序值，
        /// 它必须是低序字be in the low-order word,高序字(the high-order)必须为0</param>
        /// <returns>如果执行成功, 返回值为外部函数或变量的地址如果执行失败，返回值为NULL. 
        /// 如果要获取更多错误信息，请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        /// <summary>
        /// SetSuspendState函数关闭电源挂起系统，根据休眠参数设置，系统将挂起或者休眠，如果ForceFlag为真，
        /// 系统将立即停止所有操作，如果为假，系统将征求所有应用程序和驱动程序意见后才会这么做
        /// </summary>
        /// <param name="Hibernate">休眠参数，如果为true，系统进入修改，如果为false，系统挂起</param>
        /// <param name="ForceCritical">强制挂起. 如果为TRUE, 函数向每个应用程序和驱动广播一个 PBT_APMSUSPEND 事件, 然后立即挂起所有操作。
        /// 如果为 FALSE, 函数向每个应用程序广播一个 PBT_APMQUERYSUSPEND 事件，征求挂起</param>
        /// <param name="DisableWakeEvent">如果为 TRUE, 系统禁用所有唤醒事件，如果为 FALSE, 所有系统唤醒事件继续有效</param>
        /// <returns>如果执行成功，返回非0<br></br><br>如果执行失败, 返回0. 如果要获取更多错误信息，请调用 Marshal.GetLastWin32Error.</returns>
        [DllImport("powrprof.dll", EntryPoint = "SetSuspendState", CharSet = CharSet.Ansi)]
        private static extern int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent);
        /// <summary>
        /// OpenProcessToken 函数与进程关联的访问令牌
        /// </summary>
        /// <param name="ProcessHandle">打开访问令牌进程的句柄</param>
        /// <param name="DesiredAccess">一个访问符，指定需要的访问令牌的访问类型。
        /// 这些访问类型与访问令牌自定义的访问控制列表(DACL)比较后，决定哪些访问是允许的，哪些是拒绝的</param>
        /// <param name="TokenHandle">句柄指针:标志了刚刚打开的由函数返回的访问令牌</param>
        /// <returns>如果执行成功，返回非0<br></br><br>如果执行失败，返回0. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", CharSet = CharSet.Ansi)]
        private static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);
        /// <summary>
        /// LookupPrivilegeValue函数返回本地唯一标志LUID，用于在指定的系统上代表特定的权限名
        /// </summary>
        /// <param name="lpSystemName">以null结束的字符串指针，标志了在其上查找权限名的系统名称. 如果设置为null, 函数将试图查找指定系统的权限名.</param>
        /// <param name="lpName">以null结束的字符串指针，指定了在Winnt.h头文件中定义的权限名. 例如, 该参数可以是一个常量 SE_SECURITY_NAME, 或者对应的字符串 "SeSecurityPrivilege".</param>
        /// <param name="lpLuid">接收本地唯一标志LUID的变量指针，通过它可以知道由lpSystemName 参数指定的系统上的权限.</param>
        /// <returns>如果执行成功，返回非0<br></br><br>如果执行失败，返回0，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.</br></returns>
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi)]
        private static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);
        /// <summary>
        /// AdjustTokenPrivileges 函数可以启用或禁用指定访问令牌的权限. 在一个访问令牌中启用或禁用一个权限需要 TOKEN_ADJUST_PRIVILEGES 访问权限.
        /// </summary>
        /// <param name="TokenHandle">需要改变权限的访问令牌句柄. 句柄必须含有对令牌的 TOKEN_ADJUST_PRIVILEGES 访问权限. 
        /// 如果 PreviousState 参数非null, 句柄还需要有 TOKEN_QUERY 访问权限.</param>
        /// <param name="DisableAllPrivileges">执行函数是否禁用访问令牌的所有权限. 如果参数值为 TRUE, 
        /// 函数将禁用所有权限并忽略 NewState 参数. 如果其值为 FALSE, 函数将根据NewState参数指向的信息改变权限.</param>
        /// <param name="NewState">一个 TOKEN_PRIVILEGES 结构的指针，指定了一组权限以及它们的属性. 
        /// 如果 DisableAllPrivileges 参数为 FALSE, AdjustTokenPrivileges 函数将启用或禁用访问令牌的这些权限. 
        /// 如果你为一个权限设置了 SE_PRIVILEGE_ENABLED 属性, 本函数将启用该权限; 否则, 它将禁用该权限. 
        /// 如果 DisableAllPrivileges 参数为 TRUE, 本函数忽略此参数.</param>
        /// <param name="BufferLength">为PreviousState参数指向的缓冲区用字节设置大小. 如果PreviousState 参数为 NULL，此参数可以为0</param>
        /// <param name="PreviousState">一个缓冲区指针，被函数用来填充 TOKENT_PRIVILEGES结构，它包含了被函数改变的所有权限的先前状态. 此参数可以为 NULL.</param>
        /// <param name="ReturnLength">一个变量指针，指示了由PreviousState参数指向的缓冲区的大小.如果 PreviousState 参数为 NULL，此参数可以为NULL .</param>
        /// <returns>如果执行成功，返回非0. 如果要检测函数是否调整了指定的权限, 请调用 Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
        private static extern int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);
        /// <summary>
        /// ExitWindowEx函数要么注销当前用户,关闭系统, 要么关闭系统然后重新启动. 它发送 WM_QUERYENDSESSION 给所有的应用程序，决定是否可以停止它们的操作.
        /// </summary>
        /// <param name="uFlags">指定关闭的类型.</param>
        /// <param name="dwReserved">该参数忽略.</param>
        /// <returns>如果执行成功，返回非0.<br></br><br>如果执行失败，返回0. 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error.</br></returns>
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        private static extern int ExitWindowsEx(int uFlags, int dwReserved);
        /// <summary>
        /// FormatMessage格式化消息字符串. 该函数需要一个已定义的消息参数作为输入. 消息的定义从一个缓冲区传入函数，它也可以从一个已载入模块的消息表资源中获取. 
        /// 调用者也可以要求函数搜索系统的消息表来查找消息定义. 函数根据消息ID号和语言ID号从消息表资源中查找消息定义.
        /// 函数最终将格式化的消息文本拷贝到输出缓冲区, 要求处理任何内嵌的顺序.
        /// </summary>
        /// <param name="dwFlags">指定格式化处理和如何翻译 lpSource 参数. dwFlags的低字节指定函数如何处理输出缓冲区的换行. 
        /// 低字节也可以指定格式化后的输出缓冲区的最大宽度.</param>
        /// <param name="lpSource">指定消息定义的位置. 此参数的类型依据 dwFlags 参数的设定.</param>
        /// <param name="dwMessageId">指定消息的消息标志ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING ，则该参数被忽略.</param>
        /// <param name="dwLanguageId">指定消息的语言ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING，则该参数被忽略.</param>
        /// <param name="lpBuffer">用来放置格式化消息(以null结束)的缓冲区.如果 dwFlags 参数包括 FORMAT_MESSAGE_ALLOCATE_BUFFER, 
        /// 本函数将使用LocalAlloc函数定位一个缓冲区，然后将缓冲区指针放到 lpBuffer 指向的地址.</param>
        /// <param name="nSize">如果没有设置 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志, 此参数指定了输出缓冲区可以容纳的TCHARs最大个数. 
        /// 如果设置了 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志，则此参数指定了输出缓冲区可以容纳的TCHARs 的最小个数. 对于ANSI文本, 容量为bytes的个数; 
        /// 对于Unicode 文本, 容量为字符的个数.</param>
        /// <param name="Arguments">数组指针,用于在格式化消息中插入信息. 格式字符串中的 A %1 指示参数数组中的第一值; a %2 表示第二个值; 以此类推.</param>
        /// <returns>如果执行成功, 返回值为存储在输出缓冲区的TCHARs个数, 包括了null结束符.
        /// 如果执行失败, 返回值为0. 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error.</br></returns>
        [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
        private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments);
        /// <summary>
        /// 退出Windows,如果需要,申请相应权限
        /// </summary>
        /// <param name="how">重启选项,指示如何退出Windows</param>
        /// <param name="force">True表示强制退出</param>
        /// <exception cref="PrivilegeException">当申请权限时发生了一个错误</exception>
        /// <exception cref="PlatformNotSupportedException">系统不支持则引发异常</exception>
        public static void ExitWindows(RestartOptions how, bool force)
        {
            switch (how)
            {
                case RestartOptions.Suspend:
                    SuspendSystem(false, force);
                    break;
                case RestartOptions.Hibernate:
                    SuspendSystem(true, force);
                    break;
                default:
                    ExitWindows((int)how, force);
                    break;
            }
        }
        /// <summary>
        /// 退出Windows,如果需要,申请所有权限
        /// </summary>
        /// <param name="how">重启选项,指示如何退出Windows</param>
        /// <param name="force">True表示强制退出</param>
        /// <remarks>本函数无法挂起或休眠系统</remarks>
        /// <exception cref="PrivilegeException">当申请一个权限时发生了错误</exception>
        protected static void ExitWindows(int how, bool force)
        {
            EnableToken("SeShutdownPrivilege");
            if (force)
                how = how | EWX_FORCE;
            if (ExitWindowsEx(how, 0) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }
        /// <summary>
        /// 启用指定的权限
        /// </summary>
        /// <param name="privilege">要启用的权限</param>
        /// <exception cref="PrivilegeException">表明在申请相应权限时发生了错误</exception>
        protected static void EnableToken(string privilege)
        {
            if (!CheckEntryPoint("advapi32.dll", "AdjustTokenPrivileges"))
                return;
            IntPtr tokenHandle = IntPtr.Zero;
            LUID privilegeLUID = new LUID();
            TOKEN_PRIVILEGES newPrivileges = new TOKEN_PRIVILEGES();
            TOKEN_PRIVILEGES tokenPrivileges;
            if (OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
            if (LookupPrivilegeValue("", privilege, ref privilegeLUID) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
            tokenPrivileges.PrivilegeCount = 1;
            tokenPrivileges.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
            tokenPrivileges.Privileges.pLuid = privilegeLUID;
            int size = 4;
            if (AdjustTokenPrivileges(tokenHandle, 0, ref tokenPrivileges, 4 + (12 * tokenPrivileges.PrivilegeCount), ref newPrivileges, ref size) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }
        /// <summary>
        /// 挂起或休眠系统
        /// </summary>
        /// <param name="hibernate">True表示休眠，否则表示挂起系统</param>
        /// <param name="force">True表示强制退出</param>
        /// <exception cref="PlatformNotSupportedException">如果系统不支持，将抛出PlatformNotSupportedException.</exception>
        protected static void SuspendSystem(bool hibernate, bool force)
        {
            if (!CheckEntryPoint("powrprof.dll", "SetSuspendState"))
                throw new PlatformNotSupportedException("The SetSuspendState method is not supported on this system!");
            SetSuspendState((int)(hibernate ? 1 : 0), (int)(force ? 1 : 0), 0);
        }
        /// <summary>
        /// 检测本地系统上是否存在一个指定的方法入口
        /// </summary>
        /// <param name="library">包含指定方法的库文件</param>
        /// <param name="method">指定方法的入口</param>
        /// <returns>如果存在指定方法，返回True，否则返回False</returns>
        protected static bool CheckEntryPoint(string library, string method)
        {
            IntPtr libPtr = LoadLibrary(library);
            if (!libPtr.Equals(IntPtr.Zero))
            {
                if (!GetProcAddress(libPtr, method).Equals(IntPtr.Zero))
                {
                    FreeLibrary(libPtr);
                    return true;
                }
                FreeLibrary(libPtr);
            }
            return false;
        }
        /// <summary>
        /// 将错误号转换为错误消息
        /// </summary>
        /// <param name="number">需要转换的错误号</param>
        /// <returns>代表指定错误号的字符串.</returns>
        protected static string FormatError(int number)
        {
            StringBuilder buffer = new StringBuilder(255);
            FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, number, 0, buffer, buffer.Capacity, 0);
            return buffer.ToString();
        }
    }

    /// <summary>
    /// 如果在申请一个权限时发生错，将引发本异常
    /// </summary>
    public class PrivilegeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the PrivilegeException class.
        /// </summary>
        public PrivilegeException() : base() { }
        /// <summary>
        /// Initializes a new instance of the PrivilegeException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PrivilegeException(string message) : base(message) { }
    }
}
*/
#endregion