using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel
{
	public partial class Novel:Panel
	{
		public const string Directory = "novels";
		private static Novel Instance;
		private TextCanvas Canvas;
		public  Novel()
		{
			Instance = this;
			this.LoadDefaultStyleSheet();

			

			Canvas = Add.Panel("canvas").AddChild<TextCanvas>();
			AcceptsFocus = true;
			Focus();

		}

		public override void OnButtonEvent( ButtonEvent e )
		{
			base.OnButtonEvent( e );
			if ( e.Button == "escape" ) this.SetVisible(!IsVisible);
		}

		public static BaseFileSystem FS => FileSystem.Data;

		public static void CreateDir()
		{
			FS.CreateDirectory( Directory );
		}
		[ClientCmd]
		public static void FindAll()
		{
			foreach(var dir in FS.FindDirectory( Directory ) )
			{
				var filename = FS.FindFile( $"{Directory}\\{dir}", "*.novel" ).FirstOrDefault();
				if ( filename == null ) continue;
				LoadFromFile( $"{Directory}\\{dir}\\{filename}" );
			}
		}

		public static void LoadFromFile(string filename )
		{

		}


		public static void SetBackground( string image )
		{
			Instance.Style.SetBackgroundImage( image );
		}


		[ClientCmd]
		public static void SetText( string name )
		{
			Instance.Canvas.SetText( name );
		}


		public static void Load(string name )
		{
			Instance.SetVisible( true );

		}

		class TextCanvas : Panel
		{
			public const float Speed = 0.05f;
			public readonly Label Label;
			private string buffer;
			private RealTimeSince LastCharAdd;
			private int CurChar;

			public TextCanvas()
			{
				Label = AddChild<Label>();
			}

			public void SetText(string text)
			{
				buffer = text;
				LastCharAdd = 0;
				CurChar = 0;
				Label.Text = "";
			}

			public override void Tick()
			{
			    if( buffer != null && CurChar < buffer.Length && LastCharAdd >= Speed )
				{
					LastCharAdd = 0;
					Label.Text += buffer[CurChar];
					CurChar++;
				}
			}
		}
	}
}
