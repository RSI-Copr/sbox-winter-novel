using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;



namespace TerryNovel
{
	partial class Novel
	{
		[ClientCmd("save_novel")]
		public static void Save()
		{
			var save_file_name = $"{FolderName}_{DateTime.Now.ToString( "yy_MM_dd_HH_mm_ss" )}";
			using var stream = FS.OpenRead( $"saves/{save_file_name}.save" );
			using var writer = new BinaryWriter( stream );

			writer.Write( Info.Name );
			writer.Write( DateTime.Now.ToBinary() );
			writer.Write( CurrentMessageId );
			


		}
	}
}
