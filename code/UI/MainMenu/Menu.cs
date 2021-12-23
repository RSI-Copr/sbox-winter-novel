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

		private NovelCanvas NovelCanvas;

		public MainMenu()
		{
			NovelCanvas = AddChild<NovelCanvas>();

		}
		
		public void Startnovel()
		{
			NovelCanvas.SetVisible(true);
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

	public class NovelCanvas: Panel
	{
		
		private Panel NovelsList;
		private Button StartButtton;
		public NovelCanvas()
		{
			AddClass("novels-canvas");




			NovelsList = Add.Panel( "novels-list" );

			NovelsList.Add.Panel("category").Add.Label("Main");/////

			NovelsList.AddChild<NovelEntry>();
			NovelsList.AddChild<NovelEntry>();

			NovelsList.Add.Panel( "category" ).Add.Label( "Bonus" );/////

			NovelsList.AddChild<NovelEntry>();

			var btns = Add.Panel("buttons");
			
			var back = btns.Add.Button( "Back","back" );
			StartButtton = btns.Add.Button( "Start", "start" );

		}

		private void OnCellCreated(Panel cell, object data)
		{
			cell.AddChild<NovelEntry>();
		}


		public class NovelEntry : Panel
		{
			public NovelEntry()
			{
				AddClass("entry");

				var pnl = Add.Panel("labels");

				pnl.Add.Label("Test", "title");
				pnl.Add.Label("Description", "desc");

				Add.Panel("img-canvas").Add.Image("UI/background-grid.png");
			}
		}

	}


}

