using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TerryNovel.Editor
{
	partial class GraphEditor
	{
		private static BaseFileSystem FS => Novel.FS;
		

		public static void LoadFromFile(string name )
		{
			Instance.Load( name );
		}



		public void GenerateSaveFile()
		{
			var info = GetInfoNode();
			
			if(string.IsNullOrWhiteSpace( info.Title ) )
			{
				ShowError( "Novel must have name!" );
				return;
			}

			using var filestream = FS.OpenWrite( Editor.Dir.ProjectFile );
			using var writer = new BinaryWriter( filestream );

			writer.Write( BaseNode.All.Count );

			foreach(var node in BaseNode.All )
			{
				writer.Write( node.Id );
				writer.Write( node.GetType().Name );
				writer.Write( GetRelativePosition( node ) );

			

				bool has_input = node.BaseInput != null;
				writer.Write( has_input );
				if ( has_input )
				{
					writer.Write( node.BaseInput.Id );
				}

				bool has_output = node.BaseOutput != null;
				writer.Write( has_output );
				if ( has_output )
				{
					writer.Write( node.BaseOutput.Id );
				}


				node.Write( writer );
			}

			writer.Write( Connection.All.Count );
			foreach ( var con in Connection.All ) 
			{
				writer.Write( con.Output.Id );
				writer.Write( con.Input.Id );
			}
			

			writer.Close();
			filestream.Close();
		}

		private void Load( string name )
		{
			var file = $"novels/{name}/novel_project";


			if ( !FS.FileExists( file ) )
			{
				ShowError( "File was not founded!" );
				return;
			}
			
			Clear();
			Plug.AutoIdAssignment = false;
			BaseNode.AutoIdAssignment = false;

			
			using var filestream = FS.OpenRead( file );
			using var reader = new BinaryReader( filestream );

			int node_count = reader.ReadInt32();

			for ( int i = 0; i < node_count;i++){
				int id = reader.ReadInt32();

				BaseNode.CurrentId = id;

				string classname = reader.ReadString();
				var pos = reader.ReadVector2();

				var node = CreateNode( classname, id, pos );

				if ( reader.ReadBoolean() )
				{
					node.BaseInput.SetId( reader.ReadInt32() );
				}

				if ( reader.ReadBoolean() )
				{
					node.BaseOutput.SetId( reader.ReadInt32());
				}

				

				node.Read( reader );
			}

			int con_count = reader.ReadInt32();

			for ( int i = 0; i < con_count; i++ )
			{
				var output = Plug.GetById( reader.ReadInt32() ) as PlugOut;
				var input = Plug.GetById( reader.ReadInt32() ) as PlugIn;

				if(output != null && input != null )
				{
					Connection.Create( output, input );
					
				}

			}

			reader.Close();
			filestream.Close();

			Editor.Dir.Name = name;

			Plug.CurrentId = Plug.Dict.Values.Max( p => p.Id ) +1;
			BaseNode.CurrentId = BaseNode.All.Max( n => n.Id )+1;

			Plug.AutoIdAssignment = true;
			BaseNode.AutoIdAssignment = true;


			foreach(var node in BaseNode.All )
			{
				node.OnPostLoad();
			}
			Event.Run( "editor.loaded" );
		}

		private void ShowError(string error)
		{
			Log.Error( error );
		}
	}
}
