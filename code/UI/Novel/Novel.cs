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

		public static string ProtagonistName => Local.DisplayName;
		public static Novel RootPanel { get; private set; }
		public static NovelInfo Info { get; private set; }
		public static string FolderName { get; set; }

		public static int CurrentMessageId { get; set; } = 0;
		private static Message CurrentMessage;
		private static bool Waiting = false;
		private static bool Ended = false;

		private static TextCanvas Canvas;
		private static AnswersCanvas Answers;
		private static Panel BlackScreen;
		private static SpritesCanvas Sprites;
		

		public Novel()
		{
			RootPanel = this;
			this.LoadDefaultStyleSheet();

			
			Sprites = AddChild<SpritesCanvas>();
			Answers = AddChild<AnswersCanvas>();
			BlackScreen = Add.Panel( "black" );
			Canvas = Add.Panel( "canvas" ).AddChild<TextCanvas>();
			
			
			Canvas.OnTextShown = OnTextComplete;
			
			AcceptsFocus = true;
			Focus();
		}
		

		protected override void OnClick( MousePanelEvent e )
		{
			Click();
		}
		public override void OnButtonEvent( ButtonEvent e )
		{
			base.OnButtonEvent( e );
			if ( e.Button == "escape" )
			{
				Hide();
			}
		}
		public static void EndGameAndWait()
		{
			
			Canvas.SetVisible( false );
			Ended = true;
		}

		public static void HideHudAndWait()
		{
			Canvas.SetVisible( false );
			Waiting = true;
		}


		public void Click()
		{
			if( Ended )
			{
				return;
			}

			if ( Canvas.IsTyping )
			{
				Canvas.SpeedRun();
				return;
			}

			if ( CurrentMessage?.HasAnswers ?? false )
			{
				return;
			}

			if( Waiting )
			{
				Canvas.SetVisible( true );
				Waiting = false;
				return;
			}

			ShowMessage();
		}


		public static void Show()
		{
			RootPanel.Focus();
			RootPanel.SetVisible( true );
			Music.Pause( false );
		}

		public static void Hide()
		{
			RootPanel.SetVisible( false );
			Music.Pause( true );
		}


		public static void SetBlack(bool value )
		{
			
			BlackScreen.SetClass( "smooth", CurrentMessageId != 0 );
			BlackScreen.Style.Opacity = value ? 1 : 0;
			Canvas.SetClass( "hidden", value );
		}
		public static void Start( NovelInfo info )
		{
			if ( RootPanel == null )
			{
				RootPanel = new();
			}
			Sprites.Clear();
			RootPanel.SetVisible( true );
			Canvas.SetVisible( true );
			Waiting = false;
			Ended = false;
			Info = info;
			CurrentMessageId = 0;
			CurrentMessage = null;
			info.RunEvents();
			
			ShowMessage();
			
		}
		private static void ShowMessage()
		{
			if ( CurrentMessageId == -1 )
			{
				EndGameAndWait();
				return;
			}

			Info.RunMessageEvents( CurrentMessage );
			


			var msg = Info.Messages[CurrentMessageId];
			CurrentMessage = msg;
			Canvas.Print( msg.Text );
			Answers.Clear();
			Canvas.SetCharacter( msg.Character );
			
			CurrentMessageId = msg.NextMessage;
			//Log.Info( CurrentMessageId );
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
			SetBlack( false );
		}
		public static void SetText( string name )
		{
			Canvas.Print( name );
		}
		public static void RunSpriteEvent(int id, SpriteEventType spriteEvent, SpriteComeFrom spriteCome )
		{
			Sprites.DoSprite( id, spriteEvent, spriteCome );
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
			public const float Speed = 0.020f;
			public readonly Label Label;
			public readonly Label CharLabel;

			private string buffer;
			private RealTimeSince LastCharAdd;
			private int CurChar;

			public bool IsTyping => Label.Text.Length < buffer.Length;
			public Action OnTextShown { get; set; }

			public TextCanvas()
			{
				CharLabel = AddChild<Label>( "character-label" );
				Label = AddChild<Label>("message-label");
			}


			public void SetCharacter( int id )
			{
				if ( id == -1 )
				{
					CharLabel.Style.Opacity = 0;

				}
				else
				{
					CharLabel.Style.Opacity = 1;
					CharLabel.Text = id == -2 ? ProtagonistName : Info.Characters[id];
				}

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
			private Dictionary<int, Panel> sprites = new();
			public SpritesCanvas()
			{

			}
			public void Clear()
			{
				foreach(var img in sprites.Values )
				{
					img?.Delete( true );				
				}
				sprites.Clear();
			}

			public void DoSprite( int id, SpriteEventType spriteEvent, SpriteComeFrom spriteCome )
			{
				if(spriteEvent == SpriteEventType.Delete )
				{
					sprites.GetValueOrDefault( id )?.Delete();
					sprites.Remove( id );
					return;
				}

				if ( sprites.ContainsKey( id ) ) return;
				

				

				var sprite = Add.Panel( "sprite" );
				//sprite.Style.SetBackgroundImage(Novel.LoadTexture( CurrentFileSystem ,)
				sprite.Style.SetBackgroundImage( $"assets/textures/{Info.Sprites[id].Name}" );

				
				//Log.Info( spriteCome );
				switch ( spriteCome )
				{
					case SpriteComeFrom.Left:
						sprite.AddClass( "from-left" );
						break;
					case SpriteComeFrom.Rigth:
						sprite.AddClass( "from-right" );
						break;
				}

				sprites.Add( id, sprite );
			}
		}
	}
}
