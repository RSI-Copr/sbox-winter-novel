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
		public Panel Snow { get; set; }

		public MainMenu()
		{
			NovelCanvas = AddChild<NovelCanvas>();

		}
		
		public void Startnovel()
		{
			NovelCanvas.SetVisible(true);
			NovelCanvas.Update();
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
		private float x_offset = 0;
		public override void Tick()
		{
			//Snow.Style.BackgroundPositionX = x_offset;
			x_offset++;
			
		}
	}

	public class NovelCanvas : Panel
	{

		private Panel List;
		private Button StartButtton;



		public NovelCanvas()
		{
			AddClass( "novels-canvas" );
			List = Add.Panel( "novels-list" );
			
			var btns = Add.Panel( "buttons" );


			
			var back = btns.Add.Button( "Back", "back", ()=> this.SetVisible(false) );
			StartButtton = btns.Add.Button( "Start", "start" );

		}

		public void AddCategory(string name)
		{
			List.Add.Panel( "category" ).Add.Label( name );
		}

		public void Update()
		{
			List.DeleteChildren( true );
			AddCategory( "Main" );
			FindFrom( FileSystem.Mounted, "base_novels/main/" );
			AddCategory( "Bonus" );
			FindFrom( FileSystem.Mounted, "base_novels/bonus/" );
			AddCategory( "Custom" );
			FindFrom( FileSystem.Data, "novels/" );

		}


		

		public void FindFrom(BaseFileSystem fileSystem, string dir )
		{
			foreach(var file in Novel.FindAll( fileSystem, dir ) )
			{
				if( Novel.ReadInfo( fileSystem, file, out var name, out var desc ) )
				{
					var entry = new NovelEntry( fileSystem, dir, name, desc );
					entry.AddEventListener( "onclick", () => Novel.LoadFromFile( fileSystem, file ) );
					List.AddChild( entry );
				}
			}
		}


		public class NovelEntry : Panel
		{
			public NovelEntry( BaseFileSystem fileSystem, string dir, string name, string desc)
			{
				AddClass("entry");

				var pnl = Add.Panel("labels");

				pnl.Add.Label( name, "title");
				pnl.Add.Label( desc, "desc");

				var f_name = $"{dir}/preview.png";

				var img_canvas = Add.Panel( "img-canvas" );

				if (fileSystem == FileSystem.Mounted && fileSystem.FileExists( name ) )
				{
					img_canvas.Add.Image( f_name );
				}
				else
				{
					img_canvas.Add.Image( "UI/background-grid.png" );
				}

				
			}

		}

	}


}

