using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace TerryNovel.Editor
{
	internal class EditorWindow : Panel
	{
		private static EditorWindow Instance;
		public EditorWindow()
		{
			Instance = this;

			this.LoadDefaultStyleSheet();
			AddChild<GraphEditor>();

			var bar = AddChild<Panel>( "bar" );
			var attributes = Library.GetAttributes<ContextButtonAttribute>().Where( a => a.IsStaticMethod ).GroupBy( a => a.Group );

			foreach ( var g in attributes )
			{
				var btn = bar.Add.Button( g.Key );


				btn.AddEventListener( "onclick", () =>
				{

					var popup = new Popup( btn, Popup.PositionMode.BelowLeft, 0 );
					foreach ( var attr in g )
					{
						popup.AddOption( attr.Title, () => attr.InvokeStatic() );
					}

				} );
			}
		}

		[ContextButton( "File", "New" )]
		public static void CreateNew()
		{
			Frame.Show<NewProjectFrame>();
			
		}

		[ContextButton( "File", "Open" )]
		public static void OpenFile()
		{
			ProjectsFrame.Open();
		}
		[ContextButton( "File", "Save" )]
		public static void SaveToFile()
		{
			GraphEditor.Instance.GenerateSaveFile();
		}


		[ContextButton( "File", "Close" )]
		public static void CloseEditor()
		{
			Instance.Blur();
			Instance.Delete();
			Instance = null;
		}


		[ContextButton( "Novel", "Run" )]
		public static void Run()
		{
			GraphEditor.Instance.RunNovel();
		}


		[ContextButton( "Novel", "Show" )]
		public static void ShowNovel()
		{
			Novel.Show();
		}

		[ContextButton( "Novel", "Compile" )]
		public static void CompileNovel()
		{
			GraphEditor.Instance.GenetateNovelFile();
		}
		

		public static void Open()
		{
			if(Instance == null )
			{
				Instance = Local.Hud.AddChild<EditorWindow>();
			}
			Instance.SetVisible(true);
			Instance.Focus();
		}

		public static void Close()
		{
			Instance.SetVisible( false );
			Instance.Blur();
		}

	}

	[AttributeUsage( AttributeTargets.Method )]
	public class ContextButtonAttribute : LibraryMethod
	{
		public ContextButtonAttribute( string category, string title )
		{
			Title = title;
			Group = category;
		}
	}


}
