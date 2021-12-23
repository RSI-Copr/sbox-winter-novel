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
	public partial class Novel : Panel
	{

		public static string ProtagonistName { get; set; } = "Insert name here";
		public static Novel RootPanel { get; private set; }
		public static NovelInfo Info { get; private set; }
		public static string FolderName { get; set; }

		public static int CurrentMessageId { get; set; } = 0;
		private static Message CurrentMessage;


		private static TextCanvas Canvas;
		private static AnswersCanvas Answers;
		private static CharacterInfo Character;
		private static Panel BlackScreen;
		private static SpritesCanvas Sprites;


		public Novel()
		{
			RootPanel = this;
			this.LoadDefaultStyleSheet();

			BlackScreen = Add.Panel( "black" );
			Sprites = AddChild<SpritesCanvas>();
			Answers = AddChild<AnswersCanvas>();
			Character = Add.Panel( "character-canvas" ).AddChild<CharacterInfo>();
			Canvas = Add.Panel( "canvas" ).AddChild<TextCanvas>();
			
			Canvas.OnTextShown = OnTextComplete;

			

			AcceptsFocus = true;
			Focus();
		}
		

		protected override void OnClick( MousePanelEvent e )
		{
			if ( Canvas.IsTyping )
			{
				Canvas.SpeedRun();
				return;
			}

			if(CurrentMessage?.HasAnswers ?? false)
			{
				return;
			}

			ShowMessage();
		}
		public override void OnButtonEvent( ButtonEvent e )
		{
			base.OnButtonEvent( e );
			if ( e.Button == "escape" )
			{
				this.SetVisible( false );
				Music.Pause(true);
			}
		}

		public static void Show()
		{
			RootPanel.SetVisible( true );
			Music.Pause( false );
		}
		public static void SetBlack(bool value )
		{
			BlackScreen.Style.Opacity = value ? 1 : 0;
		}
		public static void Start( NovelInfo info )
		{
			if ( RootPanel == null )
			{
				RootPanel = new();
			}
			RootPanel.SetVisible( true );
			Info = info;
			info.RunEvents();
			CurrentMessageId = 0;
			ShowMessage();
			Sprites.Clear();
		}
		private static void ShowMessage()
		{
			if ( CurrentMessageId == -1 )
			{
				RootPanel.SetVisible( false );
				return;
			}

			Info.RunMessageEvents( CurrentMessage );

			var msg = Info.Messages[CurrentMessageId];
			CurrentMessage = msg;
			Canvas.Print( msg.Text );
			Answers.Clear();
			
			Character.Set( msg.Character );

			CurrentMessageId = msg.NextMessage;
			Log.Info( CurrentMessageId );
		}
		private static void OnTextComplete()
		{
			if( CurrentMessage?.HasAnswers ?? false )
			{
				Answers.SetFrom( CurrentMessage );
			}
		}
		public static void SetBackground( string image )
		{
		
			//Texture.CreateArray().Finish( data: FS.ReadAllBytes( $"novels/{FolderName}/backgrounds/{image}" ).ToArray() );
			RootPanel.Style.SetBackgroundImage($"assets/backgrounds/{image}");
		}
		public static void SetText( string name )
		{
			Canvas.Print( name );
		}
		public static void RunSpriteEvent(int id, SpriteEventType spriteEvent, SpriteComeFrom spriteCome )
		{
			Sprites.DoSprite( id, spriteEvent, spriteCome );
		}



	    class CharacterInfo : Panel
		{
			private Label Label;

			public CharacterInfo()
			{
				Label = Add.Label();
			}
			public void Set(int id )
			{
				if(id == -1 )
				{
					this.SetVisible( false );
					Canvas.Style.BorderTopLeftRadius = Length.Pixels( 25 );
					
				}
				else
				{
					this.SetVisible( true );
					Label.Text = id == -2 ? ProtagonistName : Info.Characters[id];
					Canvas.Style.BorderTopLeftRadius = 0;
				}
				
			}
		}
		
		class AnswersCanvas:Panel
		{
			public void Clear()
			{
				DeleteChildren( true );
				Style.Opacity = 0;
			}

			public void SetFrom( Message msg )
			{
				foreach ( var ans in msg.Answers ) {
					Add.Button( ans.Text , () =>
					{
						CurrentMessageId = ans.NextMessage;
						ShowMessage();
						Info.RunAnswerEvents(ans);
					} );
				}
				Style.Opacity = 1;
			}
		}
		
		class TextCanvas : Panel
		{
			public const float Speed = 0.05f;
			public readonly Label Label;

			private string buffer;
			private RealTimeSince LastCharAdd;
			private int CurChar;

			public bool IsTyping => Label.Text.Length < buffer.Length;
			public Action OnTextShown { get; set; }

			public TextCanvas()
			{
				Label = AddChild<Label>();
			}

			public void Print(string text)
			{
				

				buffer = text.Replace("{PTG}", ProtagonistName,StringComparison.OrdinalIgnoreCase);
				LastCharAdd = 0;
				CurChar = 0;
				Label.Text = "";
			}

			public void SpeedRun()
			{
				Label.Text = buffer;
				OnTextShown?.Invoke();
			}

			public override void Tick()
			{
				if ( buffer != null && IsTyping && LastCharAdd >= Speed )
				{
					LastCharAdd = 0;
					Label.Text += buffer[CurChar];
					CurChar++;

					if ( !IsTyping )
					{
						OnTextShown?.Invoke();
					}
				}
			}
		}

		class SpritesCanvas:Panel
		{
			private Dictionary<int, Image> sprites = new();
			public SpritesCanvas()
			{

			}
			public void Clear()
			{
				foreach(var img in sprites.Values )
				{
					img.Delete( true );				
				}
				sprites.Clear();
			}

			public void DoSprite( int id, SpriteEventType spriteEvent, SpriteComeFrom spriteCome )
			{
				if(spriteEvent == SpriteEventType.Delete )
				{
					sprites.GetValueOrDefault( id )?.Delete();
					return;
				}

				

				var sprite = Info.Sprites[id];
				var img = Add.Image( $"assets/textures/{sprite.Name}","sprite" );
				Log.Info( spriteCome );
				switch ( spriteCome )
				{
					case SpriteComeFrom.Left:
						img.AddClass( "from-left" );
						break;
					case SpriteComeFrom.Rigth:
						img.AddClass( "from-right" );
						break;
				}

				sprites.Add( id, img );
			}
		}
	}
}
