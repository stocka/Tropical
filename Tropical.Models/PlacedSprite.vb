''' <summary>
''' A sprite that has been placed in a sheet at a specific location.
''' </summary>
Public Class PlacedSprite

  ''' <summary>
  ''' Gets or sets the name of the base CSS class
  ''' used for all sprites.
  ''' </summary>
  ''' <value>
  ''' The name of the base CSS class used for all sprites.
  ''' </value>
  Public Property BaseClassName As String

  ''' <summary>
  ''' Gets or sets the position of the sprite in the image.
  ''' </summary>
  ''' <value>
  ''' The position of the sprite in the image.
  ''' </value>
  Public Property Position As Nullable(Of System.Drawing.Point)

  ''' <summary>
  ''' Gets or sets the position of the hover sprite in the image.
  ''' </summary>
  ''' <value>
  ''' The position of the hover sprite in the image.
  ''' </value>
  Public Property HoverPosition As Nullable(Of System.Drawing.Point)

  ''' <summary>
  ''' Gets or sets the name of the CSS class used for this specific sprite.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class used for this specific sprite.
  ''' </value>
  Public Property ClassName As String

End Class
