using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
namespace TerryNovel.Editor
{
	[Node(Title = "Novel information", CanCreate = false, HasInput = false, HasOutput = false)]
	internal class InfoNode:BaseNode
	{
		public string Title;
		public string Desc;

		protected override void CreateContents()
		{
			Canvas.Add.TextEntry("");
			Canvas.Add.TextEntry("");

			
		}
	}
}
