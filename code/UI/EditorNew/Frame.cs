using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerryNovel.Editor
{
	public class Frame:Panel
	{
		protected Panel Header;
		protected Label HeaderLabel;

		private bool Moving;
		private Vector2 MouseMoveOffset;
		protected Panel Canvas;

		public string HeaderText
		{
			get => HeaderLabel.Text;
			set => HeaderLabel.Text = value;
		}

		public Frame()
		{
			this.LoadDefaultStyleSheet();
			AddClass( "frame" );
			Local.Hud.AddChild( this );


			Header = Add.Panel( "header" );
			Header.Add.Button( "close", "close" ,() =>Delete());
			HeaderLabel = Header.Add.Label( "", "title" );
			Canvas = Add.Panel( "canvas" );
		}

		protected override void OnMouseDown( MousePanelEvent e )
		{
			
			if(e.Target == Header && e.Button == "mouseleft" )
			{
				Moving = true;
				MouseMoveOffset = this.GetPosition();
			}
		}

		protected override void OnMouseUp( MousePanelEvent e )
		{
			Moving = false;
		}

		public override void Tick()
		{
			if ( Moving )
			{
				MouseMoveOffset += Mouse.Delta;
				this.SetPosition( MouseMoveOffset );
			}
		}
	}
}
