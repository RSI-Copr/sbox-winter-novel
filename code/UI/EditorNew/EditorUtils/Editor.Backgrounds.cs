using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace TerryNovel.Editor
{
	static partial class Editor
	{
		public readonly static BackgroundsHandler Backgrounds = new();

		

		public class BackgroundsHandler{
			FileWatch fileWatch;
			public readonly HashSet<string> Files = new();

			public void TryStartWathForDir(string dir )
			{
				if ( !Novel.FS.DirectoryExists( dir ) ) return;

				fileWatch = Novel.FS.Watch( dir );
				fileWatch.OnChangedFile += OnFileChanged;
				fileWatch.OnChanges += OnChanged;
			

				foreach (var file in Novel.FS.FindFile( dir, "*.png" ) )
				{
					Files.Add( file );
				}
			}

			public IEnumerable<string> Get()
			{
				return Files.OrderBy(f=>f);
			}

			public void OnChanged(FileWatch watch)
			{
				Log.Info( watch.watchFiles );
			}

			public void OnFileChanged(string file )
			{
				Log.Info( file );
			}
		}

		
	}
}
