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
		private static MainMenu Instance;
		private NovelCanvas NovelCanvas;
		public Panel Snow { get; set; }
		public Button ReturnBtn { get; set; }
		public MainMenu()
		{
			NovelCanvas = AddChild<NovelCanvas>();
			Instance = this;
			ReturnBtn.SetVisible( false );
		}

		public static void SetReturnButtonActive(bool value )
		{
			Instance.ReturnBtn.SetVisible( value );
		}

		public void ReturnToGame()
		{
			Novel.Show();
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
			//x_offset++;
			
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
			StartButtton = btns.Add.Button( "Start", "start", Start );

		}

		private void Start()
		{
			this.SetVisible( false );
			Novel.LoadFrom( Selected.fileSystem, Selected.directory );
		}

		public void AddCategory(string name)
		{
			List.Add.Panel( "category" ).Add.Label( name );
		}

		public void Update()
		{
			List.DeleteChildren( true );
			StartButtton.SetClass( "disabled",true );
			AddCategory( "Main" );
			FindFrom( FileSystem.Mounted, "base_novels/main" );
			AddCategory( "Bonus" );
			FindFrom( FileSystem.Mounted, "base_novels/bonus" );
			AddCategory( "Custom" );
			FindFrom( FileSystem.Data, "novels" );

		}


		

		public void FindFrom(BaseFileSystem fileSystem, string base_dir )
		{
			foreach(var dir in Novel.FindAll( fileSystem, base_dir ) )
			{
				
				
				if( Novel.ReadInfo( fileSystem, dir, out var name, out var desc ) )
				{
					var entry = new NovelEntry( fileSystem, dir, name, desc );
					entry.AddEventListener( "onclick", () =>
					{
						Select( entry );


					} );
					List.AddChild( entry );
				}
			}
		}

		private NovelEntry Selected;


		private void Select( NovelEntry entry )
		{
			Selected?.SetClass( "selected", false );
			Selected = entry;
			Selected?.SetClass( "selected", true );
			StartButtton.SetClass( "disabled", false );
		}

		public class NovelEntry : Panel
		{
			public readonly BaseFileSystem fileSystem;
			public readonly string directory;
			public NovelEntry( BaseFileSystem fileSystem, string dir, string name, string desc)
			{
				this.fileSystem = fileSystem;
				directory = dir;


				AddClass("entry");

				var pnl = Add.Panel("labels");

				pnl.Add.Label( name, "title");
				pnl.Add.Label( desc, "desc");

				var f_name = $"{dir}preview.png";


				var img_canvas = Add.Panel( "img-canvas" );

				if (fileSystem.FileExists( f_name ) )
				{
					img_canvas.Add.Image( Novel.LoadTexture( fileSystem, f_name) );
				}
				else
				{
					img_canvas.Add.Image( "UI/background-grid.png" );
				}

				
			}

			

		}

	}


}

