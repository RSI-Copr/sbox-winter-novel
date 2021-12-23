using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using TerryNovel.Editor;

namespace TerryNovel
{
	[UseTemplate]
	public class MainMenu : Panel
	{
		private VirtualScrollPanel NovelsList;
		public MainMenu()
		{
			NovelsList = AddChild<VirtualScrollPanel>( "novels-list" );
			NovelsList.Layout.ItemHeight = Length.Percent( 50f ).Value;
			NovelsList.OnCreateCell = OnCellCreated;


		}

		private void OnCellCreated( Panel cell, object data )
		{
			cell.AddChild<NovelEntry>();
		}


		public class NovelEntry:Panel{
			public NovelEntry()
			{
				AddClass( "entry" );

				var pnl = Add.Panel( "labels" );

				pnl.Add.Label( "Test", "title" );
				pnl.Add.Label( "Description", "desc" );

				Add.Panel( "img-canvas" ).Add.Image( "UI/background-grid.png" );
			}
		}


		public void Startnovel()
		{
			NovelsList.SetVisible( true );

			NovelsList.AddItem( "test" );
		}

		public void ShowAbout()
		{
			Frame.Show<AboutFrame>( true );
		}

		public void ShowEditor()
		{
			EditorWindow.Open();
		}
		public void Exit()
		{
			Local.Client.Kick();
		}




	}



}

