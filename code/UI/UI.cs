using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerryNovel.Editor;

namespace TerryNovel
{

	public static class UI
	{

		private static RootPanel RootPanel;
		private static void CreateRootPanel()
		{
			RootPanel = new RootPanel();
			RootPanel.AcceptsFocus = false;
		    RootPanel.Style.PointerEvents = "all";
			RootPanel.LoadStyleSheet( "base" );
			Local.Hud = RootPanel;
		}

		[Event.Entity.PostSpawn]
		public static void Init()
		{
			if ( !Host.IsClient ) return;
			CreateRootPanel();
			CreateChildPanels();
			
		}


		public static void CreateChildPanels()
		{
			
			AddPanel<MainMenu>();
			AddPanel<Novel>();
		}

		private static void AddPanel<T>() where T : Panel, new()
		{
			RootPanel.AddChild<T>();
		}
		public static void Destroy()
		{

			if ( Local.Hud == RootPanel )
			{
				Local.Hud = null;
			}

			RootPanel?.Delete();
			RootPanel = null;
		}

		[ClientCmd("ui_rebuild")]
		public static void Rebuild()
		{
			RootPanel.DeleteChildren( true );
			CreateChildPanels();
		}

		


	}

}
