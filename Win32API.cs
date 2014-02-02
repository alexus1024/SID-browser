using System;
using System.Runtime.InteropServices;

namespace Networking
{
	internal class Win32API
	{
		#region Win32 API Interfaces
		[DllImport( "netapi32.dll", EntryPoint = "NetApiBufferFree" )]
		internal static extern void NetApiBufferFree(IntPtr bufptr);

		[DllImport( "netapi32.dll", EntryPoint = "NetLocalGroupGetMembers" )]
		internal static extern uint NetLocalGroupGetMembers(
			IntPtr ServerName,
			IntPtr GrouprName,
			uint level,
			ref IntPtr siPtr,
			uint prefmaxlen,
			ref uint entriesread,
			ref uint totalentries,
			IntPtr resumeHandle);

		[DllImport( "netapi32.dll", EntryPoint = "NetLocalGroupEnum" )]
		internal static extern uint NetLocalGroupEnum(
			IntPtr ServerName, 
			uint level,
			ref IntPtr siPtr,
			uint prefmaxlen,
			ref uint entriesread,
			ref uint totalentries,
			IntPtr resumeHandle);

		[DllImport("Advapi32.dll", EntryPoint = "ConvertSidToStringSid")]
		internal static extern uint ConvertSidToStringSid(
			IntPtr sid,
			[MarshalAs(UnmanagedType.LPStr)] out String result
			);

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		internal struct LOCALGROUP_MEMBERS_INFO_1
		{ 
			public IntPtr lgrmi1_sid;
			public IntPtr lgrmi1_sidusage;
			public IntPtr lgrmi1_name;

		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		internal struct LOCALGROUP_INFO_1 
		{ 
			public IntPtr lpszGroupName;
			public IntPtr lpszComment;
		}


		#endregion
	}
}