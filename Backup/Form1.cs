using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;	// for DllImport, MarshalAs, etc

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

		[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			internal struct LOCALGROUP_MEMBERS_INFO_1
		{ 
			public IntPtr lgrmi1_sid;
			public IntPtr lgrmi1_sidusage;
			public IntPtr lgrmi1_name;

		}

		[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			internal struct LOCALGROUP_INFO_1 
		{ 
			public IntPtr lpszGroupName;
			public IntPtr lpszComment;
		}
		#endregion
	}


	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button groupbtn;
		protected static int LOCALGROUP_MEMBERS_INFO_1_SIZE;
		protected static int LOCALGROUP_INFO_1_SIZE;
		private System.Windows.Forms.TreeView grouptv;
        private string[] commentArray;
		private System.Windows.Forms.Label descriptionlb;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			unsafe
			{
				LOCALGROUP_INFO_1_SIZE = sizeof(Win32API.LOCALGROUP_INFO_1);
				LOCALGROUP_MEMBERS_INFO_1_SIZE = sizeof(Win32API.LOCALGROUP_MEMBERS_INFO_1);
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.groupbtn = new System.Windows.Forms.Button();
			this.grouptv = new System.Windows.Forms.TreeView();
			this.descriptionlb = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// groupbtn
			// 
			this.groupbtn.Location = new System.Drawing.Point(24, 16);
			this.groupbtn.Name = "groupbtn";
			this.groupbtn.Size = new System.Drawing.Size(160, 23);
			this.groupbtn.TabIndex = 0;
			this.groupbtn.Text = "Groups and Members";
			this.groupbtn.Click += new System.EventHandler(this.groupbtn_Click);
			// 
			// grouptv
			// 
			this.grouptv.ImageIndex = -1;
			this.grouptv.Location = new System.Drawing.Point(24, 48);
			this.grouptv.Name = "grouptv";
			this.grouptv.SelectedImageIndex = -1;
			this.grouptv.Size = new System.Drawing.Size(200, 216);
			this.grouptv.TabIndex = 2;
			this.grouptv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.grouptv_AfterSelect);
			// 
			// descriptionlb
			// 
			this.descriptionlb.Location = new System.Drawing.Point(32, 280);
			this.descriptionlb.Name = "descriptionlb";
			this.descriptionlb.Size = new System.Drawing.Size(216, 80);
			this.descriptionlb.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(240, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 176);
			this.label1.TabIndex = 4;
			this.label1.Text = "Select each node to see description and members in local computer.";
			this.label1.Visible = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 366);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1,
																		  this.descriptionlb,
																		  this.grouptv,
																		  this.groupbtn});
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void groupbtn_Click(object sender, System.EventArgs e)
		{
			//defining values for getting group names
			uint level = 1, prefmaxlen = 0xFFFFFFFF, entriesread = 0, totalentries = 0;

			//Values that will receive information.
			IntPtr GroupInfoPtr,UserInfoPtr;
			GroupInfoPtr = IntPtr.Zero;
			UserInfoPtr = IntPtr.Zero;

			Win32API.NetLocalGroupEnum(
				IntPtr.Zero, //Server name.it must be null
				level,//level can be 0 or 1 for groups.For more information see LOCALGROUP_INFO_0 and LOCALGROUP_INFO_1
				ref GroupInfoPtr,//Value that will be receive information
				prefmaxlen,//maximum length
				ref entriesread,//value that receives the count of elements actually enumerated. 
				ref totalentries,//value that receives the approximate total number of entries that could have been enumerated from the current resume position.
				IntPtr.Zero);

			//this string array will hold comments of each group
			commentArray = new string[totalentries];

			grouptv.Nodes.Clear();
			label1.Visible = true;

			//getting group names and add them to tree view
			for(int i = 0; i<totalentries; i++)
			{
				//converting unmanaged code to managed codes with using Marshal class 
				int newOffset = GroupInfoPtr.ToInt32() + LOCALGROUP_INFO_1_SIZE * i;
				Win32API.LOCALGROUP_INFO_1 groupInfo = (Win32API.LOCALGROUP_INFO_1)Marshal.PtrToStructure(new IntPtr(newOffset), typeof(Win32API.LOCALGROUP_INFO_1));
				string currentGroupName = Marshal.PtrToStringAuto(groupInfo.lpszGroupName);
				//storing group comment in an string array to show it in a label later
                commentArray[i] = Marshal.PtrToStringAuto(groupInfo.lpszComment);
				//add group name to tree
				grouptv.Nodes.Add(currentGroupName);

				//defining value for getting name of members in each group
			    uint prefmaxlen1 = 0xFFFFFFFF, entriesread1 = 0, totalentries1 = 0;

				//paramaeters for NetLocalGroupGetMembers is like NetLocalGroupEnum.
				Win32API.NetLocalGroupGetMembers(IntPtr.Zero,groupInfo.lpszGroupName,1,	ref UserInfoPtr,prefmaxlen1,ref entriesread1,ref totalentries1,	IntPtr.Zero);

				//getting members name
				for(int j = 0; j<totalentries1; j++)
				{
					//converting unmanaged code to managed codes with using Marshal class 
					int newOffset1 = UserInfoPtr.ToInt32() + LOCALGROUP_MEMBERS_INFO_1_SIZE * j;
					Win32API.LOCALGROUP_MEMBERS_INFO_1 memberInfo = (Win32API.LOCALGROUP_MEMBERS_INFO_1)Marshal.PtrToStructure(new IntPtr(newOffset1), typeof(Win32API.LOCALGROUP_MEMBERS_INFO_1));
					string currentUserName = Marshal.PtrToStringAuto(memberInfo.lgrmi1_name);
					//adding member name to tree view
					grouptv.Nodes[i].Nodes.Add(currentUserName);
				}
				//free memory
				Win32API.NetApiBufferFree(UserInfoPtr);
			}
			//free memory
			Win32API.NetApiBufferFree(GroupInfoPtr);
		}

		private void grouptv_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
            TreeView tr = (TreeView)sender;
			string path = tr.SelectedNode.FullPath;
			if(path.IndexOf("\\")==-1)
				descriptionlb.Text = commentArray[tr.SelectedNode.Index];
			else
			{
				path = path.Substring(path.IndexOf("\\")+1);
				descriptionlb.Text = path;
			}
		}
	}
}
