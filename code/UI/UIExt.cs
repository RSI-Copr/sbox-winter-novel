using Sandbox.UI;
using Sandbox.UI.Construct;

public static class UIExt
{
	public static void SetVisible( this Panel pnl, bool b )
	{
		pnl.Style.Display = b ? DisplayMode.Flex : DisplayMode.None;
		pnl.Style.Opacity = b ? 1 : 0;
	}

	public static void LoadDefaultStyleSheet( this Panel pnl )
	{
		pnl.LoadStyleSheet( pnl.GetType().Name );
	}
	public static void LoadStyleSheet( this Panel pnl, string name )
	{
		pnl.StyleSheet.Load( $"UI/Styles/{name}.scss" );
	}
	public static void SetPosition( this Panel panel, Vector2 pos )
	{
		panel.Style.Left = Length.Pixels( pos.x );
		panel.Style.Top = Length.Pixels( pos.y );
	}

	public static Vector2 GetScreenPosition( this Panel panel )
	{
		return panel.PanelPositionToScreenPosition( Vector2.Zero );
	}

	public static Vector2 GetPosition( this Panel panel )
	{
		return new Vector2( panel.Style.Left?.Value ?? 0, panel.Style.Top?.Value ?? 0 );
		//return panel.PanelPositionToScreenPosition( Vector2.Zero );
	}

	public static TextEntry TextEntryWithPlaceHolder( this PanelCreator self, string text )
	{
		var control = self.panel.AddChild<TextEntry>();
		control.Placeholder = text;

		return control;
	}
}

