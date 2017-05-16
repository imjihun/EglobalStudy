using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace ConfigEditor_proj
{
	/// <summary>
	/// Window_Search.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_Search : Window
	{
		public Window_Search()
		{
			InitializeComponent();
		}
		//private void frmTreeView_Load(object sender, EventArgs e)
		//{
		//	//현재 사용자 정보 표시
		//	System.Security.Principal.WindowsIdentity identity =
		//		   System.Security.Principal.WindowsIdentity.GetCurrent();
		//	lblLogin.Text = "현재 사용자 : " + identity.Name;

		//	//컴퓨터 존재하는 드라이브 정보 검색하여 노트 추가하기
		//	DriveInfo[] allDrives = DriveInfo.GetDrives();

		//	foreach(DriveInfo dname in allDrives)
		//	{
		//		if(dname.DriveType == DriveType.Fixed)
		//		{
		//			if(dname.Name == @"C:\")
		//			{
		//				TreeNode rootNode = new TreeNode(dname.Name);
		//				rootNode.ImageIndex = 0;
		//				rootNode.SelectedImageIndex = 0;
		//				treeView1.Nodes.Add(rootNode);
		//				Fill(rootNode);
		//			}
		//			else
		//			{
		//				TreeNode rootNode = new TreeNode(dname.Name);
		//				rootNode.ImageIndex = 1;
		//				rootNode.SelectedImageIndex = 1;
		//				treeView1.Nodes.Add(rootNode);
		//				Fill(rootNode);
		//			}
		//		}
		//	}

		//	treeView1.Nodes[0].Expand();

		//	//ListView 보기 속성 설정
		//	listView1.View = View.Details;

		//	//ListView Details 속성을 위해서 헤더를 추가해줌
		//	listView1.Columns.Add("이름", listView1.Width / 4, HorizontalAlignment.Left);
		//	listView1.Columns.Add("수정한 날짜", listView1.Width / 4, HorizontalAlignment.Left);
		//	listView1.Columns.Add("유형", listView1.Width / 4, HorizontalAlignment.Left);
		//	listView1.Columns.Add("크기", listView1.Width / 4, HorizontalAlignment.Left);

		//	//행단위 선택가능
		//	listView1.FullRowSelect = true;
		//}

		////트리노드에 디렉토리 정보 채우기
		//private void Fill(TreeNode dirNode)
		//{
		//	try
		//	{
		//		DirectoryInfo dir = new DirectoryInfo(dirNode.FullPath);
		//		foreach(DirectoryInfo dirItem in dir.GetDirectories())
		//		{
		//			// Add node for the directory.
		//			TreeNode newNode = new TreeNode(dirItem.Name);
		//			newNode.ImageIndex = 2;
		//			newNode.SelectedImageIndex = 2;
		//			dirNode.Nodes.Add(newNode);
		//			newNode.Nodes.Add("*");
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		MessageBox.Show("에러 발생 : " + ex.Message);
		//	}
		//}

		////트리노드가 확장되기 전에 발생하는 이벤트
		//private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		//{
		//	if(e.Node.Nodes[0].Text == "*")
		//	{
		//		e.Node.Nodes.Clear();
		//		Fill(e.Node);
		//	}
		//}

		////노드를 클릭하면 발생하는 이벤트
		//private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		//{
		//	try
		//	{
		//		//기존의 파일 목록 제거
		//		listView1.Items.Clear();
		//		DirectoryInfo dir = new DirectoryInfo(e.Node.FullPath);

		//		int DirectCount = 0;
		//		//하부 디렉토리 보여주기
		//		foreach(DirectoryInfo dirItem in dir.GetDirectories())
		//		{
		//			//하부 디렉토리가 존재할 경우 ListView에 추가
		//			//ListViewItem 객체를 생성
		//			ListViewItem lsvitem = new ListViewItem();

		//			//생성된 ListViewItem 객체에 똑같은 이미지를 할당
		//			lsvitem.ImageIndex = 2;
		//			lsvitem.Text = dirItem.Name;

		//			//아이템을 ListView(listView1)에 추가하기
		//			listView1.Items.Add(lsvitem);

		//			listView1.Items[DirectCount].SubItems.Add(dirItem.CreationTime.ToString());
		//			listView1.Items[DirectCount].SubItems.Add("폴더");
		//			listView1.Items[DirectCount].SubItems.Add(dirItem.GetFiles().Length.ToString() + " files");
		//			DirectCount++;
		//		}

		//		//디렉토리의 존재하는 파일목록 보여주기
		//		FileInfo[] fishow = dir.GetFiles();
		//		int Count = 0;
		//		foreach(FileInfo fri in fishow)
		//		{
		//			listView1.Items.Add(fri.Name);
		//			if(fri.LastWriteTime.ToString() != null)
		//			{
		//				listView1.Items[Count].SubItems.Add(fri.LastWriteTime.ToString());
		//			}
		//			else
		//			{
		//				listView1.Items[Count].SubItems.Add(fri.CreationTime.ToString());
		//			}
		//			listView1.Items[Count].SubItems.Add(fri.Attributes.ToString());
		//			listView1.Items[Count].SubItems.Add(fri.Length.ToString());
		//			Count++;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		MessageBox.Show("예외 발생 : " + ex.Message);
		//	}
		//	treeView1.Nodes[0].Expand();

		//}

		//private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		//{
		//	try
		//	{
		//		//기존의 파일 목록 제거
		//		listView1.Items.Clear();
		//		DirectoryInfo dir = new DirectoryInfo(listView1.SelectedItems[0].Text);

		//		int DirectCount = 0;
		//		//하부 디렉토리 보여주기
		//		foreach(DirectoryInfo dirItem in dir.GetDirectories())
		//		{
		//			//하부 디렉토리가 존재할 경우 ListView에 추가
		//			//ListViewItem 객체를 생성
		//			ListViewItem lsvitem = new ListViewItem();

		//			//생성된 ListViewItem 객체에 똑같은 이미지를 할당
		//			lsvitem.ImageIndex = 2;
		//			lsvitem.Text = dirItem.Name;

		//			//아이템을 ListView(listView1)에 추가하기
		//			listView1.Items.Add(lsvitem);
		//			//listView1.Items.Add(dirItem.Name);

		//			listView1.Items[DirectCount].SubItems.Add(dirItem.CreationTime.ToString());
		//			listView1.Items[DirectCount].SubItems.Add("폴더");
		//			listView1.Items[DirectCount].SubItems.Add(dirItem.GetFiles().Length.ToString() + " files");
		//			DirectCount++;
		//		}

		//		//디렉토리의 존재하는 파일목록 보여주기
		//		FileInfo[] fishow = dir.GetFiles();
		//		int Count = 0;
		//		foreach(FileInfo fri in fishow)
		//		{
		//			listView1.Items.Add(fri.Name);
		//			if(fri.LastWriteTime.ToString() != null)
		//			{
		//				listView1.Items[Count].SubItems.Add(fri.LastWriteTime.ToString());
		//			}
		//			else
		//			{
		//				listView1.Items[Count].SubItems.Add(fri.CreationTime.ToString());
		//			}
		//			listView1.Items[Count].SubItems.Add(fri.Attributes.ToString());
		//			listView1.Items[Count].SubItems.Add(fri.Length.ToString());
		//			Count++;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		MessageBox.Show("예외 발생 : " + ex.Message);
		//	}
		//}
	}
}
