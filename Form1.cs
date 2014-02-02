using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;	// for DllImport, MarshalAs, etc

namespace Networking
{
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button groupbtn;
		protected static Int64 LOCALGROUP_MEMBERS_INFO_1_SIZE;
		protected static Int64 LOCALGROUP_INFO_1_SIZE;
		private System.Windows.Forms.TreeView grouptv;
		private string[] commentArray;
		private System.Windows.Forms.Label label1;
		private TextBox descriptionlb;
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
			this.label1 = new System.Windows.Forms.Label();
			this.descriptionlb = new System.Windows.Forms.TextBox();
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
			this.grouptv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grouptv.Location = new System.Drawing.Point(24, 48);
			this.grouptv.Name = "grouptv";
			this.grouptv.Size = new System.Drawing.Size(246, 216);
			this.grouptv.TabIndex = 2;
			this.grouptv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.grouptv_AfterSelect);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(276, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 176);
			this.label1.TabIndex = 4;
			this.label1.Text = "Select each node to see description and members in local computer.";
			this.label1.Visible = false;
			// 
			// descriptionlb
			// 
			this.descriptionlb.AcceptsReturn = true;
			this.descriptionlb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptionlb.Location = new System.Drawing.Point(24, 280);
			this.descriptionlb.Multiline = true;
			this.descriptionlb.Name = "descriptionlb";
			this.descriptionlb.Size = new System.Drawing.Size(356, 74);
			this.descriptionlb.TabIndex = 5;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 366);
			this.Controls.Add(this.descriptionlb);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.grouptv);
			this.Controls.Add(this.groupbtn);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

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
				Int64 newOffset = GroupInfoPtr.ToInt64() + LOCALGROUP_INFO_1_SIZE * i;
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
					Int64 newOffset1 = UserInfoPtr.ToInt64() + LOCALGROUP_MEMBERS_INFO_1_SIZE * j;
					Win32API.LOCALGROUP_MEMBERS_INFO_1 memberInfo = (Win32API.LOCALGROUP_MEMBERS_INFO_1)Marshal.PtrToStructure(new IntPtr(newOffset1), typeof(Win32API.LOCALGROUP_MEMBERS_INFO_1));

					String stringSid;
					Win32API.ConvertSidToStringSid(memberInfo.lgrmi1_sid, out stringSid);

					var currentUserName = Marshal.PtrToStringAuto(memberInfo.lgrmi1_name);
					var sid = stringSid;
					var sidUsage = memberInfo.lgrmi1_sidusage;
					//adding member name to tree view
					var userNode = new TreeNode(currentUserName)
						{
							Tag = new {Sid = sid, SidUsage = sidUsage},
						};
					grouptv.Nodes[i].Nodes.Add(userNode);
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
			dynamic tag = tr.SelectedNode.Tag;
			string path = tr.SelectedNode.FullPath;
			if(path.IndexOf("\\")==-1)
				descriptionlb.Text = commentArray[tr.SelectedNode.Index];
			else
			{
				//path = path.Substring(path.IndexOf("\\")+1);
				descriptionlb.Text = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, path, tag.Sid, tag.SidUsage);
			}
		}
	}
}
