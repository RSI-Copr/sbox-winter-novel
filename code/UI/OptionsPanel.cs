using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

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

		public static void CloseAll()
		{
			foreach(var pnl in All )
			{
				
				pnl.Delete( true );
			}
			All.Clear();
		}

		public void AddOption( string name, Action onclick = null )
		{
			Add.Button( name, "optionbutton", () => { onclick?.Invoke(); Delete(); } );
		}
	}
}
