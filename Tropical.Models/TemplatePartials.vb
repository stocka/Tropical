Namespace Templates

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

  End Class

End Namespace
