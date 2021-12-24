using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerryNovel
{
	public class OptionsPanel : Panel
	{
		private static readonly List<OptionsPanel> All = new();
		public OptionsPanel()
		{
			OptionsPanel.CloseAll();
			All.Add( this );
			Style.ZIndex = 100;
			Local.Hud.AddChild( this );
			this.SetPosition( Mouse.Position );

		}
		
		public void AddSpacer(string name )
		{
			Add.Label( name, "spacer" );
		}

		public void AddOption( string name, Action onclick = null )
		{
			Add.Button( name, "optionbutton", () => { onclick?.Invoke(); Delete(); } );
		}

		public static void CloseAll( Panel exceptThisOne = null )
		{
			foreach ( var panel in All )
			{
				if ( panel == exceptThisOne ) continue;

				panel.Delete();
			}
			All.Clear();
		}

		[Event( "ui.closepopups" )]
		public static void ClosePopupsEvent( object obj )
		{
			OptionsPanel floater = null;

			if ( obj is Panel panel )
			{
				floater = panel.AncestorsAndSelf.OfType<OptionsPanel>().FirstOrDefault();
			}

			CloseAll( floater );
		}

		public override void OnLayout( ref Rect layoutRect )
		{
			var padding = 10;
			var h = Screen.Height - padding;
			var w = Screen.Width - padding;

			if ( layoutRect.bottom > h )
			{
				layoutRect.top -= layoutRect.bottom - h;
				layoutRect.bottom -= layoutRect.bottom - h;
			}

			if ( layoutRect.right > w )
			{
				layoutRect.left -= layoutRect.right - w;
				layoutRect.right -= layoutRect.right - w;
			}
		}
	}
}
