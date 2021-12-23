using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryNovel.Editor;

namespace TerryNovel
{
	[UseTemplate]
	public class MainMenu:Panel
	{
		public MainMenu()
		{
			

			
		}

		public void Startnovel()
		{

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
	}
}
