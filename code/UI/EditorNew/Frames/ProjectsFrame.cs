using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.UI.Tests;

namespace TerryNovel.Editor
{
	public class ProjectsFrame:Frame
	{
		private VirtualScrollPanel FileList;
		private static ProjectsFrame Instance;
		public ProjectsFrame()
		{
			this.LoadDefaultStyleSheet();
			HeaderText = "Open novel for editing";

			FileList = Canvas.AddChild<VirtualScrollPanel>( "list" );

			FileList.Layout.ItemHeight = 20f;
			FileList.OnCreateCell = OnCellCreated;

			FileList.AddItems( Novel.FS.FindDirectory( $"{Novel.Directory}" ).ToArray() );


		}

		private void FindNovels()
		{

		}

		private void OnCellCreated( Panel cell, object data )
		{
			var file = (string)data;
			cell.Add.Button( file, () => { GraphEditor.LoadFromFile( file ); Delete(); } );
		}

		

		public static void Open()
		{
			if( Instance != null && Instance.IsValid() )
			{
				return;
			}
			_ = new ProjectsFrame();
		}


	}
}
