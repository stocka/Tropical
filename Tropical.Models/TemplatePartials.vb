Namespace Templates

  Partial Class SpriteSheetSampleHtml

    Private mSheet As SpriteSheet

    Public Sub New(sheet As SpriteSheet)
      mSheet = sheet
    End Sub

  End Class

  Partial Class SpriteSheetTemplate

    Private mSheet As SpriteSheet

    Public Sub New(sheet As SpriteSheet)
      mSheet = sheet
    End Sub

  End Class

  Partial Class SpriteTemplate

    Private mSprite As PlacedSprite

    Public Sub New(sprite As PlacedSprite)
      mSprite = sprite
    End Sub

    ''' <summary>
    ''' Determines if the CSS for the standard sprite should
    ''' be included.
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if the CSS for the standard sprite should
    ''' be included; <c>false</c> otherwise.
    ''' </returns>
    Private Function ShouldIncludeStandardCss() As Boolean
      Return mSprite.Position.HasValue()
    End Function

    ''' <summary>
    ''' Determines if the CSS for the hover sprite should
    ''' be included.
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if the CSS for the hover sprite should
    ''' be included; <c>false</c> otherwise.
    ''' </returns>
    Private Function ShouldIncludeHoverCss() As Boolean

      ' If it's not positioned, don't render it.
      If Not mSprite.HoverPosition.HasValue() Then
        Return False
      End If

      ' Now we check if the standard and hover CSS are identical.
      ' If we don't have a standard position, then we're okay
      ' with rendering it.
      If Not mSprite.Position.HasValue() Then
        Return True
      End If

      ' We have both standard and hover CSS, but let's make sure
      ' they don't refer to the same thing - otherwise, the
      ' standard CSS will suffice.
      Return mSprite.Position.Value() <> mSprite.HoverPosition.Value()

    End Function

    ''' <summary>
    ''' Gets the CSS string (&quot;x px y px&quot;) for the provided position.
    ''' The x and y values of the position will be negated.
    ''' </summary>
    ''' <param name="position">The point representing the position.</param>
    ''' <returns>The equivalent CSS position string.</returns>
    Private Function GetPositionString(position As System.Drawing.Point) As String
      Return (-position.X).ToString() & "px " & (-position.Y).ToString() & "px"
    End Function

  End Class

End Namespace
