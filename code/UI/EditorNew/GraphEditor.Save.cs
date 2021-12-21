using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel.Editor
{
	partial class GraphEditor
	{
		public static void GenerateSaveFile()
		{
			Instance.Save();
		}

		private void Save()
		{
			var info = GetNodeById( 1 ) as InfoNode;
			if(string.IsNullOrWhiteSpace( info.Title ) )
			{
				ShowError( "Novel need name!" );
				return;
			} 
		}

		private void ShowError(string error)
		{
			Log.Error( error );
		}
	}
}
