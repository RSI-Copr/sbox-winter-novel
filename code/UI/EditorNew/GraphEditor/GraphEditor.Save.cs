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
		public static void GenerateSaveFile()
		{
			Instance.Save();
		}

		public static void LoadFromFile(string name )
		{
			Instance.Load( name );
		}

		

		private bool CheckForValid()
		{

			return true;
		}

		private void Save()
		{
			var info = GetInfoNode();
			Log.Info( info );
			if(string.IsNullOrWhiteSpace( info.Title ) )
			{
				ShowError( "Novel must have name!" );
				return;
			}
			var dir = $"{ Novel.Directory}//{info.Title.ToLower().Replace(' ','_')}";
		
			FS.CreateDirectory( dir );
			FS.CreateDirectory( $"{dir}//backgrounds" );
			FS.CreateDirectory( $"{dir}//sounds" );

			Editor.Backgrounds.TryStartWathForDir( $"{dir}//backgrounds" );

			using var filestream = FS.OpenWrite( $"{dir}//.novel_project" );
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
					Log.Info( node.BaseInput.Id );
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
			var file = $"{ Novel.Directory}//{name}//.novel_project";
			if ( !FS.FileExists( file ) )
			{
				ShowError( "File was not founded!" );
				return;
			}


			Editor.Backgrounds.TryStartWathForDir( $"{ Novel.Directory}//{name}//backgrounds" );

			Clear();
			Plug.AutoIdAssignment = false;
			BaseNode.AutoIdAssignment = false;

			using var filestream = FS.OpenRead( file );
			using var reader = new BinaryReader( filestream );

			int node_count = reader.ReadInt32();

			for ( int i = 0; i < node_count;i++){
				int id = reader.ReadInt32();
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

			Plug.AutoIdAssignment = true;
			BaseNode.AutoIdAssignment = true;
		}

		private void ShowError(string error)
		{
			Log.Error( error );
		}
	}
}
